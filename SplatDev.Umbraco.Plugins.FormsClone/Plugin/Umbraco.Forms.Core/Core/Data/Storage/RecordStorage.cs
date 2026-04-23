
// Type: Umbraco.Forms.Core.Data.Storage.RecordStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using System.Data;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Core.Services.Notifications;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;


#nullable enable
namespace Umbraco.Forms.Core.Data.Storage
{
    internal sealed class RecordStorage : IRecordStorage
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IRecordFieldStorage _recordFieldStorage;
        private readonly IRecordFieldValueStorage _fieldValueStorage;
        private readonly IFieldTypeStorage _fieldTypeStorage;
        private readonly IProfilingLogger _profilingLogger;
        private readonly MediaFileManager _mediaFileManager;
        private readonly IEventMessagesFactory _eventMessagesFactory;

        public RecordStorage(
          IScopeProvider scopeProvider,
          IRecordFieldStorage recordFieldStorage,
          IRecordFieldValueStorage recordFieldValueStorage,
          IFieldTypeStorage fieldTypeStorage,
          IProfilingLogger profilingLogger,
          MediaFileManager mediaFileManager,
          IEventMessagesFactory eventMessagesFactory)
        {
            this._scopeProvider = scopeProvider;
            this._recordFieldStorage = recordFieldStorage;
            this._fieldValueStorage = recordFieldValueStorage;
            this._fieldTypeStorage = fieldTypeStorage;
            this._profilingLogger = profilingLogger;
            this._mediaFileManager = mediaFileManager;
            this._eventMessagesFactory = eventMessagesFactory;
        }

