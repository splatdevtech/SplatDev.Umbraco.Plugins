
// Type: Umbraco.Forms.Core.Data.Storage.FormDeserializationHelper
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Data.Storage
{
  internal static class FormDeserializationHelper
  {
    public static Form? DeserializeForm(string json) => JsonSerializer.Deserialize<Form>(json, FormsJsonSerializerOptions.Default);

    public static IDictionary<FormState, IEnumerable<Workflow>> DeserializeWorkflows(
      string json)
    {
      Dictionary<FormState, IEnumerable<Workflow>> workflows = new Dictionary<FormState, IEnumerable<Workflow>>();
      if (!(JsonNode.Parse(json)?["formWorkflows"] is JsonObject workflowsAsJsonObject))
        return (IDictionary<FormState, IEnumerable<Workflow>>) workflows;
      FormDeserializationHelper.LoadWorkflowsForState(workflows, FormState.Submitted, workflowsAsJsonObject, "onSubmit");
      FormDeserializationHelper.LoadWorkflowsForState(workflows, FormState.Approved, workflowsAsJsonObject, "onApprove");
      return (IDictionary<FormState, IEnumerable<Workflow>>) workflows;
    }

    private static void LoadWorkflowsForState(
      Dictionary<FormState, IEnumerable<Workflow>> workflows,
      FormState formState,
      JsonObject workflowsAsJsonObject,
      string key)
    {
      if (!(workflowsAsJsonObject[key] is JsonArray source))
      {
        workflows.Add(formState, Enumerable.Empty<Workflow>());
      }
      else
      {
        JsonSerializerOptions serializationOptions = FormsJsonSerializerOptions.Default;
        serializationOptions.Converters.Add((JsonConverter) new FormDeserializationHelper.WorkflowSettingsFromTemplateConverter());
        workflows.Add(formState, source.Select<JsonNode, Workflow>((Func<JsonNode, Workflow>) (x => x.Deserialize<Workflow>(serializationOptions))).Where<Workflow>((Func<Workflow, bool>) (x => x != null)).Select<Workflow, Workflow>((Func<Workflow, Workflow>) (x => x)));
      }
    }

    private class WorkflowSettingsFromTemplateConverter : JsonConverter<Dictionary<string, string>>
    {
      public override Dictionary<string, string>? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        if (reader.TokenType == JsonTokenType.StartArray)
        {
          while (reader.TokenType != JsonTokenType.EndArray)
          {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
              string empty = string.Empty;
              string key = string.Empty;
              string str1 = string.Empty;
              while (reader.Read())
              {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                  if (!string.IsNullOrEmpty(key))
                  {
                    dictionary.Add(key, str1);
                    break;
                  }
                  break;
                }
                if (reader.TokenType == JsonTokenType.PropertyName)
                  empty = reader.GetString();
                if (reader.TokenType == JsonTokenType.String)
                {
                  string str2 = reader.GetString() ?? string.Empty;
                  if (!(empty == "alias"))
                  {
                    if (empty == "value")
                      str1 = str2;
                  }
                  else
                    key = str2;
                }
              }
            }
            reader.Read();
          }
        }
        return dictionary;
      }

      public override void Write(
        Utf8JsonWriter writer,
        Dictionary<string, string> value,
        JsonSerializerOptions options)
      {
        JsonSerializer.Serialize<Dictionary<string, string>>(writer, value);
      }

      public override bool CanConvert(Type objectType) => objectType == typeof (Dictionary<string, string>);
    }
  }
}
