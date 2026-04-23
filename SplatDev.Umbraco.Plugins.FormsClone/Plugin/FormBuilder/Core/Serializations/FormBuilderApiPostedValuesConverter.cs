using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FormBuilder.Core.Serializations
{
    internal class FormBuilderApiPostedValuesConverter :
      JsonConverter<IDictionary<string, IList<string>>>
    {
        public override IDictionary<string, IList<string>> Read(
          ref Utf8JsonReader reader,
          Type typeToConvert,
          JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException(GetJsonExceptionMessage("Start object token not found"));
            Dictionary<string, IList<string>> dictionary = [];
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return dictionary;
                string key = reader.TokenType == JsonTokenType.PropertyName ? GetFieldAlias(ref reader) : throw new JsonException(GetJsonExceptionMessage("Field alias token not found"));
                reader.Read();
                dictionary[key] = GetFieldValues(ref reader);
            }
            throw new JsonException(GetJsonExceptionMessage("End object token not found"));
        }

        private static string GetJsonExceptionMessage(string message)
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(27, 2);
            interpolatedStringHandler.AppendLiteral("Could not read input in ");
            interpolatedStringHandler.AppendFormatted(nameof(FormBuilderApiPostedValuesConverter));
            interpolatedStringHandler.AppendLiteral(". ");
            interpolatedStringHandler.AppendFormatted(message);
            interpolatedStringHandler.AppendLiteral(".");
            return interpolatedStringHandler.ToStringAndClear();
        }

        private static string GetFieldAlias(ref Utf8JsonReader reader)
        {
            string? str = reader.GetString();
            return !string.IsNullOrEmpty(str) ? str : throw new JsonException(GetJsonExceptionMessage("Field alias value is empty"));
        }

        private static List<string> GetFieldValues(ref Utf8JsonReader reader)
        {
            List<string> fieldValues = [];
            if (reader.TokenType == JsonTokenType.String)
            {
                string str = reader.GetString() ?? string.Empty;
                fieldValues.Add(str);
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                fieldValues = [];
                while (reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        string fieldObjectValue = GetSerializedFieldObjectValue(ref reader);
                        fieldValues.Add(fieldObjectValue);
                    }
                    else if (reader.TokenType == JsonTokenType.String)
                    {
                        string str = reader.GetString() ?? string.Empty;
                        fieldValues.Add(str);
                    }
                    reader.Read();
                }
            }
            return fieldValues;
        }

        private static string GetSerializedFieldObjectValue(ref Utf8JsonReader reader)
        {
            StringBuilder stringBuilder = new('{');
            bool flag = false;
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    if (flag)
                        stringBuilder.Append(", ");
                    stringBuilder.Append('"').Append(reader.GetString() ?? string.Empty).Append('"').Append(": ");
                }
                else if (reader.TokenType == JsonTokenType.String)
                {
                    stringBuilder.Append('"').Append(reader.GetString() ?? string.Empty).Append('"');
                    flag = true;
                }
                reader.Read();
            }
            stringBuilder.Append('}');
            return stringBuilder.ToString();
        }

        public override void Write(
          Utf8JsonWriter writer,
          IDictionary<string, IList<string>> value,
          JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}