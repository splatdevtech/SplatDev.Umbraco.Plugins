using System.Text.Json.Serialization;

namespace SplatDev.Umbraco.Common.Models
{
    public class SimpleSelectListItem
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
        [JsonPropertyName("alias")]
        public string Alias { get; set; } = string.Empty;
        [JsonPropertyName("value")]
        public object Value { get; set; } = new();
        [JsonPropertyName("group")]
        public string? Group { get; set; }
    }

    public class SimpleSelectListItemInteger
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
        [JsonPropertyName("alias")]
        public string Alias { get; set; } = string.Empty;
        [JsonPropertyName("value")]
        public int Value { get; set; } = 0;
        [JsonPropertyName("group")]
        public string? Group { get; set; }
    }
}
