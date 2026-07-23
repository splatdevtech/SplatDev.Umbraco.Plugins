using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Options;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Interfaces;

using System.Text.Json;

namespace FormBuilder.Core.Extensions
{
    public static class RecordExtensions
    {
        public static string ValueAsString(this Record record, string alias)
        {
            ArgumentNullException.ThrowIfNull(record);
            ArgumentNullException.ThrowIfNull(alias);
            foreach (RecordField recordField in record.RecordFields.Values)
            {
                if (string.Equals(recordField.Alias, alias))
                    return recordField.ValuesAsString();
            }
            return string.Empty;
        }

        public static IDictionary<string, string?>? GetAdditionalData(this Record record)
        {
            string? additionalData = record.AdditionalData;
            return additionalData is null || string.IsNullOrEmpty(additionalData) ? null : (IDictionary<string, string?>?)JsonSerializer.Deserialize<Dictionary<string, string?>>(additionalData, FormsJsonSerializerOptions.Default);
        }

        public static void SetAdditionalData(
          this Record record,
          IDictionary<string, string?>? additionalData)
        {
            record.AdditionalData = additionalData is null || additionalData.Count <= 0 ? null : JsonSerializer.Serialize(additionalData, FormsJsonSerializerOptions.Default);
        }

        internal static Dictionary<string, object[]> CreateFormStateFromRecord(
          this Record record,
          Form form,
          IFieldTypeStorage fieldTypeStorage)
        {
            Dictionary<string, object[]> formStateFromRecord = [];
            foreach (KeyValuePair<Guid, RecordField> recordField in record.RecordFields)
            {
                KeyValuePair<Guid, RecordField> entry = recordField;
                Field? field = form.AllFields.FirstOrDefault(x => x.Id == entry.Value.FieldId);
                if (field is not null)
                {
                    FieldType? fieldTypeByField = fieldTypeStorage.GetFieldTypeByField(field);
                    if (fieldTypeByField is not null)
                        formStateFromRecord.Add(field.Id.ToString(), [.. fieldTypeByField.ConvertFromRecord(field, entry.Value.Values)]);
                }
            }
            return formStateFromRecord;
        }

        internal static Dictionary<string, string> DeserializeRecordData(this Record record) => string.IsNullOrWhiteSpace(record.RecordData) ? [] : JsonSerializer.Deserialize<Dictionary<string, string>>(record.RecordData.Replace("\"", "\\\"").Replace("\\'", "~||~").Replace("'", "\"").Replace("~||~", "'").ReplaceNewlines("\\r\\n"), FormsJsonSerializerOptions.Default) ?? [];
    }
}