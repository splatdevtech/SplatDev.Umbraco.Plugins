using System.Text.Json;
using System.Text.Json.Serialization;

namespace FormBuilder.Core.Serializations
{
    internal class FormBuilderApiSettingsConverter : JsonConverter<IDictionary<string, string>>
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
            JsonSerializerOptions jsonSerializerOptions = new(options)
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            };
            JsonSerializerOptions optionsAdjusted = jsonSerializerOptions;
            JsonSerializer.Serialize(writer, value, optionsAdjusted);
        }
    }
}