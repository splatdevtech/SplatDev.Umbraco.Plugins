using System.Text.Json;
using System.Text.Json.Serialization;

namespace FormBuilder.Core.Helpers
{
    internal sealed class JsonIntToStringConverter : JsonConverter<string?>
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(string);

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