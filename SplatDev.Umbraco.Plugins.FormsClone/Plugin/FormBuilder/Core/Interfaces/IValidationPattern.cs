using System.Text.Json.Serialization;

namespace FormBuilder.Core.Interfaces
{
    public interface IValidationPattern
    {
        [JsonPropertyName("alias")]
        string Alias { get; }

        [JsonPropertyName("labelKey")]
        string LabelKey { get; }

        [JsonPropertyName("name")]
        string Name { get; }

        [JsonPropertyName("pattern")]
        string Pattern { get; }

        [JsonPropertyName("readOnly")]
        bool ReadOnly { get; }
    }
}