using FormBuilder.Core.Serializations;

using System.Text.Json.Serialization;

namespace FormBuilder.Core.Dto
{
    public class FormFieldDto
    {
        public Guid Id { get; set; }

        public string Caption { get; set; } = string.Empty;

        public string? HelpText { get; set; }

        public string? CssClass { get; set; }

        public string Alias { get; set; } = string.Empty;

        public bool Required { get; set; }

        public string? RequiredErrorMessage { get; set; }

        public string? Pattern { get; set; }

        public string? PatternInvalidErrorMessage { get; set; }

        public FormConditionDto? Condition { get; set; }

        public FormFileUploadOptionsDto? FileUploadOptions { get; set; }

        public IEnumerable<FormFieldPrevalueDto> PreValues { get; set; } = [];

        [JsonConverter(typeof(FormBuilderApiSettingsConverter))]
        public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public FormFieldTypeDto Type { get; set; } = new FormFieldTypeDto();
    }
}