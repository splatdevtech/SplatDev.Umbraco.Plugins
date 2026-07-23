
// Type: Umbraco.Forms.Core.Data.Helpers.JsonIntToStringConverter
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Text.Json;
using System.Text.Json.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Data.Helpers
{
  internal sealed class JsonIntToStringConverter : JsonConverter<string?>
  {
    public override bool CanConvert(Type objectType) => objectType == typeof (string);

    public override string? Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options)
    {
      return reader.TokenType == JsonTokenType.Number ? reader.GetInt32().ToString() : reader.GetString();
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options) => writer.WriteStringValue(value?.ToString());
  }
}
