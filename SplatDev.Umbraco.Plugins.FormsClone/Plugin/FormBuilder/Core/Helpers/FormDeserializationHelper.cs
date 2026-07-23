using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Options;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace FormBuilder.Core.Helpers
{
    internal static class FormDeserializationHelper
    {
        public static Form? DeserializeForm(string json) => JsonSerializer.Deserialize<Form>(json, FormsJsonSerializerOptions.Default);

        public static IDictionary<FormState, IEnumerable<Workflow>> DeserializeWorkflows(
          string json)
        {
            Dictionary<FormState, IEnumerable<Workflow>>? workflows = [];
            if (JsonNode.Parse(json)?["formWorkflows"] is not JsonObject workflowsAsJsonObject)
                return workflows;
            LoadWorkflowsForState(workflows, FormState.Submitted, workflowsAsJsonObject, "onSubmit");
            LoadWorkflowsForState(workflows, FormState.Approved, workflowsAsJsonObject, "onApprove");
            return workflows;
        }

        private static void LoadWorkflowsForState(
          Dictionary<FormState, IEnumerable<Workflow>> workflows,
          FormState formState,
          JsonObject workflowsAsJsonObject,
          string key)
        {
            if (workflowsAsJsonObject[key] is not JsonArray source)
            {
                workflows.Add(formState, []);
            }
            else
            {
                JsonSerializerOptions? serializationOptions = FormsJsonSerializerOptions.Default;
                serializationOptions.Converters.Add(new WorkflowSettingsFromTemplateConverter());
                workflows.Add(formState, source.Select(x => x.Deserialize<Workflow>(serializationOptions))?.Where(x => x is not null)?.Select(x => x)!);
            }
        }

        private class WorkflowSettingsFromTemplateConverter : JsonConverter<Dictionary<string, string>>
        {
            public override Dictionary<string, string>? Read(
              ref Utf8JsonReader reader,
              Type typeToConvert,
              JsonSerializerOptions options)
            {
                Dictionary<string, string>? dictionary = [];
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    while (reader.TokenType != JsonTokenType.EndArray)
                    {
                        if (reader.TokenType == JsonTokenType.StartObject)
                        {
                            string? empty = string.Empty;
                            string? key = string.Empty;
                            string? str1 = string.Empty;
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
                                    string? str2 = reader.GetString() ?? string.Empty;
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
                JsonSerializer.Serialize(writer, value);
            }

            public override bool CanConvert(Type objectType) => objectType == typeof(Dictionary<string, string>);
        }
    }
}