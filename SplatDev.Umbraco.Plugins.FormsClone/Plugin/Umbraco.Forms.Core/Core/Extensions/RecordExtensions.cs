
// Type: Umbraco.Forms.Core.Extensions.RecordExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Logging;

using System.Text.Json;

using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
    public static class RecordExtensions
    {
        public static string ValueAsString(this Record record, string alias)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));
            if (alias == null)
                throw new ArgumentNullException(nameof(alias));
            foreach (RecordField recordField in record.RecordFields.Values)
            {
                if (string.Equals(recordField.Alias, alias))
                    return recordField.ValuesAsString();
            }
            return string.Empty;
        }

        public static IDictionary<string, string?>? GetAdditionalData(this Record record)
        {
            string additionalData = record.AdditionalData;
            return additionalData == null || string.IsNullOrEmpty(additionalData) ? null : (IDictionary<string, string>)JsonSerializer.Deserialize<Dictionary<string, string>>(additionalData, FormsJsonSerializerOptions.Default);
        }

        public static void SetAdditionalData(
          this Record record,
          IDictionary<string, string?>? additionalData)
        {
            record.AdditionalData = additionalData == null || additionalData.Count <= 0 ? null : JsonSerializer.Serialize<IDictionary<string, string>>(additionalData, FormsJsonSerializerOptions.Default);
        }

        public static Dictionary<string, object[]> CreateFormStateFromRecord(
          this Record record,
          Form form,
          IFieldTypeStorage fieldTypeStorage)
        {
            Dictionary<string, object[]> formStateFromRecord = new Dictionary<string, object[]>();
            foreach (KeyValuePair<Guid, RecordField> recordField in record.RecordFields)
            {
                KeyValuePair<Guid, RecordField> entry = recordField;
                Field field = form.AllFields.FirstOrDefault<Field>(x => x.Id == entry.Value.FieldId);
                if (field != null)
                {
                    FieldType fieldTypeByField = fieldTypeStorage.GetFieldTypeByField(field);
                    if (fieldTypeByField != null)
                        formStateFromRecord.Add(field.Id.ToString(), fieldTypeByField.ConvertFromRecord(field, entry.Value.Values).ToArray<object>());
                }
            }
            return formStateFromRecord;
        }

        public static Dictionary<string, string> DeserializeRecordData(
          this Record record,
          ILogger logger)
        {
            string json = record.RecordData;
            if (string.IsNullOrWhiteSpace(json))
                return new Dictionary<string, string>();
            if (json.StartsWith("{'"))
                json = json.Replace("\"", "\\\"").Replace("\\'", "~||~").Replace("'", "\"").Replace("~||~", "'").ReplaceNewlines("\\r\\n");
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json, FormsJsonSerializerOptions.Default) ?? new Dictionary<string, string>();
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Could not deserialize record data for entry {RecordUniqueId} (form {FormId}), either manually fix the data or delete the entry.", record.UniqueId, record.Form);
                return new Dictionary<string, string>();
            }
        }
    }
}
