
// Type: Umbraco.Forms.Examine.Indexes.RecordValueSetBuilder
// Assembly: Umbraco.Forms.Examine, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF5A33E-94A1-42C9-B681-695454D27A51

using Examine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Extensions;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Examine.Indexes
{
  internal sealed class RecordValueSetBuilder : IValueSetBuilder<Record>
  {
    public IEnumerable<ValueSet> GetValueSets(params Record[] records)
    {
      Record[] recordArray = records;
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
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary.Add("fields", (object) RecordValueSetBuilder.SanitizeXmlString(xml2));
        dictionary.Add("blob", (object) RecordValueSetBuilder.SanitizeXmlString(xml1));
        dictionary.Add("State", (object) RecordValueSetBuilder.SanitizeXmlString(Enum.GetName(typeof (FormState), (object) record.State)));
        dictionary.Add("Ip", (object) record.IP);
        Guid guid = record.UniqueId;
        dictionary.Add("UniqueId", (object) guid.ToString());
        dictionary.Add("Updated", (object) record.Updated.ToString("s"));
        dictionary.Add("Created", (object) record.Created.ToString("s"));
        guid = record.Form;
        dictionary.Add("Form", (object) guid.ToString());
        dictionary.Add("MemberKey", (object) (record.MemberKey ?? string.Empty));
        dictionary.Add("CurrentPage", (object) record.CurrentPage);
        dictionary.Add("UmbracoPageId", (object) record.UmbracoPageId);
        dictionary.Add("RecordFields", (object) JsonSerializer.Serialize<Dictionary<Guid, RecordField>>(record.RecordFields, FormsJsonSerializerOptions.Default));
        Dictionary<string, object> values = dictionary;
        yield return new ValueSet(record.Id.ToInvariantString(), "UmbracoForms", "Record", (IDictionary<string, object>) values);
      }
      recordArray = (Record[]) null;
    }

    internal static string SanitizeXmlString(string xml)
    {
      StringBuilder stringBuilder = xml != null ? new StringBuilder(xml.Length) : throw new ArgumentNullException(nameof (xml));
      foreach (char character in xml)
      {
        if (RecordValueSetBuilder.IsLegalXmlChar((int) character))
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
