
// Type: Umbraco.Forms.Core.FormsJsonSerializerOptions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;


#nullable enable
namespace Umbraco.Forms.Core
{
  public static class FormsJsonSerializerOptions
  {
    public static JsonSerializerOptions Default
    {
      get
      {
        JsonSerializerOptions serializerOptions = new JsonSerializerOptions();
        serializerOptions.PropertyNameCaseInsensitive = true;
        serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        serializerOptions.Converters.Add((JsonConverter) new JsonStringEnumConverter());
        serializerOptions.Converters.Add((JsonConverter) new FieldPrevalueJsonConverter());
        return serializerOptions;
      }
    }
  }
}
