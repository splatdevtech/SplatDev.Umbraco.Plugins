using FormBuilder.Core.Exceptions;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using NPoco;

using System.Data;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Scoping;
using Umbraco.Extensions;

namespace FormBuilder.Core.Storage
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class RecordFieldStorage(
      IScopeProvider scopeProvider,
      IFieldTypeStorage fieldTypeStorage,
      IRecordFieldValueStorage recordFieldValueStorage) : IRecordFieldStorage
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;
        private readonly IFieldTypeStorage _fieldTypeStorage = fieldTypeStorage;
        private readonly IRecordFieldValueStorage _recordFieldValueStorage = recordFieldValueStorage;

        public Dictionary<Guid, RecordField> GetAllRecordFields(
          Record record,
          Form form)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            Dictionary<Guid, RecordField> recordFields = GetRecordFields(scope.Database.Fetch<RecordField>("WHERE record = @recordId", new
            {
                recordId = record.Id
            }), form);
            ((ICoreScope)scope).Complete();
            return recordFields;
        }

        public Dictionary<Guid, RecordField> GetAllRecordFields(
          IEnumerable<Record> records,
          Form form)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            IEnumerable<IEnumerable<Record>> records1 = records.ToList().InGroupsOf(1000);
            Dictionary<Guid, RecordField> allRecordFields = [];
            foreach (IEnumerable<Record> source in records1)
            {
                foreach (var recordField in GetRecordFields(scope.Database.Fetch<RecordField>("WHERE record in (@recordIds)", new
                {
                    recordIds = source.Select((Func<Record, int>)(p => p.Id))
                }), form))
                {
                    if (!allRecordFields.ContainsKey(recordField.Key))
                        allRecordFields.Add(recordField.Key, recordField.Value);
                }
            }
              ((ICoreScope)scope).Complete();
            return allRecordFields;
        }

        private Dictionary<Guid, RecordField> GetRecordFields(
          IEnumerable<RecordField> recordFields,
          Form form)
        {
            Dictionary<Guid, RecordField> recordFields1 = [];
            foreach (RecordField recordField1 in (IEnumerable<RecordField>)recordFields.OrderBy(x => x.FormFieldOrder(form.AllFields)))
            {
                RecordField recordField = recordField1;
                recordField.Field = form.AllFields.FirstOrDefault(f => f.Id == recordField.FieldId);
                recordField.Values = _recordFieldValueStorage.GetRecordFieldValues(recordField);
                recordFields1.TryAdd(recordField.Key, recordField);
            }
            return recordFields1;
        }

        public RecordField? GetRecordField(Guid key)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            RecordField? recordFieldInForm = scope.Database.Fetch<RecordField>().SingleOrDefault(x => x.Key == key);
            if (recordFieldInForm is not null)
            {
                recordFieldInForm.Values = _recordFieldValueStorage.GetRecordFieldValues(recordFieldInForm);
                RecordField recordField = recordFieldInForm;
                Field? field = recordFieldInForm.Field;
                Guid guid = field is not null ? field.Id : Guid.Empty;
                recordField.FieldId = guid;
            }
              ((ICoreScope)scope).Complete();
            return recordFieldInForm;
        }

        public IEnumerable<RecordField> InsertRecordFields(
          IEnumerable<RecordField> recordFields)
        {
            if (!recordFields.Any() || recordFields.Any(p => p.Field is null) || recordFields.Any(p => p.Field?.Id == Guid.Empty))
                return recordFields;
            List<RecordField> list = [.. recordFields.Where(recordField => _fieldTypeStorage.GetFieldTypeByField(recordField.Field!) is not null).Select(recordField =>
            {
                Guid guid = Guid.NewGuid();
                recordField.Key = guid;
                return new RecordField()
                {
                    Key = guid,
                    FieldId = recordField.Field!.Id,
                    Alias = recordField.Field.Alias,
                    Record = recordField.Record,
                    DataTypeAlias = _fieldTypeStorage.GetFieldTypeByField(recordField.Field)!.DataType.ToString()
                };
            })];
            using (IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false))
            {
                scope.Database.InsertBatch(list);
                recordFields.ToList().ForEach(recordField => _recordFieldValueStorage.InsertRecordFieldValues(recordField));
                ((ICoreScope)scope).Complete();
            }
            return recordFields;
        }

        public RecordField InsertRecordField(RecordField recordfield)
        {
            if (recordfield.Field is null || recordfield.Field.Id == Guid.Empty)
                return recordfield;
            recordfield.Key = Guid.NewGuid();
            using (IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false))
            {
                FieldType? fieldTypeByField = _fieldTypeStorage.GetFieldTypeByField(recordfield.Field);
                if (fieldTypeByField is null)
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new(85, 1);
                    interpolatedStringHandler.AppendLiteral("Could not insert a field record as the field type for field with Id '");
                    interpolatedStringHandler.AppendFormatted(recordfield.Field.Id);
                    interpolatedStringHandler.AppendLiteral("' was not found.");
                    throw new ProviderException(interpolatedStringHandler.ToStringAndClear());
                }
                FieldType fieldType = fieldTypeByField;
                scope.Database.Execute("INSERT INTO FormBuilderRecordFields ([Key], fieldid, alias, record, datatype) VALUES(@key, @fieldId, @fieldAlias, @recordId, @datatypeAlias)", new
                {
                    key = recordfield.Key,
                    fieldId = recordfield.Field.Id,
                    fieldAlias = recordfield.Field.Alias,
                    recordId = recordfield.Record,
                    datatypeAlias = fieldType.DataType.ToString()
                });
                _recordFieldValueStorage.InsertRecordFieldValues(recordfield);
                ((ICoreScope)scope).Complete();
            }
            return recordfield;
        }

        public bool DeleteRecordField(RecordField recordField)
        {
            _recordFieldValueStorage.DeleteRecordFieldValues(recordField);
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            scope.Database.Delete<RecordField>(recordField);
            ((ICoreScope)scope).Complete();
            return true;
        }

        public RecordField UpdateRecordField(RecordField recordField)
        {
            using (IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false))
            {
                if (recordField.Field is null)
                    throw new InvalidOperationException("Field is not available on provided record.");
                FieldType? fieldTypeByField = _fieldTypeStorage.GetFieldTypeByField(recordField.Field);
                if (fieldTypeByField is null)
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new(85, 1);
                    interpolatedStringHandler.AppendLiteral("Could not update a field record as the field type for field with Id '");
                    interpolatedStringHandler.AppendFormatted(recordField.Field.Id);
                    interpolatedStringHandler.AppendLiteral("' was not found.");
                    throw new ProviderException(interpolatedStringHandler.ToStringAndClear());
                }
                FieldType fieldType = fieldTypeByField;
                scope.Database.Execute("INSERT INTO FormBuilderRecordFields ([key], fieldid, alias, record, datatype) VALUES(@key, @fieldId, @fieldAlias, @recordId, @datatypeAlias)", new
                {
                    key = recordField.Key,
                    fieldId = recordField.Field.Id,
                    fieldAlias = recordField.Field.Alias,
                    recordId = recordField.Record,
                    datatypeAlias = fieldType.DataType.ToString()
                });
                _recordFieldValueStorage.DeleteRecordFieldValues(recordField);
                _recordFieldValueStorage.InsertRecordFieldValues(recordField);
                ((ICoreScope)scope).Complete();
            }
            return recordField;
        }

        public IEnumerable<RecordField> UpdateRecordFields(
          IEnumerable<RecordField> recordFields)
        {
            if (recordFields.Any(p => p.Field is null) || recordFields.Any(p => p.Field!.Id == Guid.Empty))
                return recordFields;
            List<RecordField> list = [.. recordFields.Where(recordField => _fieldTypeStorage.GetFieldTypeByField(recordField.Field!) is not null).Select(recordField =>
            {
                Guid guid = Guid.NewGuid();
                recordField.Key = guid;
                return new RecordField()
                {
                    Key = guid,
                    FieldId = recordField.Field!.Id,
                    Alias = recordField.Field.Alias,
                    Record = recordField.Record,
                    DataTypeAlias = _fieldTypeStorage.GetFieldTypeByField(recordField.Field)!.DataType.ToString()
                };
            })];
            _recordFieldValueStorage.DeleteRecordFieldValues(list);
            using (IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false))
            {
                if (scope.Database.DatabaseType == DatabaseType.SQLCe)
                    scope.Database.InsertBulk(list);
                else
                    scope.Database.InsertBatch(list);
                recordFields.ToList().ForEach(recordField => _recordFieldValueStorage.InsertRecordFieldValues(recordField));
                ((ICoreScope)scope).Complete();
            }
            return recordFields;
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}