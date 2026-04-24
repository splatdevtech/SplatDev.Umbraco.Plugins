using FormBuilder.Core.JsonConverters;

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FormBuilder.Core.Options
{
    public static class FormsJsonSerializerOptions
    {
        public static JsonSerializerOptions Default
        {
            get
            {
                JsonSerializerOptions serializerOptions = new()
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                serializerOptions.Converters.Add(new JsonStringEnumConverter());
                serializerOptions.Converters.Add(new FieldPrevalueJsonConverter());
                return serializerOptions;
            }
        }
    }
}