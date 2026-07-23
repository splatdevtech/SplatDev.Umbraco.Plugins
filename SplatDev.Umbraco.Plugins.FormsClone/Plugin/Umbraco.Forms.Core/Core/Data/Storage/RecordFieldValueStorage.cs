
// Type: Umbraco.Forms.Core.Data.Storage.RecordFieldValueStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Logging;
using NPoco;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Core.Data.Storage
{
  internal sealed class RecordFieldValueStorage : IRecordFieldValueStorage
  {
    private readonly IScopeProvider _scopeProvider;
    private readonly FieldCollection _fieldCollection;
    private readonly IFieldTypeStorage _fieldTypeStorage;
    private readonly ILogger<RecordFieldValueStorage> _logger;

    public RecordFieldValueStorage(
      IScopeProvider scopeProvider,
      FieldCollection fieldCollection,
      IFieldTypeStorage fieldTypeStorage,
      ILogger<RecordFieldValueStorage> logger)
    {
      this._scopeProvider = scopeProvider;
      this._fieldCollection = fieldCollection;
      this._fieldTypeStorage = fieldTypeStorage;
      this._logger = logger;
    }

    public List<object> GetRecordFieldValues(RecordField recordFieldInForm)
    {
      List<object> recordFieldValues = new List<object>();
      using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, (IEventDispatcher) null, (IScopedNotificationPublisher) null, null, false, false))
      {
        if (recordFieldInForm.Field != null)
        {
          FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(recordFieldInForm.Field);
          if (fieldTypeByField != null)
          {
            switch (fieldTypeByField.DataType)
            {
              case FieldDataType.String:
                IUmbracoDatabase database1 = scope.Database;
                Sql<ISqlContext> sql1 = NPocoSqlExtensions.Where<RecordFieldDataString>(NPocoSqlExtensions.From<RecordFieldDataString>(NPocoSqlExtensions.Select<RecordFieldDataString>(scope.SqlContext.Sql(), Array.Empty<Expression<Func<RecordFieldDataString, object>>>()), (string) null), (Expression<Func<RecordFieldDataString, bool>>) (x => x.Key == recordFieldInForm.Key), (string) null);
                using (List<RecordFieldDataString>.Enumerator enumerator = ((IDatabaseQuery) database1).Fetch<RecordFieldDataString>((Sql) sql1).GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    RecordFieldDataString current = enumerator.Current;
                    recordFieldValues.Add((object) current.Value);
                  }
                  break;
                }
              case FieldDataType.LongString:
                IUmbracoDatabase database2 = scope.Database;
                Sql<ISqlContext> sql2 = NPocoSqlExtensions.Where<RecordFieldDataLongString>(NPocoSqlExtensions.From<RecordFieldDataLongString>(NPocoSqlExtensions.Select<RecordFieldDataLongString>(scope.SqlContext.Sql(), Array.Empty<Expression<Func<RecordFieldDataLongString, object>>>()), (string) null), (Expression<Func<RecordFieldDataLongString, bool>>) (x => x.Key == recordFieldInForm.Key), (string) null);
                using (List<RecordFieldDataLongString>.Enumerator enumerator = ((IDatabaseQuery) database2).Fetch<RecordFieldDataLongString>((Sql) sql2).GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    RecordFieldDataLongString current = enumerator.Current;
                    recordFieldValues.Add((object) current.Value);
                  }
                  break;
                }
              case FieldDataType.Integer:
                IUmbracoDatabase database3 = scope.Database;
                Sql<ISqlContext> sql3 = NPocoSqlExtensions.Where<RecordFieldDataInteger>(NPocoSqlExtensions.From<RecordFieldDataInteger>(NPocoSqlExtensions.Select<RecordFieldDataInteger>(scope.SqlContext.Sql(), Array.Empty<Expression<Func<RecordFieldDataInteger, object>>>()), (string) null), (Expression<Func<RecordFieldDataInteger, bool>>) (x => x.Key == recordFieldInForm.Key), (string) null);
                using (List<RecordFieldDataInteger>.Enumerator enumerator = ((IDatabaseQuery) database3).Fetch<RecordFieldDataInteger>((Sql) sql3).GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    RecordFieldDataInteger current = enumerator.Current;
                    recordFieldValues.Add((object) current.Value);
                  }
                  break;
                }
              case FieldDataType.DateTime:
                IUmbracoDatabase database4 = scope.Database;
                Sql<ISqlContext> sql4 = NPocoSqlExtensions.Where<RecordFieldDataDateTime>(NPocoSqlExtensions.From<RecordFieldDataDateTime>(NPocoSqlExtensions.Select<RecordFieldDataDateTime>(scope.SqlContext.Sql(), Array.Empty<Expression<Func<RecordFieldDataDateTime, object>>>()), (string) null), (Expression<Func<RecordFieldDataDateTime, bool>>) (x => x.Key == recordFieldInForm.Key), (string) null);
                using (List<RecordFieldDataDateTime>.Enumerator enumerator = ((IDatabaseQuery) database4).Fetch<RecordFieldDataDateTime>((Sql) sql4).GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    RecordFieldDataDateTime current = enumerator.Current;
                    recordFieldValues.Add((object) current.Value);
                  }
                  break;
                }
              case FieldDataType.Bit:
                IUmbracoDatabase database5 = scope.Database;
                Sql<ISqlContext> sql5 = NPocoSqlExtensions.Where<RecordFieldDataBit>(NPocoSqlExtensions.From<RecordFieldDataBit>(NPocoSqlExtensions.Select<RecordFieldDataBit>(scope.SqlContext.Sql(), Array.Empty<Expression<Func<RecordFieldDataBit, object>>>()), (string) null), (Expression<Func<RecordFieldDataBit, bool>>) (x => x.Key == recordFieldInForm.Key), (string) null);
                using (List<RecordFieldDataBit>.Enumerator enumerator = ((IDatabaseQuery) database5).Fetch<RecordFieldDataBit>((Sql) sql5).GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    RecordFieldDataBit current = enumerator.Current;
                    recordFieldValues.Add((object) current.Value);
                  }
                  break;
                }
            }
          }
        }
        ((ICoreScope) scope).Complete();
      }
      return recordFieldValues;
    }

    public List<object> InsertRecordFieldValues(RecordField recordFieldInForm)
    {
      if (recordFieldInForm?.Field == null)
        throw new ArgumentNullException(nameof (recordFieldInForm));
      FieldType field = this._fieldCollection[recordFieldInForm.Field.FieldTypeId];
      if (field == null)
      {
        this._logger.LogWarning("The field type for the {RecordField} was not found so the record will not be saved.", (object) recordFieldInForm.Alias);
        return recordFieldInForm.Values;
      }
      string tableName = this.GetTableName(field.DataType);
      Type toType = this.ParseToType(field.DataType);
      if (recordFieldInForm.Values.Count > 1)
        this._logger.LogDebug("The Record Field {RecordField} with DataType {FieldDataType} has more than one value to save. Expect mulitple SQL calls", (object) recordFieldInForm.Alias, (object) recordFieldInForm.DataTypeAlias);
      using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, (IEventDispatcher) null, (IScopedNotificationPublisher) null, null, false, false))
      {
        foreach (object obj in recordFieldInForm.Values)
          scope.Database.Execute("INSERT INTO " + tableName + "([Key], [Value]) VALUES(@key, @value)", (object) new
          {
            key = recordFieldInForm.Key,
            value = RecordFieldValueStorage.GetTypedValue(toType, obj)
          });
        ((ICoreScope) scope).Complete();
      }
      return recordFieldInForm.Values;
    }

    private static object GetTypedValue(Type dataTypeObjectType, object value) => value is JsonElement jsonElement ? (object) jsonElement.ToString() : Convert.ChangeType(value, dataTypeObjectType);

    public bool DeleteRecordFieldValues(RecordField recordFieldInForm)
    {
      using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, (IEventDispatcher) null, (IScopedNotificationPublisher) null, null, false, false))
      {
        if (scope == null)
          throw new InvalidOperationException("Could not access ambient scope.");
        foreach (string name in Enum.GetNames(typeof (FieldDataType)))
        {
          string sql = "DELETE FROM UFRecordData" + name + " WHERE [Key] = @key";
          scope.Database.Execute(sql, (object) new
          {
            key = recordFieldInForm.Key
          });
        }
        ((ICoreScope) scope).Complete();
      }
      return true;
    }

    public bool DeleteRecordFieldValues(IEnumerable<RecordField> recordFieldInForms)
    {
      using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, (IEventDispatcher) null, (IScopedNotificationPublisher) null, null, false, false))
      {
        foreach (string name in Enum.GetNames(typeof (FieldDataType)))
        {
          string sql = "DELETE FROM UFRecordData" + name + " WHERE [Key] in (@key)";
          scope.Database.Execute(sql, (object) new
          {
            key = recordFieldInForms.Select<RecordField, Guid>((Func<RecordField, Guid>) (p => p.Key))
          });
        }
        ((ICoreScope) scope).Complete();
      }
      return true;
    }

    public void DeleteAllRecordFieldValues(Record record) => this.DeleteAllRecordFieldValues((IList<int>) new int[1]
    {
      record.Id
    });

    public void DeleteAllRecordFieldValues(IList<int> recordIds)
    {
      if (!recordIds.Any<int>())
        return;
      using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, (IEventDispatcher) null, (IScopedNotificationPublisher) null, null, false, false))
      {
        scope.Database.Execute((Sql) this.GetDeleteRecordFieldDataSql<RecordFieldDataBit>(scope, recordIds));
        scope.Database.Execute((Sql) this.GetDeleteRecordFieldDataSql<RecordFieldDataDateTime>(scope, recordIds));
        scope.Database.Execute((Sql) this.GetDeleteRecordFieldDataSql<RecordFieldDataInteger>(scope, recordIds));
        scope.Database.Execute((Sql) this.GetDeleteRecordFieldDataSql<RecordFieldDataLongString>(scope, recordIds));
        scope.Database.Execute((Sql) this.GetDeleteRecordFieldDataSql<RecordFieldDataString>(scope, recordIds));
        string sql = "DELETE FROM UFRecordFields WHERE UFRecordFields.Record in (" + string.Join(", ", recordIds.Select<int, string>((Func<int, int, string>) ((x, i) => "@" + i.ToString()))) + ")";
        scope.Database.Execute(sql, recordIds.Cast<object>().ToArray<object>());
        ((ICoreScope) scope).Complete();
      }
    }

    public void DeleteAllRecordAuditValues(Record record) => this.DeleteAllRecordAuditValues((IList<int>) new int[1]
    {
      record.Id
    });

    public void DeleteAllRecordAuditValues(IList<int> recordIds)
    {
      if (!recordIds.Any<int>())
        return;
      using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, (IEventDispatcher) null, (IScopedNotificationPublisher) null, null, false, false))
      {
        string sql1 = "DELETE FROM UFRecordAudit WHERE UFRecordAudit.Record IN (" + string.Join(", ", recordIds.Select<int, string>((Func<int, int, string>) ((x, i) => "@" + i.ToString()))) + ")";
        scope.Database.Execute(sql1, recordIds.Cast<object>().ToArray<object>());
        string sql2 = "DELETE FROM UFRecordWorkflowAudit WHERE RecordUniqueId IN (SELECT UniqueId FROM UFRecords WHERE UFRecords.Id IN (" + string.Join(", ", recordIds.Select<int, string>((Func<int, int, string>) ((x, i) => "@" + i.ToString()))) + "))";
        scope.Database.Execute(sql2, recordIds.Cast<object>().ToArray<object>());
        ((ICoreScope) scope).Complete();
      }
    }

    private Sql<ISqlContext> GetDeleteRecordFieldDataSql<T>(
      IScope ambientScope,
      IList<int> recordIds)
      where T : IRecordFieldData
    {
      return NPocoSqlExtensions.WhereIn<T>(NPocoSqlExtensions.From<T>(NPocoSqlExtensions.Delete(ambientScope.SqlContext.Sql()), (string) null), (Expression<Func<T, object>>) (x => (object) x.Id), NPocoSqlExtensions.WhereIn<Record>(NPocoSqlExtensions.On<RecordField, Record>(NPocoSqlExtensions.InnerJoin<Record>(NPocoSqlExtensions.On<T, RecordField>(NPocoSqlExtensions.InnerJoin<RecordField>(NPocoSqlExtensions.From<T>(NPocoSqlExtensions.Select<T>(ambientScope.SqlContext.Sql(), new Expression<Func<T, object>>[1]
      {
        (Expression<Func<T, object>>) (x => (object) x.Id)
      }), (string) null), (string) null), (Expression<Func<T, object>>) (left => (object) left.Key), (Expression<Func<RecordField, object>>) (right => (object) right.Key)), (string) null), (Expression<Func<RecordField, object>>) (left => (object) left.Record), (Expression<Func<Record, object>>) (right => (object) right.Id)), (Expression<Func<Record, object>>) (x => (object) x.Id), (IEnumerable) recordIds));
    }

    private string GetTableName(FieldDataType dataType) => "UFRecordData" + Enum.GetName(dataType.GetType(), (object) dataType);

    private Type ParseToType(FieldDataType dataType)
    {
      switch (dataType)
      {
        case FieldDataType.String:
          return typeof (string);
        case FieldDataType.LongString:
          return typeof (string);
        case FieldDataType.Integer:
          return typeof (int);
        case FieldDataType.DateTime:
          return typeof (DateTime);
        case FieldDataType.Bit:
          return typeof (bool);
        default:
          return typeof (object);
      }
    }
  }
}
