
// Type: Umbraco.Forms.Core.FieldPrevalueJsonConverter
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core
{
  internal class FieldPrevalueJsonConverter : JsonConverter<FieldPrevalue>
  {
    public override FieldPrevalue? Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options)
    {
      if (reader.TokenType == JsonTokenType.String)
      {
        string str = reader.GetString() ?? string.Empty;
        if (str.Contains("~|~"))
        {
          string[] strArray = str.Split(new string[1]
          {
            "~|~"
          }, StringSplitOptions.RemoveEmptyEntries);
          return new FieldPrevalue()
          {
            Value = strArray[0],
            Caption = strArray[1]
          };
        }
        return new FieldPrevalue() { Value = str };
      }
      FieldPrevalue fieldPrevalue = new FieldPrevalue();
      string empty = string.Empty;
      while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
      {
        if (reader.TokenType == JsonTokenType.PropertyName)
          empty = reader.GetString();
        if (reader.TokenType == JsonTokenType.String)
        {
          string str = reader.GetString() ?? string.Empty;
          if (!(empty == "value"))
          {
            if (empty == "caption")
              fieldPrevalue.Caption = str;
          }
          else
            fieldPrevalue.Value = str;
        }
      }
      return fieldPrevalue;
    }

    public override void Write(
      Utf8JsonWriter writer,
      FieldPrevalue value,
      JsonSerializerOptions options)
    {
      writer.WriteStartObject();
      writer.WritePropertyName(nameof (value));
      writer.WriteStringValue(value.Value);
      writer.WritePropertyName("caption");
      writer.WriteStringValue(value.Caption ?? string.Empty);
      writer.WriteEndObject();
    }
  }
}
