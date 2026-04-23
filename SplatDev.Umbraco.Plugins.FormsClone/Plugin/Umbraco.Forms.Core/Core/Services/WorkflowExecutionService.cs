
// Type: Umbraco.Forms.Core.Services.WorkflowExecutionService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Collections;
using System.Data;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services.Notifications;

using IScope = Umbraco.Cms.Core.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Core.Scoping.IScopeProvider;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
    internal sealed class WorkflowExecutionService : IWorkflowExecutionService
    {
        private readonly IRecordStorage _recordStorage;
        private readonly IWorkflowService _workflowService;
        private readonly IPageService _pageService;
        private readonly IScopeProvider _scopeProvider;
        private readonly WorkflowCollectionFactory _workflowCollectionFactory;
        private readonly IPlaceholderParsingService _placeholderParsingService;
        private readonly IEventMessagesFactory _eventMessagesFactory;
        private readonly IProfilingLogger _profilingLogger;
        private readonly ILogger<WorkflowExecutionService> _logger;
        private readonly PackageOptionSettings _config;
        private readonly IFieldTypeStorage _fieldTypeStorage;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WorkflowExecutionService(
          IRecordStorage recordStorage,
          IWorkflowService workflowService,
          IPageService pageService,
          IScopeProvider scopeProvider,
          WorkflowCollectionFactory workflowCollectionFactory,
          IPlaceholderParsingService placeholderParsingService,
          IEventMessagesFactory eventMessagesFactory,
          IProfilingLogger profilingLogger,
          ILogger<WorkflowExecutionService> logger,
          IOptions<PackageOptionSettings> config,
          IFieldTypeStorage fieldTypeStorage,
          IHttpContextAccessor httpContextAccessor)
        {
            this._recordStorage = recordStorage;
            this._workflowService = workflowService;
            this._scopeProvider = scopeProvider;
            this._pageService = pageService;
            this._workflowCollectionFactory = workflowCollectionFactory;
            this._profilingLogger = profilingLogger;
            this._placeholderParsingService = placeholderParsingService;
            this._eventMessagesFactory = eventMessagesFactory;
            this._logger = logger;
            this._config = config.Value;
            this._fieldTypeStorage = fieldTypeStorage;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<WorkflowExecutionResult> ExecuteWorkflowsAsync(
          Record record,
          Form form,
          FormState state,
          bool editMode)
        {
            bool flag = false;
            if (editMode)
                flag = this.ShouldWorkflowsBeIgnored(form);
            if (flag)
                return WorkflowExecutionResult.Create();
            return await this.ExecuteWorkflowsAsync(this._workflowService.GetActiveWorkFlows(form, state), record, form, state).ConfigureAwait(false);
        }

        private bool ShouldWorkflowsBeIgnored(Form form)
        {
            string lower = this._config.IgnoreWorkFlowsOnEdit.ToLower();
            if (string.IsNullOrEmpty(lower) || lower == "false")
                return false;
            return lower == "true" || lower.Split(',').Contains<string>(form.Name.ToLower());
        }

        public async Task<WorkflowExecutionResult> ExecuteWorkflowsAsync(
            List<Workflow> workflows,
            Record record,
            Form form,
            FormState state)
        {
            if (record is null)
                return WorkflowExecutionResult.Create();

            using var scope = _scopeProvider.CreateScope(
                IsolationLevel.Unspecified,
                RepositoryCacheMode.Unspecified,
                null,
                null,
                null,
                false,
                false
            );

            var result = WorkflowExecutionResult.Create();

            if (workflows.Count == 0)
                return result;

            var formState = record.CreateFormStateFromRecord(form, _fieldTypeStorage);
            var formFieldValues = FieldConditionEvaluation.GetFormFieldValuesForConditions(
                formState,
                form.AllFields,
                _fieldTypeStorage,
                _httpContextAccessor.HttpContext!
            );

            var pageElements = record.UmbracoPageId > 0
                ? _pageService.GetPageElements(record.UmbracoPageId)
                : _pageService.GetPageElements();

            var additionalData = record.GetAdditionalData() ?? new Dictionary<string, string>()!;
            var workflowCollection = _workflowCollectionFactory.GetWorkflowCollection();

            foreach (var workflow in workflows)
            {
                var workflowType = workflowCollection[workflow.WorkflowTypeId];

                if (IsWorkflowExcludedByConditions(form, workflow, formFieldValues))
                {
                    var audit = CreateRecordAudit(record, workflow, workflowType, state,
                        WorkflowExecutionStatus.SkippedDueToConditions);
                    RecordAuditRecord(scope, form, audit);
                    result.AddRecordAudit(audit);
                    continue;
                }

                using (_profilingLogger.DebugDuration<WorkflowExecutionService>(
                    $"Executing {state} Workflow '{workflow.Name}' " +
                    $"for the Form '{form.Name}' with id '{form.Id}'"))
                {
                    workflowType.LoadSettings(workflow, _placeholderParsingService,
                        record, form, pageElements, additionalData);

                    var eventMessages = _eventMessagesFactory.Get();
                    var startNotification = RaiseWorkflowStartedEvent(scope, workflow, record, eventMessages);

                    try
                    {
                        var executionStatus = await ExecuteWorkflow(
                            workflowType, record, form, state,
                            pageElements, additionalData, workflow
                        ).ConfigureAwait(false);

                        var audit = CreateRecordAudit(record, workflow, workflowType, state, executionStatus);
                        RecordAuditRecord(scope, form, audit);
                        result.AddRecordAudit(audit);
                        RaiseWorkflowCompletedEvent(scope, workflow, record, eventMessages,
                            executionStatus, startNotification);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Form workflow {WorkflowName} failed on {FormState} of Record {RecordId} from Form {FormName} ({FormId})",
                            workflow.Name, state, record.UniqueId, form.Name, form.Id);

                        var audit = CreateRecordAudit(record, workflow, workflowType, state,
                            WorkflowExecutionStatus.Failed);
                        RecordAuditRecord(scope, form, audit);
                        result.AddRecordAudit(audit);

                        RaiseWorkflowFailedEvent(scope, workflow, record, eventMessages, ex, startNotification);
                    }
                }
            }

            record.SetAdditionalData(additionalData);

            if (state == FormState.Deleted)
            {
                DeleteRecord(record, form);
            }

            scope.Complete();
            return result;
        }

        private bool IsWorkflowExcludedByConditions(
      Form form,
      Workflow workflow,
      IDictionary<Guid, string> formFieldValues)
        {
            return workflow.Condition != null && !workflow.Condition.IsVisible(form, this._fieldTypeStorage, formFieldValues, this._placeholderParsingService);
        }

        private static RecordWorkflowAudit CreateRecordAudit(
          Record record,
          Workflow workflow,
          WorkflowType type,
          FormState formState,
          WorkflowExecutionStatus wfes)
        {
            return new RecordWorkflowAudit()
            {
                RecordUniqueId = record.UniqueId,
                ExecutedOn = DateTime.Now,
                ExecutionStage = new int?((int)formState),
                ExecutionStatus = (int)wfes,
                WorkflowKey = workflow.Id,
                WorkflowName = workflow.Name,
                WorkflowTypeId = workflow.WorkflowTypeId,
                WorkflowTypeName = type.Name
            };
        }

        private static void RecordAuditRecord(IScope scope, Form form, RecordWorkflowAudit recordAudit)
        {
            if (!form.StoreRecordsLocally)
                return;
            scope.Database.Insert<RecordWorkflowAudit>(recordAudit);
        }

        private static WorkflowExecutionStartedNotification RaiseWorkflowStartedEvent(
          IScope scope,
          Workflow workflow,
          Record record,
          EventMessages eventMessages)
        {
            WorkflowExecutionStartedNotification startedNotification = new WorkflowExecutionStartedNotification(workflow, record, eventMessages);
            ((ICoreScope)scope).Notifications.Publish(startedNotification);
            return startedNotification;
        }

        private static async Task<WorkflowExecutionStatus> ExecuteWorkflow(
          WorkflowType type,
          Record record,
          Form form,
          FormState state,
          Hashtable pageElements,
          IDictionary<string, string?> additionalData,
          Workflow workflow)
        {
            WorkflowExecutionContext context = new WorkflowExecutionContext(record, form, state, pageElements, additionalData);
            WorkflowExecutionStatus workflowExecutionStatus;
            if (workflow.ExcludeSensitiveData())
            {
                Dictionary<Guid, RecordField> sensitiveRecordFields = WorkflowExecutionService.GetSensitiveRecordFields(record, form);
                WorkflowExecutionService.ExcludeSensitiveFieldsFromRecord(record, form);
                workflowExecutionStatus = await type.ExecuteWorkflowAsync(context, workflow).ConfigureAwait(false);
                WorkflowExecutionService.RestoreSensitiveDataToRecord(record, sensitiveRecordFields);
                sensitiveRecordFields = null;
            }
            else
                workflowExecutionStatus = await type.ExecuteWorkflowAsync(context, workflow).ConfigureAwait(false);
            return workflowExecutionStatus;
        }

        private static Dictionary<Guid, RecordField> GetSensitiveRecordFields(
          Record record,
          Form form)
        {
            IEnumerable<Guid> sensitiveFieldIds = form.AllFields.Where<Field>(p => p.ContainsSensitiveData).Select<Field, Guid>(q => q.Id);
            return record.RecordFields.Where<KeyValuePair<Guid, RecordField>>(r => sensitiveFieldIds.Contains<Guid>(r.Value.FieldId)).ToDictionary<KeyValuePair<Guid, RecordField>, Guid, RecordField>(pair => pair.Key, pair => pair.Value);
        }

        private static void ExcludeSensitiveFieldsFromRecord(Record record, Form form)
        {
            IEnumerable<Guid> sensitiveFieldIds = form.AllFields.Where<Field>(p => p.ContainsSensitiveData).Select<Field, Guid>(q => q.Id);
            IEnumerable<KeyValuePair<Guid, RecordField>> second = record.RecordFields.Where<KeyValuePair<Guid, RecordField>>(r => sensitiveFieldIds.Contains<Guid>(r.Value.FieldId));
            record.RecordFields = record.RecordFields.Except<KeyValuePair<Guid, RecordField>>(second).ToDictionary<KeyValuePair<Guid, RecordField>, Guid, RecordField>(x => x.Key, y => y.Value);
        }

        private static void RestoreSensitiveDataToRecord(
          Record record,
          Dictionary<Guid, RecordField> sensitiveRecordFields)
        {
            foreach (KeyValuePair<Guid, RecordField> sensitiveRecordField in sensitiveRecordFields)
                record.RecordFields.Add(sensitiveRecordField.Key, sensitiveRecordField.Value);
        }

        private static void RaiseWorkflowCompletedEvent(
          IScope scope,
          Workflow workflow,
          Record record,
          EventMessages eventMessages,
          WorkflowExecutionStatus workflowExecutionStatus,
          WorkflowExecutionStartedNotification workflowStartedNotification)
        {
            INotification notification;
            switch (workflowExecutionStatus)
            {
                case WorkflowExecutionStatus.Failed:
                    notification = new WorkflowExecutionFailedNotification(workflow, record, eventMessages).WithStateFrom<WorkflowExecutionFailedNotification, WorkflowExecutionStartedNotification>(workflowStartedNotification);
                    break;
                case WorkflowExecutionStatus.NotConfigured:
                    notification = new WorkflowExecutionNotConfiguredNotification(workflow, record, eventMessages).WithStateFrom<WorkflowExecutionNotConfiguredNotification, WorkflowExecutionStartedNotification>(workflowStartedNotification);
                    break;
                case WorkflowExecutionStatus.Cancelled:
                    notification = new WorkflowExecutionCancelledNotification(workflow, record, eventMessages).WithStateFrom<WorkflowExecutionCancelledNotification, WorkflowExecutionStartedNotification>(workflowStartedNotification);
                    break;
                case WorkflowExecutionStatus.Completed:
                    notification = new WorkflowExecutionCompletedNotification(workflow, record, eventMessages).WithStateFrom<WorkflowExecutionCompletedNotification, WorkflowExecutionStartedNotification>(workflowStartedNotification);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(workflowExecutionStatus));
            }
          ((ICoreScope)scope).Notifications.Publish(notification);
        }

        private static void RaiseWorkflowFailedEvent(
          IScope scope,
          Workflow workflow,
          Record record,
          EventMessages eventMessages,
          Exception exception,
          WorkflowExecutionStartedNotification workflowStartedNotification)
        {
            WorkflowExecutionFailedNotification failedNotification = new WorkflowExecutionFailedNotification(workflow, record, eventMessages)
            {
                Exception = exception
            }.WithStateFrom<WorkflowExecutionFailedNotification, WorkflowExecutionStartedNotification>(workflowStartedNotification);
            ((ICoreScope)scope).Notifications.Publish(failedNotification);
        }

        private void DeleteRecord(Record record, Form form)
        {
            Record record1 = this._recordStorage.GetRecord(record.Id, form);
            if (record1 == null)
                return;
            this._recordStorage.DeleteRecord(record1, form);
        }
    }
}
