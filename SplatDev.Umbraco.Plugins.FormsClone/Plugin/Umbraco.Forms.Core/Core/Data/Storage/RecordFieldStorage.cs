
// Type: Umbraco.Forms.Core.Data.Storage.RecordFieldStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using System.Data;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Scoping;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Exceptions;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Core.Data.Storage
{
    internal sealed class RecordFieldStorage : IRecordFieldStorage
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IFieldTypeStorage _fieldTypeStorage;
        private readonly IRecordFieldValueStorage _recordFieldValueStorage;

        public RecordFieldStorage(
          IScopeProvider scopeProvider,
          IFieldTypeStorage fieldTypeStorage,
          IRecordFieldValueStorage recordFieldValueStorage)
        {
            this._scopeProvider = scopeProvider;
            this._fieldTypeStorage = fieldTypeStorage;
            this._recordFieldValueStorage = recordFieldValueStorage;
        }

        public Dictionary<Guid, RecordField> GetAllRecordFields(
          Record record,
          Form form)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                Dictionary<Guid, RecordField> recordFields = this.GetRecordFields(scope.Database.Fetch<RecordField>("WHERE record = @recordId", new
                {
                    recordId = record.Id
                }), form);
                scope.Complete();
                return recordFields;
            }
        }

        public Dictionary<Guid, RecordField> GetAllRecordFields(
          IEnumerable<Record> records,
          Form form)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                IEnumerable<IEnumerable<Record>> records1 = records.ToList<Record>().InGroupsOf<Record>(1000);
                Dictionary<Guid, RecordField> allRecordFields = new Dictionary<Guid, RecordField>();
                foreach (IEnumerable<Record> source in records1)
                {
                    foreach (KeyValuePair<Guid, RecordField> recordField in this.GetRecordFields(scope.Database.Fetch<RecordField>("WHERE record in (@recordIds)", new
                    {
                        recordIds = source.Select<Record, int>((Func<Record, int>)(p => p.Id))
                    }), form))
                    {
                        if (!allRecordFields.ContainsKey(recordField.Key))
                            allRecordFields.Add(recordField.Key, recordField.Value);
                    }
                }
                scope.Complete();
                return allRecordFields;
            }
        }

        private Dictionary<Guid, RecordField> GetRecordFields(
          IEnumerable<RecordField> recordFields,
          Form form)
        {
            Dictionary<Guid, RecordField> recordFields1 = new Dictionary<Guid, RecordField>();
            foreach (RecordField recordField1 in (IEnumerable<RecordField>)recordFields.OrderBy<RecordField, int>(x => x.FormFieldOrder(form.AllFields)))
            {
                RecordField recordField = recordField1;
                recordField.Field = form.AllFields.FirstOrDefault<Field>(f => f.Id == recordField.FieldId);
                recordField.Values = this._recordFieldValueStorage.GetRecordFieldValues(recordField);
                if (!recordFields1.ContainsKey(recordField.Key))
                    recordFields1.Add(recordField.Key, recordField);
            }
            return recordFields1;
        }

        public RecordField? GetRecordField(Guid key)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                RecordField recordFieldInForm = scope.Database.Fetch<RecordField>().SingleOrDefault<RecordField>(x => x.Key == key);
                if (recordFieldInForm != null)
                {
                    recordFieldInForm.Values = this._recordFieldValueStorage.GetRecordFieldValues(recordFieldInForm);
                    RecordField recordField = recordFieldInForm;
                    Field field = recordFieldInForm.Field;
                    Guid guid = field != null ? field.Id : Guid.Empty;
                    recordField.FieldId = guid;
                }
                scope.Complete();
                return recordFieldInForm;
            }
        }

        public IEnumerable<RecordField> InsertRecordFields(
          IEnumerable<RecordField> recordFields)
        {
            if (!recordFields.Any<RecordField>() || recordFields.Any<RecordField>(p => p.Field == null) || recordFields.Any<RecordField>(p => p.Field.Id == Guid.Empty))
                return recordFields;
            List<RecordField> list = recordFields.Where<RecordField>(recordField => this._fieldTypeStorage.GetFieldTypeByField(recordField.Field) != null).Select<RecordField, RecordField>(recordField =>
            {
                Guid guid = Guid.NewGuid();
                recordField.Key = guid;
                return new RecordField()
                {
                    Key = guid,
                    FieldId = recordField.Field.Id,
                    Alias = recordField.Field.Alias,
                    Record = recordField.Record,
                    DataTypeAlias = this._fieldTypeStorage.GetFieldTypeByField(recordField.Field).DataType.ToString()
                };
            }).ToList<RecordField>();
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                scope.Database.InsertBatch<RecordField>(list);
                recordFields.ToList<RecordField>().ForEach(recordField => this._recordFieldValueStorage.InsertRecordFieldValues(recordField));
                scope.Complete();
            }
            return recordFields;
        }

        public RecordField InsertRecordField(RecordField recordfield)
        {
            if (recordfield.Field == null || recordfield.Field.Id == Guid.Empty)
                return recordfield;
            recordfield.Key = Guid.NewGuid();
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(recordfield.Field);
                if (fieldTypeByField == null)
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(85, 1);
                    interpolatedStringHandler.AppendLiteral("Could not insert a field record as the field type for field with Id '");
                    interpolatedStringHandler.AppendFormatted<Guid>(recordfield.Field.Id);
                    interpolatedStringHandler.AppendLiteral("' was not found.");
                    throw new ProviderException(interpolatedStringHandler.ToStringAndClear());
                }
                FieldType fieldType = fieldTypeByField;
                scope.Database.Execute("INSERT INTO UFRecordFields ([Key], fieldid, alias, record, datatype) VALUES(@key, @fieldId, @fieldAlias, @recordId, @datatypeAlias)", new
                {
                    key = recordfield.Key,
                    fieldId = recordfield.Field.Id,
                    fieldAlias = recordfield.Field.Alias,
                    recordId = recordfield.Record,
                    datatypeAlias = fieldType.DataType.ToString()
                });
                this._recordFieldValueStorage.InsertRecordFieldValues(recordfield);
                scope.Complete();
            }
            return recordfield;
        }

        public bool DeleteRecordField(RecordField recordField)
        {
            this._recordFieldValueStorage.DeleteRecordFieldValues(recordField);
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                scope.Database.Delete<RecordField>(recordField);
                scope.Complete();
            }
            return true;
        }

        public RecordField UpdateRecordField(RecordField recordField)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                if (recordField.Field == null)
                    throw new InvalidOperationException("Field is not available on provided record.");
                FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(recordField.Field);
                if (fieldTypeByField == null)
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(85, 1);
                    interpolatedStringHandler.AppendLiteral("Could not update a field record as the field type for field with Id '");
                    interpolatedStringHandler.AppendFormatted<Guid>(recordField.Field.Id);
                    interpolatedStringHandler.AppendLiteral("' was not found.");
                    throw new ProviderException(interpolatedStringHandler.ToStringAndClear());
                }
                FieldType fieldType = fieldTypeByField;
                scope.Database.Execute("INSERT INTO UFRecordFields ([key], fieldid, alias, record, datatype) VALUES(@key, @fieldId, @fieldAlias, @recordId, @datatypeAlias)", new
                {
                    key = recordField.Key,
                    fieldId = recordField.Field.Id,
                    fieldAlias = recordField.Field.Alias,
                    recordId = recordField.Record,
                    datatypeAlias = fieldType.DataType.ToString()
                });
                this._recordFieldValueStorage.DeleteRecordFieldValues(recordField);
                this._recordFieldValueStorage.InsertRecordFieldValues(recordField);
                scope.Complete();
            }
            return recordField;
        }

        public IEnumerable<RecordField> UpdateRecordFields(
          IEnumerable<RecordField> recordFields)
        {
            if (recordFields.Any<RecordField>(p => p.Field == null) || recordFields.Any<RecordField>(p => p.Field.Id == Guid.Empty))
                return recordFields;
            List<RecordField> list = recordFields.Where<RecordField>(recordField => this._fieldTypeStorage.GetFieldTypeByField(recordField.Field) != null).Select<RecordField, RecordField>(recordField =>
            {
                Guid guid = Guid.NewGuid();
                recordField.Key = guid;
                return new RecordField()
                {
                    Key = guid,
                    FieldId = recordField.Field.Id,
                    Alias = recordField.Field.Alias,
                    Record = recordField.Record,
                    DataTypeAlias = this._fieldTypeStorage.GetFieldTypeByField(recordField.Field).DataType.ToString()
                };
            }).ToList<RecordField>();
            this._recordFieldValueStorage.DeleteRecordFieldValues(list);
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                if (scope.Database.DatabaseType == DatabaseType.SQLCe)
                    scope.Database.InsertBulk<RecordField>(list);
                else
                    scope.Database.InsertBatch<RecordField>(list);
                recordFields.ToList<RecordField>().ForEach(recordField => this._recordFieldValueStorage.InsertRecordFieldValues(recordField));
                scope.Complete();
            }
            return recordFields;
        }
    }
}
