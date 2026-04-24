using FormBuilder.Core.Configuration;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Evaluators;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Providers.Factories;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Services.Notifications;
using FormBuilder.Core.Services.Results;
using FormBuilder.Core.Storage.Interfaces;
using FormBuilder.Core.Workflows;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Collections;
using System.Data;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;

namespace FormBuilder.Core.Services
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class WorkflowExecutionService(
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
      IHttpContextAccessor httpContextAccessor) : IWorkflowExecutionService
    {
        private readonly IRecordStorage _recordStorage = recordStorage;
        private readonly IWorkflowService _workflowService = workflowService;
        private readonly IPageService _pageService = pageService;
        private readonly IScopeProvider _scopeProvider = scopeProvider;
        private readonly WorkflowCollectionFactory _workflowCollectionFactory = workflowCollectionFactory;
        private readonly IPlaceholderParsingService _placeholderParsingService = placeholderParsingService;
        private readonly IEventMessagesFactory _eventMessagesFactory = eventMessagesFactory;
        private readonly IProfilingLogger _profilingLogger = profilingLogger;
        private readonly ILogger<WorkflowExecutionService> _logger = logger;
        private readonly PackageOptionSettings _config = config.Value;
        private readonly IFieldTypeStorage _fieldTypeStorage = fieldTypeStorage;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<WorkflowExecutionResult> ExecuteWorkflowsAsync(
          Record record,
          Form form,
          FormState state,
          bool editMode)
        {
            bool flag = false;
            if (editMode)
                flag = ShouldWorkflowsBeIgnored(form);
            if (flag)
                return WorkflowExecutionResult.Create();
            return await ExecuteWorkflowsAsync(_workflowService.GetActiveWorkFlows(form, state), record, form, state).ConfigureAwait(false);
        }

        private bool ShouldWorkflowsBeIgnored(Form form)
        {
            string lower = _config.IgnoreWorkFlowsOnEdit.ToLower();
            if (string.IsNullOrEmpty(lower) || lower == "false")
                return false;
            return lower == "true" || lower.Split(',').Contains(form.Name.ToLower());
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
                new bool?(),
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
            return workflow.Condition is not null && !workflow.Condition.IsVisible(form, _fieldTypeStorage, formFieldValues, _placeholderParsingService);
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
            scope.Database.Insert(recordAudit);
        }

        private static WorkflowExecutionStartedNotification RaiseWorkflowStartedEvent(
          IScope scope,
          Workflow workflow,
          Record record,
          EventMessages eventMessages)
        {
            WorkflowExecutionStartedNotification startedNotification = new(workflow, record, eventMessages);
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
            WorkflowExecutionContext context = new(record, form, state, pageElements, additionalData);
            WorkflowExecutionStatus workflowExecutionStatus;
            if (workflow.ExcludeSensitiveData())
            {
                Dictionary<Guid, RecordField>? sensitiveRecordFields = GetSensitiveRecordFields(record, form);
                ExcludeSensitiveFieldsFromRecord(record, form);
                workflowExecutionStatus = await type.ExecuteWorkflowAsync(context, workflow).ConfigureAwait(false);
                RestoreSensitiveDataToRecord(record, sensitiveRecordFields);
            }
            else
                workflowExecutionStatus = await type.ExecuteWorkflowAsync(context, workflow).ConfigureAwait(false);
            return workflowExecutionStatus;
        }

        private static Dictionary<Guid, RecordField> GetSensitiveRecordFields(
          Record record,
          Form form)
        {
            IEnumerable<Guid> sensitiveFieldIds = form.AllFields.Where(p => p.ContainsSensitiveData).Select(q => q.Id);
            return record.RecordFields.Where(r => sensitiveFieldIds.Contains(r.Value.FieldId)).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private static void ExcludeSensitiveFieldsFromRecord(Record record, Form form)
        {
            IEnumerable<Guid> sensitiveFieldIds = form.AllFields.Where(p => p.ContainsSensitiveData).Select(q => q.Id);
            IEnumerable<KeyValuePair<Guid, RecordField>> second = record.RecordFields.Where(r => sensitiveFieldIds.Contains(r.Value.FieldId));
            record.RecordFields = record.RecordFields.Except(second).ToDictionary(x => x.Key, y => y.Value);
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
            INotification notification = workflowExecutionStatus switch
            {
                WorkflowExecutionStatus.Failed => new WorkflowExecutionFailedNotification(workflow, record, eventMessages).WithStateFrom(workflowStartedNotification),
                WorkflowExecutionStatus.NotConfigured => new WorkflowExecutionNotConfiguredNotification(workflow, record, eventMessages).WithStateFrom(workflowStartedNotification),
                WorkflowExecutionStatus.Cancelled => new WorkflowExecutionCancelledNotification(workflow, record, eventMessages).WithStateFrom(workflowStartedNotification),
                WorkflowExecutionStatus.Completed => new WorkflowExecutionCompletedNotification(workflow, record, eventMessages).WithStateFrom(workflowStartedNotification),
                _ => throw new ArgumentOutOfRangeException(nameof(workflowExecutionStatus)),
            };
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
            }.WithStateFrom(workflowStartedNotification);
            ((ICoreScope)scope).Notifications.Publish(failedNotification);
        }

        private void DeleteRecord(Record record, Form form)
        {
            Record? record1 = _recordStorage.GetRecord(record.Id, form);
            if (record1 is null)
                return;
            _recordStorage.DeleteRecord(record1, form);
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}