using Examine;

using FormBuilder.Core.Options;
using FormBuilder.Core.Persistence.Fields;

using System.Text;
using System.Text.Json;

using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Extensions;

namespace FormBuilder.Examine.ValueSetBuilders
{
    internal sealed class RecordValueSetBuilder : IValueSetBuilder<Record>
    {
        public IEnumerable<ValueSet> GetValueSets(params Record[] records)
        {
            Record[]? recordArray = records;
            for (int index = 0; index < recordArray.Length; ++index)
            {
                Record record = recordArray[index];
                string str = string.Empty;
                string xml1 = "| ";
                foreach (KeyValuePair<Guid, RecordField> recordField in record.RecordFields)
                {
                    str = str + "'" + recordField.Value.FieldId.ToString() + "':'" + recordField.Value.ValuesAsString() + "',";
                    xml1 = xml1 + recordField.Value.ValuesAsString() + " ";
                }
                string xml2 = "{" + str.TrimEnd(',') + "}";
                Dictionary<string, object> dictionary = new()
                {
                    { "fields", SanitizeXmlString(xml2) },
                    { "blob", SanitizeXmlString(xml1) },
                    { "State", SanitizeXmlString(Enum.GetName(record.State)!) },
                    { "Ip", record.IP }
                };
                Guid guid = record.UniqueId;
                dictionary.Add("UniqueId", guid.ToString());
                dictionary.Add("Updated", record.Updated.ToString("s"));
                dictionary.Add("Created", record.Created.ToString("s"));
                guid = record.Form;
                dictionary.Add("Form", guid.ToString());
                dictionary.Add("MemberKey", record.MemberKey ?? string.Empty);
                dictionary.Add("CurrentPage", record.CurrentPage);
                dictionary.Add("UmbracoPageId", record.UmbracoPageId);
                dictionary.Add("RecordFields", JsonSerializer.Serialize(record.RecordFields, FormsJsonSerializerOptions.Default));
                Dictionary<string, object> values = dictionary;
                yield return new ValueSet(record.Id.ToInvariantString(), "FormBuilder", "Record", (IDictionary<string, object>)values);
            }
        }

        internal static string SanitizeXmlString(string xml)
        {
            StringBuilder stringBuilder = xml is not null ? new StringBuilder(xml.Length) : throw new ArgumentNullException(nameof(xml));
            foreach (char character in xml)
            {
                if (IsLegalXmlChar(character))
                    stringBuilder.Append(character);
            }
            return stringBuilder.ToString();
        }

        private static bool IsLegalXmlChar(int character)
        {
            if (character == 9 || character == 10 || character == 13 || character >= 32 && character <= 55295 || character >= 57344 && character <= 65533)
                return true;
            return character >= 65536 && character <= 1114111;
        }
    }
}