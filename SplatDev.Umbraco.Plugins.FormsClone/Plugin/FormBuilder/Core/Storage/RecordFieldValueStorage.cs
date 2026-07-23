using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Persistence.Interfaces;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.Extensions.Logging;

using NPoco;

using System.Data;
using System.Text.Json;

using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extensions;

namespace FormBuilder.Core.Storage
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class RecordFieldValueStorage(
      IScopeProvider scopeProvider,
      FieldCollection fieldCollection,
      IFieldTypeStorage fieldTypeStorage,
      ILogger<RecordFieldValueStorage> logger) : IRecordFieldValueStorage
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;
        private readonly FieldCollection _fieldCollection = fieldCollection;
        private readonly IFieldTypeStorage _fieldTypeStorage = fieldTypeStorage;
        private readonly ILogger<RecordFieldValueStorage> _logger = logger;

        public List<object> GetRecordFieldValues(RecordField recordFieldInForm)
        {
            List<object> recordFieldValues = [];
            using (IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false))
            {
                if (recordFieldInForm.Field is not null)
                {
                    FieldType? fieldTypeByField = _fieldTypeStorage.GetFieldTypeByField(recordFieldInForm.Field);
                    if (fieldTypeByField is not null)
                    {
                        switch (fieldTypeByField.DataType)
                        {
                            case FieldDataType.String:
                                IUmbracoDatabase database1 = scope.Database;
                                Sql<ISqlContext> sql1 = scope.SqlContext.Sql().Select<RecordFieldDataString>([]).From<RecordFieldDataString>(null).Where<RecordFieldDataString>(x => x.Key == recordFieldInForm.Key, null);
                                using (List<RecordFieldDataString>.Enumerator enumerator = database1.Fetch<RecordFieldDataString>(sql1).GetEnumerator())
                                {
                                    while (enumerator.MoveNext())
                                    {
                                        RecordFieldDataString current = enumerator.Current;
                                        recordFieldValues.Add(current.Value);
                                    }
                                    break;
                                }
                            case FieldDataType.LongString:
                                IUmbracoDatabase database2 = scope.Database;
                                Sql<ISqlContext> sql2 = scope.SqlContext.Sql().Select<RecordFieldDataLongString>([]).From<RecordFieldDataLongString>(null).Where<RecordFieldDataLongString>(x => x.Key == recordFieldInForm.Key, null);
                                using (List<RecordFieldDataLongString>.Enumerator enumerator = database2.Fetch<RecordFieldDataLongString>(sql2).GetEnumerator())
                                {
                                    while (enumerator.MoveNext())
                                    {
                                        RecordFieldDataLongString current = enumerator.Current;
                                        recordFieldValues.Add(current.Value);
                                    }
                                    break;
                                }
                            case FieldDataType.Integer:
                                IUmbracoDatabase database3 = scope.Database;
                                Sql<ISqlContext> sql3 = scope.SqlContext.Sql().Select<RecordFieldDataInteger>([]).From<RecordFieldDataInteger>(null).Where<RecordFieldDataInteger>(x => x.Key == recordFieldInForm.Key, null);
                                using (List<RecordFieldDataInteger>.Enumerator enumerator = database3.Fetch<RecordFieldDataInteger>(sql3).GetEnumerator())
                                {
                                    while (enumerator.MoveNext())
                                    {
                                        RecordFieldDataInteger current = enumerator.Current;
                                        recordFieldValues.Add(current.Value);
                                    }
                                    break;
                                }
                            case FieldDataType.DateTime:
                                IUmbracoDatabase database4 = scope.Database;
                                Sql<ISqlContext> sql4 = scope.SqlContext.Sql().Select<RecordFieldDataDateTime>([]).From<RecordFieldDataDateTime>(null).Where<RecordFieldDataDateTime>(x => x.Key == recordFieldInForm.Key, null);
                                using (List<RecordFieldDataDateTime>.Enumerator enumerator = database4.Fetch<RecordFieldDataDateTime>(sql4).GetEnumerator())
                                {
                                    while (enumerator.MoveNext())
                                    {
                                        RecordFieldDataDateTime current = enumerator.Current;
                                        recordFieldValues.Add(current.Value);
                                    }
                                    break;
                                }
                            case FieldDataType.Bit:
                                IUmbracoDatabase database5 = scope.Database;
                                Sql<ISqlContext> sql5 = scope.SqlContext.Sql().Select<RecordFieldDataBit>([]).From<RecordFieldDataBit>(null).Where<RecordFieldDataBit>(x => x.Key == recordFieldInForm.Key, null);
                                using (List<RecordFieldDataBit>.Enumerator enumerator = database5.Fetch<RecordFieldDataBit>(sql5).GetEnumerator())
                                {
                                    while (enumerator.MoveNext())
                                    {
                                        RecordFieldDataBit current = enumerator.Current;
                                        recordFieldValues.Add(current.Value);
                                    }
                                    break;
                                }
                        }
                    }
                }
              ((ICoreScope)scope).Complete();
            }
            return recordFieldValues;
        }

        public List<object> InsertRecordFieldValues(RecordField recordFieldInForm)
        {
            if (recordFieldInForm?.Field is null)
                throw new ArgumentNullException(nameof(recordFieldInForm));
            FieldType? field = _fieldCollection[recordFieldInForm.Field.FieldTypeId];
            if (field is null)
            {
                _logger.LogWarning("The field type for the {RecordField} was not found so the record will not be saved.", recordFieldInForm.Alias);
                return recordFieldInForm.Values;
            }
            string tableName = GetTableName(field.DataType);
            Type toType = ParseToType(field.DataType);
            if (recordFieldInForm.Values.Count > 1)
                _logger.LogDebug("The Record Field {RecordField} with DataType {FieldDataType} has more than one value to save. Expect mulitple SQL calls", recordFieldInForm.Alias, recordFieldInForm.DataTypeAlias);
            using (IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false))
            {
                foreach (object obj in recordFieldInForm.Values)
                    scope.Database.Execute("INSERT INTO " + tableName + "([Key], [Value]) VALUES(@key, @value)", new
                    {
                        key = recordFieldInForm.Key,
                        value = GetTypedValue(toType, obj)
                    });
                ((ICoreScope)scope).Complete();
            }
            return recordFieldInForm.Values;
        }

        private static object GetTypedValue(Type dataTypeObjectType, object value) => value is JsonElement jsonElement ? jsonElement.ToString() : Convert.ChangeType(value, dataTypeObjectType);

        public bool DeleteRecordFieldValues(RecordField recordFieldInForm)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false) ?? throw new InvalidOperationException("Could not access ambient scope.");
            foreach (string name in Enum.GetNames<FieldDataType>())
            {
                string sql = "DELETE FROM FormBuilderRecordData" + name + " WHERE [Key] = @key";
                scope.Database.Execute(sql, new
                {
                    key = recordFieldInForm.Key
                });
            }
              ((ICoreScope)scope).Complete();
            return true;
        }

        public bool DeleteRecordFieldValues(IEnumerable<RecordField> recordFieldInForms)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            foreach (string name in Enum.GetNames<FieldDataType>())
            {
                string sql = "DELETE FROM FormBuilderRecordData" + name + " WHERE [Key] in (@key)";
                scope.Database.Execute(sql, new
                {
                    key = recordFieldInForms.Select((Func<RecordField, Guid>)(p => p.Key))
                });
            }
              ((ICoreScope)scope).Complete();
            return true;
        }

        public void DeleteAllRecordFieldValues(Record record) => DeleteAllRecordFieldValues(
        [
            record.Id
        ]);

        public void DeleteAllRecordFieldValues(IList<int> recordIds)
        {
            if (!recordIds.Any())
                return;
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            scope.Database.Execute(GetDeleteRecordFieldDataSql<RecordFieldDataBit>(scope, recordIds));
            scope.Database.Execute(GetDeleteRecordFieldDataSql<RecordFieldDataDateTime>(scope, recordIds));
            scope.Database.Execute(GetDeleteRecordFieldDataSql<RecordFieldDataInteger>(scope, recordIds));
            scope.Database.Execute(GetDeleteRecordFieldDataSql<RecordFieldDataLongString>(scope, recordIds));
            scope.Database.Execute(GetDeleteRecordFieldDataSql<RecordFieldDataString>(scope, recordIds));
            string sql = "DELETE FROM FormBuilderRecordFields WHERE FormBuilderRecordFields.Record in (" + string.Join(", ", recordIds.Select((x, i) => "@" + i.ToString())) + ")";
            scope.Database.Execute(sql, [.. recordIds.Cast<object>()]);
            ((ICoreScope)scope).Complete();
        }

        public void DeleteAllRecordAuditValues(Record record) => DeleteAllRecordAuditValues(
        [
            record.Id
        ]);

        public void DeleteAllRecordAuditValues(IList<int> recordIds)
        {
            if (!recordIds.Any())
                return;
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            string sql1 = "DELETE FROM FormBuilderRecordAudit WHERE FormBuilderRecordAudit.Record IN (" + string.Join(", ", recordIds.Select((x, i) => "@" + i.ToString())) + ")";
            scope.Database.Execute(sql1, [.. recordIds.Cast<object>()]);
            string sql2 = "DELETE FROM FormBuilderRecordWorkflowAudit WHERE RecordUniqueId IN (SELECT UniqueId FROM FormBuilderRecords WHERE FormBuilderRecords.Id IN (" + string.Join(", ", recordIds.Select((x, i) => "@" + i.ToString())) + "))";
            scope.Database.Execute(sql2, [.. recordIds.Cast<object>()]);
            ((ICoreScope)scope).Complete();
        }

        private static Sql<ISqlContext> GetDeleteRecordFieldDataSql<T>(
          IScope ambientScope,
          IList<int> recordIds)
          where T : IRecordFieldData
        {
            return ambientScope.SqlContext.Sql().Delete().From<T>(null).WhereIn<T>(x => x.Id, ambientScope.SqlContext.Sql().Select<T>([
                 x =>  x.Id
            ]).From<T>(null).InnerJoin<RecordField>(null).On<T, RecordField>(left => left.Key, right => right.Key).InnerJoin<Record>(null).On<RecordField, Record>(left => left.Record, right => right.Id).WhereIn<Record>(x => x.Id, recordIds));
        }

        private static string GetTableName(FieldDataType dataType) => "UFRecordData" + Enum.GetName(dataType.GetType(), dataType);

        private static Type ParseToType(FieldDataType dataType)
        {
            return dataType switch
            {
                FieldDataType.String => typeof(string),
                FieldDataType.LongString => typeof(string),
                FieldDataType.Integer => typeof(int),
                FieldDataType.DateTime => typeof(DateTime),
                FieldDataType.Bit => typeof(bool),
                _ => typeof(object),
            };
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}