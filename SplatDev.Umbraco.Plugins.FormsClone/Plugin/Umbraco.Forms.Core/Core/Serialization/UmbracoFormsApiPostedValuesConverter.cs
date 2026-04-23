
// Type: Umbraco.Forms.Core.Serialization.UmbracoFormsApiPostedValuesConverter
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Serialization
{
  internal class UmbracoFormsApiPostedValuesConverter : 
    JsonConverter<IDictionary<string, IList<string>>>
  {
    public override IDictionary<string, IList<string>> Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options)
    {
      if (reader.TokenType != JsonTokenType.StartObject)
        throw new JsonException(UmbracoFormsApiPostedValuesConverter.GetJsonExceptionMessage("Start object token not found"));
      Dictionary<string, IList<string>> dictionary = new Dictionary<string, IList<string>>();
      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndObject)
          return (IDictionary<string, IList<string>>) dictionary;
        string key = reader.TokenType == JsonTokenType.PropertyName ? UmbracoFormsApiPostedValuesConverter.GetFieldAlias(ref reader) : throw new JsonException(UmbracoFormsApiPostedValuesConverter.GetJsonExceptionMessage("Field alias token not found"));
        reader.Read();
        dictionary[key] = (IList<string>) UmbracoFormsApiPostedValuesConverter.GetFieldValues(ref reader);
      }
      throw new JsonException(UmbracoFormsApiPostedValuesConverter.GetJsonExceptionMessage("End object token not found"));
    }

    private static string GetJsonExceptionMessage(string message)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 2);
      interpolatedStringHandler.AppendLiteral("Could not read input in ");
      interpolatedStringHandler.AppendFormatted(nameof (UmbracoFormsApiPostedValuesConverter));
      interpolatedStringHandler.AppendLiteral(". ");
      interpolatedStringHandler.AppendFormatted(message);
      interpolatedStringHandler.AppendLiteral(".");
      return interpolatedStringHandler.ToStringAndClear();
    }

    private static string GetFieldAlias(ref Utf8JsonReader reader)
    {
      string str = reader.GetString();
      return !string.IsNullOrEmpty(str) ? str : throw new JsonException(UmbracoFormsApiPostedValuesConverter.GetJsonExceptionMessage("Field alias value is empty"));
    }

    private static List<string> GetFieldValues(ref Utf8JsonReader reader)
    {
      List<string> fieldValues = new List<string>();
      if (reader.TokenType == JsonTokenType.String)
      {
        string str = reader.GetString() ?? string.Empty;
        fieldValues.Add(str);
      }
      else if (reader.TokenType == JsonTokenType.StartArray)
      {
        fieldValues = new List<string>();
        while (reader.TokenType != JsonTokenType.EndArray)
        {
          if (reader.TokenType == JsonTokenType.StartObject)
          {
            string fieldObjectValue = UmbracoFormsApiPostedValuesConverter.GetSerializedFieldObjectValue(ref reader);
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
      StringBuilder stringBuilder = new StringBuilder("{");
      bool flag = false;
      while (reader.TokenType != JsonTokenType.EndObject)
      {
        if (reader.TokenType == JsonTokenType.PropertyName)
        {
          if (flag)
            stringBuilder.Append(", ");
          stringBuilder.Append("\"").Append(reader.GetString() ?? string.Empty).Append("\"").Append(": ");
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
          stringBuilder.Append("\"").Append(reader.GetString() ?? string.Empty).Append("\"");
          flag = true;
        }
        reader.Read();
      }
      stringBuilder.Append("}");
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
