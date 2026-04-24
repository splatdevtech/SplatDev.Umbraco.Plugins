using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Services.Notifications;
using FormBuilder.Core.Storage.Interfaces;

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

namespace FormBuilder.Core.Storage
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class RecordStorage(
      IScopeProvider scopeProvider,
      IRecordFieldStorage recordFieldStorage,
      IRecordFieldValueStorage recordFieldValueStorage,
      IFieldTypeStorage fieldTypeStorage,
      IProfilingLogger profilingLogger,
      MediaFileManager mediaFileManager,
      IEventMessagesFactory eventMessagesFactory) : IRecordStorage
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;
        private readonly IRecordFieldStorage _recordFieldStorage = recordFieldStorage;
        private readonly IRecordFieldValueStorage _fieldValueStorage = recordFieldValueStorage;
        private readonly IFieldTypeStorage _fieldTypeStorage = fieldTypeStorage;
        private readonly IProfilingLogger _profilingLogger = profilingLogger;
        private readonly MediaFileManager _mediaFileManager = mediaFileManager;
        private readonly IEventMessagesFactory _eventMessagesFactory = eventMessagesFactory;

        public List<Record> GetAllRecords(Form form, bool includeFields = true)
        {
            IProfilingLogger profilingLogger = _profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(38, 2);
            interpolatedStringHandler.AppendLiteral("Get all Records for Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
                Sql<ISqlContext> sql = scope.SqlContext.Sql().Where<Record>(x => x.Form == form.Id, null).OrderByDescending<Record>(x => x.Created);
                List<Record> allRecords = scope.Database.Fetch<Record>(sql);
                if (!includeFields)
                {
                    ((ICoreScope)scope).Complete();
                    return allRecords;
                }
                Dictionary<Guid, RecordField> recordFields = _recordFieldStorage.GetAllRecordFields(allRecords, form);
                allRecords.ForEach(record =>
                {
                    record.RecordFields = recordFields.Where(p => p.Value.Record == record.Id).ToDictionary(x => x.Key, x => x.Value);
                    PublishRecordFetchingNotification(scope, record);
                });
                ((ICoreScope)scope).Complete();
                return allRecords;
            }
        }

        public List<Record> GetRecords(IEnumerable<Guid> keys, Form form, bool includeFields = true)
        {
            IProfilingLogger profilingLogger = _profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(43, 2);
            interpolatedStringHandler.AppendLiteral("Get specific Records for Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
                Sql<ISqlContext> sql = scope.SqlContext.Sql().Where("uniqueid IN (@values)", new
                {
                    values = keys
                });
                if (includeFields)
                    sql.Append(" AND form = @formId", new
                    {
                        formId = form.Id
                    });
                sql.OrderByDescending<Record>(x => x.Created);
                List<Record> records = scope.Database.Fetch<Record>(sql);
                if (!includeFields)
                {
                    ((ICoreScope)scope).Complete();
                    return records;
                }
                Dictionary<Guid, RecordField> recordFields = _recordFieldStorage.GetAllRecordFields(records, form);
                records.ForEach(record =>
                {
                    record.RecordFields = recordFields.Where(p => p.Value.Record == record.Id).ToDictionary(x => x.Key, x => x.Value);
                    PublishRecordFetchingNotification(scope, record);
                });
                ((ICoreScope)scope).Complete();
                return records;
            }
        }

        public Record? GetRecord(int id, Form form)
        {
            ArgumentNullException.ThrowIfNull(form);
            IProfilingLogger profilingLogger = _profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(13, 1);
            interpolatedStringHandler.AppendLiteral("Get Record '");
            interpolatedStringHandler.AppendFormatted(id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
                Sql<ISqlContext> sql = NPocoSqlExtensions.Select(scope.SqlContext.Sql(), Array.Empty<Expression<Func<Record, object?>>>()).From<Record>(null).Where<Record>(x => x.Id == id, null);
                Record record = scope.Database.SingleOrDefault<Record>(sql);
                if (record is not null)
                {
                    record.RecordFields = _recordFieldStorage.GetAllRecordFields(record, form);
                    PublishRecordFetchingNotification(scope, record);
                }
                  ((ICoreScope)scope).Complete();
                return record;
            }
        }

        private void PublishRecordFetchingNotification(IScope scope, Record record)
        {
            EventMessages messages = _eventMessagesFactory.Get();
            RecordFetchingNotification fetchingNotification = new(record, messages);
            ((ICoreScope)scope).Notifications.Publish(fetchingNotification);
        }

        public Record? GetRecordByUniqueId(Guid uniqueId, Form form)
        {
            IProfilingLogger profilingLogger = _profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(26, 1);
            interpolatedStringHandler.AppendLiteral("Get Record by unique id '");
            interpolatedStringHandler.AppendFormatted(uniqueId);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
                Sql<ISqlContext> sql = scope.SqlContext.Sql();
                NPocoSqlExtensions.Select(sql, Array.Empty<Expression<Func<Record, object?>>>()).From<Record>(null).Where<Record>(x => x.UniqueId == uniqueId, null);
                sql.Where<Record>(x => x.Form == form.Id, null);
                Record record = scope.Database.SingleOrDefault<Record>(sql);
                if (record is not null)
                {
                    record.RecordFields = _recordFieldStorage.GetAllRecordFields(record, form);
                    PublishRecordFetchingNotification(scope, record);
                }
                  ((ICoreScope)scope).Complete();
                return record;
            }
        }

        public Record InsertRecord(Record record, Form form)
        {
            IProfilingLogger profilingLogger = _profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(40, 3);
            interpolatedStringHandler.AppendLiteral("Insert Record '");
            interpolatedStringHandler.AppendFormatted(record.UniqueId);
            interpolatedStringHandler.AppendLiteral("' into Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
                record.Created = DateTime.Now;
                record.Updated = DateTime.Now;
                EventMessages messages = _eventMessagesFactory.Get();
                var notification1 = new RecordCreatingNotification(record, messages);
                if (((ICoreScope)scope).Notifications.PublishCancelable(notification1))
                {
                    ((ICoreScope)scope).Complete();
                    return record;
                }
                RecordSavingNotification notification2 = new(record, messages);
                if (((ICoreScope)scope).Notifications.PublishCancelable(notification2))
                {
                    ((ICoreScope)scope).Complete();
                    return record;
                }
                scope.Database.Insert(record);
                Dictionary<Guid, RecordField> dictionary = [];
                _fieldValueStorage.DeleteAllRecordFieldValues(record);
                foreach (KeyValuePair<Guid, RecordField> recordField in record.RecordFields)
                {
                    if (recordField.Value.Field is null)
                        throw new InvalidOperationException("Field is not available on provided record.");
                    FieldType? fieldTypeByField = _fieldTypeStorage.GetFieldTypeByField(recordField.Value.Field);
                    if (fieldTypeByField is null || fieldTypeByField.StoresData)
                    {
                        recordField.Value.Record = record.Id;
                        dictionary.Add(recordField.Key, recordField.Value);
                    }
                }
                _recordFieldStorage.InsertRecordFields(record.RecordFields.Select(rf => rf.Value));
                record.RecordFields = dictionary;
                ((ICoreScope)scope).Complete();
                return record;
            }
        }

        public void DeleteFormRecords(Form form)
        {
            IProfilingLogger profilingLogger = _profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(45, 2);
            interpolatedStringHandler.AppendLiteral("Delete all Records for the Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
                DeleteRecords(scope.Database.Fetch<Record>("WHERE form=@formId", new
                {
                    formId = form.Id
                }), form);
                ((ICoreScope)scope).Complete();
            }
        }

        public int DeleteFormRecords(
          Form form,
          FormState formState,
          DateTime deleteRecordsCreatedEarlierThan)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            List<Record> records = scope.Database.Fetch<Record>("WHERE Form = @0 AND State = @1 AND Created < @2", form.Id, formState.ToString(), deleteRecordsCreatedEarlierThan);
            Dictionary<Guid, RecordField> recordFields = _recordFieldStorage.GetAllRecordFields(records, form);
            records.ForEach(record => record.RecordFields = recordFields.Where(p => p.Value.Record == record.Id).ToDictionary(x => x.Key, x => x.Value));
            ((ICoreScope)scope).Complete();
            return DeleteRecords(records, form) ? records.Count : 0;
        }

        public void DeleteRecord(Record record, Form form) => DeleteRecords(
        [
            record
        ], form);

        private bool DeleteRecords(List<Record> records, Form form)
        {
            ArgumentNullException.ThrowIfNull(records);

            if (records.Count == 0)
                return false;
            IProfilingLogger profilingLogger = _profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(42, 3);
            interpolatedStringHandler.AppendLiteral("Delete ");
            interpolatedStringHandler.AppendFormatted(records.Count);
            interpolatedStringHandler.AppendLiteral(" records for the Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
                EventMessages messages = _eventMessagesFactory.Get();
                RecordDeletingNotification notification = new(records, messages);
                if (((ICoreScope)scope).Notifications.PublishCancelable(notification))
                {
                    ((ICoreScope)scope).Complete();
                    return false;
                }
                foreach (Field field in form.AllFields.Where(x => x.FieldTypeId == Guid.Parse("84A17CF8-B711-46A6-9840-0E4A072AD000")))
                {
                    foreach (Record record in (IEnumerable<Record>)records)
                    {
                        RecordField? recordField = record.GetRecordField(field.Id);
                        if (recordField is not null)
                        {
                            foreach (object? obj in recordField.Values)
                            {
                                if (obj is null) continue;
                                string? fileName = _mediaFileManager.FileSystem.GetFileName(obj.ToString()!);
                                string? path = obj.ToString()?.Replace(fileName, string.Empty);
                                if (_mediaFileManager.FileSystem.FileExists(obj.ToString()!))
                                {
                                    _mediaFileManager.FileSystem.DeleteFile(obj.ToString()!);
                                    if (!string.IsNullOrEmpty(path) && _mediaFileManager.FileSystem.DirectoryExists(path))
                                        _mediaFileManager.FileSystem.DeleteDirectory(path, true);
                                }
                            }
                        }
                    }
                }
                List<int> array = [.. records.Select(x => x.Id)];
                if (array.Count != 0)
                {
                    foreach (var batch in GetBatches(2000, array))
                    {
                        _fieldValueStorage.DeleteAllRecordFieldValues(batch);
                        _fieldValueStorage.DeleteAllRecordAuditValues(batch);
                        Sql<ISqlContext> sql = scope.Database.SqlContext.Sql().Delete<Record>().WhereIn<Record>(r => r.Id, batch);
                        scope.Database.Execute(sql);
                    }
                }
                  ((ICoreScope)scope).Complete();
                return true;
            }
        }

        private static IEnumerable<IList<int>> GetBatches(
          int batchSize,
          List<int> allRecordIds)
        {
            int count = allRecordIds.Count;
            int skip = 0;
            do
            {
                int[] array = [.. allRecordIds.Skip(skip).Take(batchSize)];
                skip += batchSize;
                yield return array;
            }
            while (skip <= count);
        }

        public Record UpdateRecord(Record record, Form form) => UpdateRecord(record, form, new int?());

        public Record UpdateRecord(Record record, Form form, int? userId)
        {
            IProfilingLogger profilingLogger = _profilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(43, 3);
            interpolatedStringHandler.AppendLiteral("Update Record '");
            interpolatedStringHandler.AppendFormatted(record.UniqueId);
            interpolatedStringHandler.AppendLiteral("' for the Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<RecordStorage>(stringAndClear))
            {
                using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
                record.Updated = DateTime.Now;
                foreach (KeyValuePair<Guid, RecordField> recordField in record.RecordFields)
                    recordField.Value.Record = record.Id;
                EventMessages messages = _eventMessagesFactory.Get();
                RecordSavingNotification notification = new(record, messages);
                if (((ICoreScope)scope).Notifications.PublishCancelable(notification))
                {
                    ((ICoreScope)scope).Complete();
                    return record;
                }
                record.RecordData = record.GenerateRecordDataAsJson();
                scope.Database.Update(record);
                Dictionary<Guid, RecordField> dictionary = [];
                _fieldValueStorage.DeleteAllRecordFieldValues(record);
                List<RecordField> source1 = [];
                List<RecordField> source2 = [];
                foreach (KeyValuePair<Guid, RecordField> recordField in record.RecordFields)
                {
                    if (recordField.Value.Field is not null)
                    {
                        FieldType? fieldTypeByField = _fieldTypeStorage.GetFieldTypeByField(recordField.Value.Field);
                        if (fieldTypeByField is null || fieldTypeByField.StoresData)
                        {
                            dictionary.Add(recordField.Key, recordField.Value);
                            if (recordField.Value.Key == Guid.Empty)
                                source1.Add(recordField.Value);
                            else
                                source2.Add(recordField.Value);
                        }
                    }
                }
                if (source1.Count != 0)
                    _recordFieldStorage.InsertRecordFields(record.RecordFields.Select(rf => rf.Value));
                if (source2.Count != 0)
                    _recordFieldStorage.UpdateRecordFields(record.RecordFields.Select(rf => rf.Value));
                record.RecordFields = dictionary;
                RecordAudit poco = new()
                {
                    Record = record.Id,
                    UpdatedOn = record.Updated,
                    UpdatedBy = userId
                };
                scope.Database.Insert(poco);
                ((ICoreScope)scope).Complete();
                return record;
            }
        }

        public int GetRecordCount(Form form)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            Sql<ISqlContext> sql = scope.Database.SqlContext.Sql().SelectCount(null).From<Record>(null).Where<Record>(x => x.Form == form.Id, null);
            int recordCount = scope.Database.ExecuteScalar<int>(sql);
            ((ICoreScope)scope).Complete();
            return recordCount;
        }

#pragma warning restore CS0618 // Type or member is obsolete
    }
}