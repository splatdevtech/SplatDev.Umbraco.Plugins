using FormBuilder.Core.Serializations;

using System.Text.Json.Serialization;

namespace FormBuilder.Core.Dto
{
    public class FormEntryDto
    {
        [JsonPropertyName("values")]
        [JsonConverter(typeof(FormBuilderApiPostedValuesConverter))]
        public IDictionary<string, IList<string>> Values { get; set; } = new Dictionary<string, IList<string>>();

        [JsonPropertyName("contentId")]
        public string? ContentId { get; set; }

        [JsonPropertyName("culture")]
        public string? Culture { get; set; }

        [JsonPropertyName("additionalData")]
        public IDictionary<string, string?>? AdditionalData { get; set; }
    }
}