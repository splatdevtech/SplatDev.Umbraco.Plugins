using System.Text.Json.Serialization;

namespace FormBuilder.Core.Dto
{
    public class FormFileUploadDto
    {
        [JsonPropertyName("fileName")]
        public string? FileName { get; set; }

        [JsonPropertyName("fileContents")]
        public string? FileContents { get; set; }
    }
}