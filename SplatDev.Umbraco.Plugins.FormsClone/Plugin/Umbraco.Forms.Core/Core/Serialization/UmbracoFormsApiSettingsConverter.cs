
// Type: Umbraco.Forms.Core.Serialization.UmbracoFormsApiSettingsConverter
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Serialization
{
  internal class UmbracoFormsApiSettingsConverter : JsonConverter<IDictionary<string, string>>
  {
    public override bool CanConvert(Type typeToConvert) => true;

    public override IDictionary<string, string> Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options)
    {
      throw new NotSupportedException();
    }

    public override void Write(
      Utf8JsonWriter writer,
      IDictionary<string, string> value,
      JsonSerializerOptions options)
    {
      JsonSerializer.Serialize<IDictionary<string, string>>(writer, value, new JsonSerializerOptions(options)
      {
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
      });
    }
  }
}
