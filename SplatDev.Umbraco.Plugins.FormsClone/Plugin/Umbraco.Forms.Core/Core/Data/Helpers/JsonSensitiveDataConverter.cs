
// Type: Umbraco.Forms.Core.Data.Helpers.JsonSensitiveDataConverter
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Data.Helpers
{
  internal sealed class JsonSensitiveDataConverter : JsonConverter<IncludeSensitiveData>
  {
    public override bool CanConvert(Type objectType) => objectType == typeof (IncludeSensitiveData);

    public override IncludeSensitiveData Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options)
    {
      if (reader.TokenType == JsonTokenType.String)
      {
        string str = reader.GetString();
        if (string.IsNullOrEmpty(str))
          return IncludeSensitiveData.Undefined;
        bool result;
        if (bool.TryParse(str, out result))
          return !result ? IncludeSensitiveData.False : IncludeSensitiveData.True;
        if (str == "True")
          return IncludeSensitiveData.True;
        if (str == "False")
          return IncludeSensitiveData.False;
      }
      else
      {
        if (reader.TokenType == JsonTokenType.False)
          return IncludeSensitiveData.False;
        if (reader.TokenType == JsonTokenType.True)
          return IncludeSensitiveData.True;
      }
      return IncludeSensitiveData.Undefined;
    }

    public override void Write(
      Utf8JsonWriter writer,
      IncludeSensitiveData value,
      JsonSerializerOptions options)
    {
      writer.WriteStringValue(value.ToString());
    }
  }
}
