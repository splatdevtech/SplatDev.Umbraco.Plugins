
// Type: Umbraco.Forms.Core.Services.RecordService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Logging;

using System.Data;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services.Notifications;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Core.Services
{
    internal sealed class RecordService : IRecordService
    {
        private readonly IRecordStorage _recordStorage;
        private readonly IWorkflowExecutionService _workflowService;
        private readonly IFieldTypeStorage _fieldTypeStorage;
        private readonly IDataSourceService _dataSourceService;
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        private readonly IProfilingLogger _profilingLogger;
        private readonly IScopeProvider _scopeProvider;
        private readonly IEventMessagesFactory _eventMessagesFactory;
        private readonly DataSourceTypeCollection _dataSourceTypeCollection;
        private readonly ILogger<RecordService> _logger;

        public RecordService(
          IRecordStorage recordStorage,
          IWorkflowExecutionService workflowService,
          IFieldTypeStorage fieldTypeStorage,
          IDataSourceService dataSourceService,
          IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
          IProfilingLogger profilingLogger,
          IScopeProvider scopeProvider,
          IEventMessagesFactory eventMessagesFactory,
          DataSourceTypeCollection dataSourceTypeCollection,
          ILogger<RecordService> logger)
        {
            this._recordStorage = recordStorage;
            this._workflowService = workflowService;
            this._fieldTypeStorage = fieldTypeStorage;
            this._dataSourceService = dataSourceService;
            this._backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            this._profilingLogger = profilingLogger;
            this._scopeProvider = scopeProvider;
            this._eventMessagesFactory = eventMessagesFactory;
            this._dataSourceTypeCollection = dataSourceTypeCollection;
            this._logger = logger;
        }

        private void StoreRecord(Record record, Form form)
        {
            using (this._profilingLogger.DebugDuration<RecordService>(string.Format("Store Record '{0}' into Form '{1}' with id '{2}'", record.UniqueId, form.Name, form.Id)))
            {
                record.RecordFields.RemoveAll<KeyValuePair<Guid, RecordField>>(x => x.Value.Field == null || !this.DoesFieldStoreData(x.Value.Field));
                record.RecordData = record.GenerateRecordDataAsJson();
                if (record.Id <= 0)
                    this._recordStorage.InsertRecord(record, form);
                else
                    this._recordStorage.UpdateRecord(record, form);
                if (form.DataSource == null)
                    return;
                if (record.State != FormState.Approved)
                    return;
                try
                {
                    FormDataSource datasource = this._dataSourceService.Get(form.DataSource.Id);
                    if (datasource == null)
                        return;
                    using (FormDataSourceType formDataSourceType = datasource.GetFormDataSourceType(this._dataSourceTypeCollection))
                    {
                        if (formDataSourceType == null)
                            return;
                        formDataSourceType.LoadSettings(datasource);
                        if (!formDataSourceType.SupportsInsert)
                            return;
                        formDataSourceType.InsertRecord(record);
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "There was a problem trying to save the record unique id {RecordId} from the Form {FormName} with the id of {FormId} with Datasource id {DataSourceId}", record.UniqueId, form.Name, form.Id, form.DataSource.Id);
                    throw;
                }
            }
        }

        private bool DoesFieldStoreData(Field field)
        {
            FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(field);
            return fieldTypeByField != null && fieldTypeByField.StoresData;
        }

        public async Task SubmitAsync(Record record, Form form)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                DisposableTimer disposableTimer = this._profilingLogger.DebugDuration<RecordService>(string.Format("Submitting Record '{0}' into Form '{1}' with id '{2}'", record.UniqueId, form.Name, form.Id));
                try
                {
                    record.State = FormState.Submitted;
                    record.Created = DateTime.Now;
                    bool editingRecord = record.Id > 0;
                    this._logger.LogInformation("Form Entry Submitted - Form Entry Id: {RecordId}, Form Id: {FormId}, User: {Username}", record.UniqueId, form.Id, this.GetUsernameOfCurrentUser());
                    scope.Notifications.Publish(new RecordSubmittedNotification(record, this._eventMessagesFactory.Get(), form));
                    WorkflowExecutionResult workflowExecutionResult = await this._workflowService.ExecuteWorkflowsAsync(record, form, FormState.Submitted, editingRecord).ConfigureAwait(false);
                    if (record.State != FormState.Deleted)
                    {
                        if (form.ManualApproval)
                        {
                            if (form.StoreRecordsLocally)
                                this.StoreRecord(record, form);
                        }
                        else
                        {
                            if (record.State == FormState.Approved)
                            {
                                if (record.Id > 0)
                                    goto label_15;
                            }
                            await this.Approve(record, form, editingRecord).ConfigureAwait(false);
                        }
                    }
                    else
                        await this.DeleteAsync(record, form).ConfigureAwait(false);
                }
                finally
                {
                    disposableTimer?.Dispose();
                }
            label_15:
                disposableTimer = null;
                scope.Complete();
            }
        }

        public async Task ApproveAsync(Record record, Form form) => await this.Approve(record, form, false).ConfigureAwait(false);

        private async Task Approve(Record record, Form form, bool editingRecord)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                using (this._profilingLogger.DebugDuration<RecordService>(string.Format("Approving Record '{0}' into Form '{1}' with id '{2}'", record.UniqueId, form.Name, form.Id)))
                {
                    ConfiguredTaskAwaitable configuredTaskAwaitable = this.UpdateRecordState(record, form, editingRecord, FormState.Approved).ConfigureAwait(false);
                    await configuredTaskAwaitable;
                    scope.Notifications.Publish(new RecordApprovedNotification(record, this._eventMessagesFactory.Get(), form));
                    if (record.State == FormState.Deleted)
                    {
                        configuredTaskAwaitable = this.DeleteAsync(record, form).ConfigureAwait(false);
                        await configuredTaskAwaitable;
                    }
                }
                scope.Complete();
            }
        }

        public async Task RejectAsync(Record record, Form form)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                using (this._profilingLogger.DebugDuration<RecordService>(string.Format("Rejecting Record '{0}' into Form '{1}' with id '{2}'", record.UniqueId, form.Name, form.Id)))
                {
                    await this.UpdateRecordState(record, form, false, FormState.Rejected).ConfigureAwait(false);
                    scope.Notifications.Publish(new RecordRejectedNotification(record, this._eventMessagesFactory.Get(), form));
                }
                scope.Complete();
            }
        }

        private async Task UpdateRecordState(
          Record record,
          Form form,
          bool editingRecord,
          FormState state)
        {
            record.State = state;
            if (form.StoreRecordsLocally)
                this.StoreRecord(record, form);
            WorkflowExecutionResult workflowExecutionResult = await this._workflowService.ExecuteWorkflowsAsync(record, form, state, editingRecord).ConfigureAwait(false);
            if (form.StoreRecordsLocally && workflowExecutionResult.HasCompletedWorkflows())
                this.StoreRecord(record, form);
            this.LogInfo(state, record.UniqueId, form.Id);
        }

        private void LogInfo(FormState state, Guid recordUniqueId, Guid formId)
        {
            string usernameOfCurrentUser = this.GetUsernameOfCurrentUser();
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
            this._logger.LogInformation(message, recordUniqueId, formId, usernameOfCurrentUser);
        }

        private string GetUsernameOfCurrentUser()
        {
            IBackOfficeSecurity backOfficeSecurity = this._backOfficeSecurityAccessor.BackOfficeSecurity;
            return backOfficeSecurity?.CurrentUser == null ? string.Empty : backOfficeSecurity.CurrentUser.Username;
        }

        public async Task DeleteAsync(Record record, Form form)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                using (this._profilingLogger.DebugDuration<RecordService>(string.Format("Record Service: Deleting Record '{0}' into Form '{1}' with id '{2}'", record.UniqueId, form.Name, form.Id)))
                {
                    record.State = FormState.Deleted;
                    WorkflowExecutionResult workflowExecutionResult = await this._workflowService.ExecuteWorkflowsAsync(record, form, FormState.Deleted, false).ConfigureAwait(false);
                    if (record.State == FormState.Deleted && record.Id > 0)
                    {
                        this._recordStorage.DeleteRecord(record, form);
                        this._logger.LogInformation("Form Entry Deleted - Form Entry Id: {RecordId}, Form Id: {FormId}, User: {Username}", record.UniqueId, form.Id, this.GetUsernameOfCurrentUser());
                        scope.Notifications.Publish(new RecordDeletedNotification(record, this._eventMessagesFactory.Get(), form));
                    }
                    scope.Complete();
                }
            }
        }

        public IReadOnlyList<Record> GetAllRecords(Form form, bool includeFields = true) => this._recordStorage.GetAllRecords(form, includeFields);

        public int GetRecordCount(Form form) => this._recordStorage.GetRecordCount(form);
    }
}