        public List<Record> GetAllRecords(Form form, bool includeFields = true)
        {
            IProfilingLogger profilingLogger = this._profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 2);
            interpolatedStringHandler.AppendLiteral("Get all Records for Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted<Guid>(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                {
                    Sql<ISqlContext> sql = NPocoSqlExtensions.OrderByDescending<Record>(NPocoSqlExtensions.Where<Record>(scope.SqlContext.Sql(), x => x.Form == form.Id, null), x => x.Created);
                    List<Record> allRecords = scope.Database.Fetch<Record>(sql);
                    if (!includeFields)
                    {
                        scope.Complete();
                        return allRecords;
                    }
                    Dictionary<Guid, RecordField> recordFields = this._recordFieldStorage.GetAllRecordFields(allRecords, form);
                    allRecords.ForEach(record =>
                    {
                        record.RecordFields = recordFields.Where<KeyValuePair<Guid, RecordField>>(p => p.Value.Record == record.Id).ToDictionary<KeyValuePair<Guid, RecordField>, Guid, RecordField>(x => x.Key, x => x.Value);
                        this.PublishRecordFetchingNotification(scope, record);
                    });
                    scope.Complete();
                    return allRecords;
                }
            }
        }

        public List<Record> GetRecords(IEnumerable<Guid> keys, Form form, bool includeFields = true)
        {
            IProfilingLogger profilingLogger = this._profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 2);
            interpolatedStringHandler.AppendLiteral("Get specific Records for Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted<Guid>(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                {
                    Sql<ISqlContext> sql = scope.SqlContext.Sql().Where("uniqueid IN (@values)", new
                    {
                        values = keys
                    });
                    if (includeFields)
                        sql.Append(" AND form = @formId", new
                        {
                            formId = form.Id
                        });
                    NPocoSqlExtensions.OrderByDescending<Record>(sql, x => x.Created);
                    List<Record> records = scope.Database.Fetch<Record>(sql);
                    if (!includeFields)
                    {
                        scope.Complete();
                        return records;
                    }
                    Dictionary<Guid, RecordField> recordFields = this._recordFieldStorage.GetAllRecordFields(records, form);
                    records.ForEach(record =>
                    {
                        record.RecordFields = recordFields.Where<KeyValuePair<Guid, RecordField>>(p => p.Value.Record == record.Id).ToDictionary<KeyValuePair<Guid, RecordField>, Guid, RecordField>(x => x.Key, x => x.Value);
                        this.PublishRecordFetchingNotification(scope, record);
                    });
                    scope.Complete();
                    return records;
                }
            }
        }

        public Record? GetRecord(int id, Form form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));
            IProfilingLogger profilingLogger = this._profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
            interpolatedStringHandler.AppendLiteral("Get Record '");
            interpolatedStringHandler.AppendFormatted<int>(id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                {
                    Sql<ISqlContext> sql = NPocoSqlExtensions.Where<Record>(NPocoSqlExtensions.From<Record>(NPocoSqlExtensions.Select<Record>(scope.SqlContext.Sql(), Array.Empty<Expression<Func<Record, object>>>()), null), x => x.Id == id, null);
                    Record record = scope.Database.SingleOrDefault<Record>(sql);
                    if (record != null)
                    {
                        record.RecordFields = this._recordFieldStorage.GetAllRecordFields(record, form);
                        this.PublishRecordFetchingNotification(scope, record);
                    }
                    scope.Complete();
                    return record;
                }
            }
        }

        private void PublishRecordFetchingNotification(IScope scope, Record record)
        {
            EventMessages messages = this._eventMessagesFactory.Get();
            RecordFetchingNotification fetchingNotification = new RecordFetchingNotification(record, messages);
            scope.Notifications.Publish(fetchingNotification);
        }

        public Record? GetRecordByUniqueId(Guid uniqueId, Form form)
        {
            IProfilingLogger profilingLogger = this._profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
            interpolatedStringHandler.AppendLiteral("Get Record by unique id '");
            interpolatedStringHandler.AppendFormatted<Guid>(uniqueId);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                {
                    Sql<ISqlContext> sql = scope.SqlContext.Sql();
                    NPocoSqlExtensions.Where<Record>(NPocoSqlExtensions.From<Record>(NPocoSqlExtensions.Select<Record>(sql, Array.Empty<Expression<Func<Record, object>>>()), null), x => x.UniqueId == uniqueId, null);
                    NPocoSqlExtensions.Where<Record>(sql, x => x.Form == form.Id, null);
                    Record record = scope.Database.SingleOrDefault<Record>(sql);
                    if (record != null)
                    {
                        record.RecordFields = this._recordFieldStorage.GetAllRecordFields(record, form);
                        this.PublishRecordFetchingNotification(scope, record);
                    }
                    scope.Complete();
                    return record;
                }
            }
        }

        public Record InsertRecord(Record record, Form form)
        {
            IProfilingLogger profilingLogger = this._profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 3);
            interpolatedStringHandler.AppendLiteral("Insert Record '");
            interpolatedStringHandler.AppendFormatted<Guid>(record.UniqueId);
            interpolatedStringHandler.AppendLiteral("' into Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted<Guid>(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                {
                    record.Created = DateTime.Now;
                    record.Updated = DateTime.Now;
                    EventMessages messages = this._eventMessagesFactory.Get();
                    RecordCreatingNotification notification1 = new RecordCreatingNotification(record, messages);
                    if (scope.Notifications.PublishCancelable(notification1))
                    {
                        scope.Complete();
                        return record;
                    }
                    RecordSavingNotification notification2 = new RecordSavingNotification(record, messages);
                    if (scope.Notifications.PublishCancelable(notification2))
                    {
                        scope.Complete();
                        return record;
                    }
                    try
                    {
                        var result = scope.Database.Insert<Record>(record);
                        var records = scope.Database.Fetch<Record>();
                        scope.Database.Save<Record>(record);
                    }
                    catch (Exception ex)
                    {
                        _ = ex;
                        throw;
                    }
                    Dictionary<Guid, RecordField> dictionary = new Dictionary<Guid, RecordField>();
                    this._fieldValueStorage.DeleteAllRecordFieldValues(record);
                    foreach (KeyValuePair<Guid, RecordField> recordField in record.RecordFields)
                    {
                        if (recordField.Value.Field == null)
                            throw new InvalidOperationException("Field is not available on provided record.");
                        FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(recordField.Value.Field);
                        if (fieldTypeByField == null || fieldTypeByField.StoresData)
                        {
                            recordField.Value.Record = record.Id;
                            dictionary.Add(recordField.Key, recordField.Value);
                        }
                    }
                    this._recordFieldStorage.InsertRecordFields(record.RecordFields.Select<KeyValuePair<Guid, RecordField>, RecordField>(rf => rf.Value));
                    record.RecordFields = dictionary;
                    scope.Complete();
                    return record;
                }
            }
        }

        public void DeleteFormRecords(Form form)
        {
            IProfilingLogger profilingLogger = this._profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 2);
            interpolatedStringHandler.AppendLiteral("Delete all Records for the Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted<Guid>(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                {
                    this.DeleteRecords(scope.Database.Fetch<Record>("WHERE form=@formId", new
                    {
                        formId = form.Id
                    }), form);
                    scope.Complete();
                }
            }
        }

        public int DeleteFormRecords(
          Form form,
          FormState formState,
          DateTime deleteRecordsCreatedEarlierThan)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                List<Record> records = scope.Database.Fetch<Record>("WHERE Form = @0 AND State = @1 AND Created < @2", form.Id, formState.ToString(), deleteRecordsCreatedEarlierThan);
                Dictionary<Guid, RecordField> recordFields = this._recordFieldStorage.GetAllRecordFields(records, form);
                records.ForEach(record => record.RecordFields = recordFields.Where<KeyValuePair<Guid, RecordField>>(p => p.Value.Record == record.Id).ToDictionary<KeyValuePair<Guid, RecordField>, Guid, RecordField>(x => x.Key, x => x.Value));
                scope.Complete();
                return this.DeleteRecords(records, form) ? records.Count : 0;
            }
        }

        public void DeleteRecord(Record record, Form form) => this.DeleteRecords(new Record[1]
        {
      record
        }, form);

        private bool DeleteRecords(IList<Record> records, Form form)
        {
            if (records.Count == 0)
                return false;
            IProfilingLogger profilingLogger = this._profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 3);
            interpolatedStringHandler.AppendLiteral("Delete ");
            interpolatedStringHandler.AppendFormatted<int>(records.Count);
            interpolatedStringHandler.AppendLiteral(" records for the Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted<Guid>(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                {
                    EventMessages messages = this._eventMessagesFactory.Get();
                    RecordDeletingNotification notification = new RecordDeletingNotification(records, messages);
                    if (scope.Notifications.PublishCancelable(notification))
                    {
                        scope.Complete();
                        return false;
                    }
                    foreach (Field field in form.AllFields.Where<Field>(x => x.FieldTypeId == Guid.Parse("84A17CF8-B711-46A6-9840-0E4A072AD000")))
                    {
                        foreach (Record record in (IEnumerable<Record>)records)
                        {
                            RecordField recordField = record.GetRecordField(field.Id);
                            if (recordField != null)
                            {
                                foreach (object obj in recordField.Values)
                                {
                                    string fileName = this._mediaFileManager.FileSystem.GetFileName(obj.ToString());
                                    string path = obj.ToString().Replace(fileName, string.Empty);
                                    if (this._mediaFileManager.FileSystem.FileExists(obj.ToString()))
                                    {
                                        this._mediaFileManager.FileSystem.DeleteFile(obj.ToString());
                                        if (!string.IsNullOrEmpty(path) && this._mediaFileManager.FileSystem.DirectoryExists(path))
                                            this._mediaFileManager.FileSystem.DeleteDirectory(path, true);
                                    }
                                }
                            }
                        }
                    }
                    int[] array = records.Select<Record, int>(x => x.Id).ToArray<int>();
                    if (array.Any<int>())
                    {
                        foreach (IList<int> batch in this.GetBatches(2000, array))
                        {
                            this._fieldValueStorage.DeleteAllRecordFieldValues(batch);
                            this._fieldValueStorage.DeleteAllRecordAuditValues(batch);
                            Sql<ISqlContext> sql = NPocoSqlExtensions.WhereIn<Record>(NPocoSqlExtensions.Delete<Record>(scope.Database.SqlContext.Sql()), r => r.Id, batch);
                            scope.Database.Execute(sql);
                        }
                    }
                    scope.Complete();
                    return true;
                }
            }
        }

        private IEnumerable<IList<int>> GetBatches(
          int batchSize,
          IList<int> allRecordIds)
        {
            int count = allRecordIds.Count;
            int skip = 0;
            do
            {
                int[] array = allRecordIds.Skip<int>(skip).Take<int>(batchSize).ToArray<int>();
                skip += batchSize;
                yield return array;
            }
            while (skip <= count);
        }

        public Record UpdateRecord(Record record, Form form) => this.UpdateRecord(record, form, null);

        public Record UpdateRecord(Record record, Form form, int? userId)
        {
            IProfilingLogger profilingLogger = this._profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 3);
            interpolatedStringHandler.AppendLiteral("Update Record '");
            interpolatedStringHandler.AppendFormatted<Guid>(record.UniqueId);
            interpolatedStringHandler.AppendLiteral("' for the Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted<Guid>(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                {
                    record.Updated = DateTime.Now;
                    foreach (KeyValuePair<Guid, RecordField> recordField in record.RecordFields)
                        recordField.Value.Record = record.Id;
                    EventMessages messages = this._eventMessagesFactory.Get();
                    RecordSavingNotification notification = new RecordSavingNotification(record, messages);
                    if (scope.Notifications.PublishCancelable(notification))
                    {
                        scope.Complete();
                        return record;
                    }
                    record.RecordData = record.GenerateRecordDataAsJson();
                    scope.Database.Update(record);
                    Dictionary<Guid, RecordField> dictionary = new Dictionary<Guid, RecordField>();
                    this._fieldValueStorage.DeleteAllRecordFieldValues(record);
                    List<RecordField> source1 = new List<RecordField>();
                    List<RecordField> source2 = new List<RecordField>();
                    foreach (KeyValuePair<Guid, RecordField> recordField in record.RecordFields)
                    {
                        if (recordField.Value.Field != null)
                        {
                            FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(recordField.Value.Field);
                            if (fieldTypeByField == null || fieldTypeByField.StoresData)
                            {
                                dictionary.Add(recordField.Key, recordField.Value);
                                if (recordField.Value.Key == Guid.Empty)
                                    source1.Add(recordField.Value);
                                else
                                    source2.Add(recordField.Value);
                            }
                        }
                    }
                    if (source1.Any<RecordField>())
                        this._recordFieldStorage.InsertRecordFields(record.RecordFields.Select<KeyValuePair<Guid, RecordField>, RecordField>(rf => rf.Value));
                    if (source2.Any<RecordField>())
                        this._recordFieldStorage.UpdateRecordFields(record.RecordFields.Select<KeyValuePair<Guid, RecordField>, RecordField>(rf => rf.Value));
                    record.RecordFields = dictionary;
                    RecordAudit poco = new RecordAudit()
                    {
                        Record = record.Id,
                        UpdatedOn = record.Updated,
                        UpdatedBy = userId
                    };
                    scope.Database.Insert<RecordAudit>(poco);
                    scope.Complete();
                    return record;
                }
            }
        }

        public int GetRecordCount(Form form)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                Sql<ISqlContext> sql = NPocoSqlExtensions.Where<Record>(NPocoSqlExtensions.From<Record>(NPocoSqlExtensions.SelectCount(scope.Database.SqlContext.Sql(), null), null), x => x.Form == form.Id, null);
                int recordCount = scope.Database.ExecuteScalar<int>(sql);
                scope.Complete();
                return recordCount;
            }
        }
    }
}
