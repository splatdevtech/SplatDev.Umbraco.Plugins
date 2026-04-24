using FormBuilder.Core.DataSources;
using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Services.Notifications;
using FormBuilder.Core.Services.Results;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.Extensions.Logging;

using System.Data;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;

namespace FormBuilder.Core.Services
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class RecordService(
      IRecordStorage recordStorage,
      IWorkflowExecutionService workflowService,
      IFieldTypeStorage fieldTypeStorage,
      IDataSourceService dataSourceService,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IProfilingLogger profilingLogger,
      IScopeProvider scopeProvider,
      IEventMessagesFactory eventMessagesFactory,
      DataSourceTypeCollection dataSourceTypeCollection,
      ILogger<RecordService> logger) : IRecordService
    {
        private readonly IRecordStorage _recordStorage = recordStorage;
        private readonly IWorkflowExecutionService _workflowService = workflowService;
        private readonly IFieldTypeStorage _fieldTypeStorage = fieldTypeStorage;
        private readonly IDataSourceService _dataSourceService = dataSourceService;
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        private readonly IProfilingLogger _profilingLogger = profilingLogger;
        private readonly IScopeProvider _scopeProvider = scopeProvider;
        private readonly IEventMessagesFactory _eventMessagesFactory = eventMessagesFactory;
        private readonly DataSourceTypeCollection _dataSourceTypeCollection = dataSourceTypeCollection;
        private readonly ILogger<RecordService> _logger = logger;

        private void StoreRecord(Record record, Form form)
        {
            using (_profilingLogger.DebugDuration<RecordService>(string.Format("Store Record '{0}' into Form '{1}' with id '{2}'", record.UniqueId, form.Name, form.Id)))
            {
                record.RecordFields.RemoveAll(x => x.Value.Field is null || !DoesFieldStoreData(x.Value.Field));
                record.RecordData = record.GenerateRecordDataAsJson();
                if (record.Id <= 0)
                    _recordStorage.InsertRecord(record, form);
                else
                    _recordStorage.UpdateRecord(record, form);
                if (form.DataSource is null)
                    return;
                if (record.State != FormState.Approved)
                    return;
                try
                {
                    FormDataSource? datasource = _dataSourceService.Get(form.DataSource.Id);
                    if (datasource is null)
                        return;
                    using FormDataSourceType? formDataSourceType = datasource.GetFormDataSourceType(_dataSourceTypeCollection);
                    if (formDataSourceType is null)
                        return;
                    formDataSourceType.LoadSettings(datasource);
                    if (!formDataSourceType.SupportsInsert)
                        return;
                    formDataSourceType.InsertRecord(record);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "There was a problem trying to save the record unique id {RecordId} from the Form {FormName} with the id of {FormId} with Datasource id {DataSourceId}", record.UniqueId, form.Name, form.Id, form.DataSource.Id);
                    throw;
                }
            }
        }

        private bool DoesFieldStoreData(Field field)
        {
            FieldType? fieldTypeByField = _fieldTypeStorage.GetFieldTypeByField(field);
            return fieldTypeByField is not null && fieldTypeByField.StoresData;
        }

        public async Task SubmitAsync(Record record, Form? form)
        {
            using (IScope? scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false))
            {
                if (form is null) return;
                DisposableTimer? disposableTimer = _profilingLogger.DebugDuration<RecordService>(string.Format("Submitting Record '{0}' into Form '{1}' with id '{2}'", record.UniqueId, form.Name, form.Id));
                try
                {
                    record.State = FormState.Submitted;
                    record.Created = DateTime.Now;
                    bool editingRecord = record.Id > 0;
                    _logger.LogInformation("Form Entry Submitted - Form Entry Id: {RecordId}, Form Id: {FormId}, User: {Username}", record.UniqueId, form.Id, GetUsernameOfCurrentUser());
                    ((ICoreScope)scope).Notifications.Publish(new RecordSubmittedNotification(record, _eventMessagesFactory.Get(), form));
                    WorkflowExecutionResult? workflowExecutionResult = await _workflowService.ExecuteWorkflowsAsync(record, form, FormState.Submitted, editingRecord).ConfigureAwait(false);
                    if (record.State != FormState.Deleted)
                    {
                        if (form.ManualApproval)
                        {
                            if (form.StoreRecordsLocally)
                                StoreRecord(record, form);
                        }
                        else
                        {
                            if (record.State == FormState.Approved)
                            {
                                if (record.Id > 0)
                                    goto label_15;
                            }
                            await Approve(record, form, editingRecord).ConfigureAwait(false);
                        }
                    }
                    else
                        await DeleteAsync(record, form).ConfigureAwait(false);
                }
                finally
                {
                    disposableTimer?.Dispose();
                }
            label_15:
                using var _ = disposableTimer = null;
                ((ICoreScope)scope).Complete();
            }
        }

        public async Task ApproveAsync(Record record, Form form) => await Approve(record, form, false).ConfigureAwait(false);

        private async Task Approve(Record record, Form form, bool editingRecord)
        {
            using IScope? scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            using (_profilingLogger.DebugDuration<RecordService>(string.Format("Approving Record '{0}' into Form '{1}' with id '{2}'", record.UniqueId, form.Name, form.Id)))
            {
                ConfiguredTaskAwaitable configuredTaskAwaitable = UpdateRecordState(record, form, editingRecord, FormState.Approved).ConfigureAwait(false);
                await configuredTaskAwaitable;
                ((ICoreScope)scope).Notifications.Publish(new RecordApprovedNotification(record, _eventMessagesFactory.Get(), form));
                if (record.State == FormState.Deleted)
                {
                    configuredTaskAwaitable = DeleteAsync(record, form).ConfigureAwait(false);
                    await configuredTaskAwaitable;
                }
            }
              ((ICoreScope)scope).Complete();
        }

        public async Task RejectAsync(Record record, Form form)
        {
            using IScope? scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            using (_profilingLogger.DebugDuration<RecordService>(string.Format("Rejecting Record '{0}' into Form '{1}' with id '{2}'", record.UniqueId, form.Name, form.Id)))
            {
                await UpdateRecordState(record, form, false, FormState.Rejected).ConfigureAwait(false);
                ((ICoreScope)scope).Notifications.Publish(new RecordRejectedNotification(record, _eventMessagesFactory.Get(), form));
            }
              ((ICoreScope)scope).Complete();
        }

        private async Task UpdateRecordState(
          Record record,
          Form form,
          bool editingRecord,
          FormState state)
        {
            record.State = state;
            if (form.StoreRecordsLocally)
                StoreRecord(record, form);
            WorkflowExecutionResult workflowExecutionResult = await _workflowService.ExecuteWorkflowsAsync(record, form, state, editingRecord).ConfigureAwait(false);
            if (form.StoreRecordsLocally && workflowExecutionResult.HasCompletedWorkflows())
                StoreRecord(record, form);
            LogInfo(state, record.UniqueId, form.Id);
        }

        private void LogInfo(FormState state, Guid recordUniqueId, Guid formId)
        {
            string usernameOfCurrentUser = GetUsernameOfCurrentUser();
            string message = string.Empty;
            switch (state)
            {
                case FormState.Approved:
                    message = "Form Entry Approved - Form Entry Id: {RecordId}, Form Id: {FormId}, User: {Username}";
                    break;

                case FormState.Rejected:
                    message = "Form Entry Rejected - Form Entry Id: {RecordId}, Form Id: {FormId}, User: {Username}";
                    break;
            }
            _logger.LogInformation(message, recordUniqueId, formId, usernameOfCurrentUser);
        }

        private string GetUsernameOfCurrentUser()
        {
            IBackOfficeSecurity? backOfficeSecurity = _backOfficeSecurityAccessor.BackOfficeSecurity;
            return backOfficeSecurity?.CurrentUser is null ? string.Empty : backOfficeSecurity.CurrentUser.Username;
        }

        public async Task DeleteAsync(Record record, Form form)
        {
            using IScope? scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            using (_profilingLogger.DebugDuration<RecordService>(string.Format("Record Service: Deleting Record '{0}' into Form '{1}' with id '{2}'", record.UniqueId, form.Name, form.Id)))
            {
                record.State = FormState.Deleted;
                WorkflowExecutionResult? workflowExecutionResult = await _workflowService.ExecuteWorkflowsAsync(record, form, FormState.Deleted, false).ConfigureAwait(false);
                if (record.State == FormState.Deleted && record.Id > 0)
                {
                    _recordStorage.DeleteRecord(record, form);
                    _logger.LogInformation("Form Entry Deleted - Form Entry Id: {RecordId}, Form Id: {FormId}, User: {Username}", record.UniqueId, form.Id, GetUsernameOfCurrentUser());
                    ((ICoreScope)scope).Notifications.Publish(new RecordDeletedNotification(record, _eventMessagesFactory.Get(), form));
                }
              ((ICoreScope)scope).Complete();
            }
        }

        public IReadOnlyList<Record> GetAllRecords(Form form, bool includeFields = true) => _recordStorage.GetAllRecords(form, includeFields);

        public int GetRecordCount(Form form) => _recordStorage.GetRecordCount(form);
    }

#pragma warning restore CS0618 // Type or member is obsolete
}