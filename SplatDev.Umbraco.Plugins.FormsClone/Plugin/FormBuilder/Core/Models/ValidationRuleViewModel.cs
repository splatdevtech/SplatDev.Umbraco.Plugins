using Newtonsoft.Json;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a view model for a validation rule.</summary>
    [Serializable]
    public class ValidationRuleViewModel
    {
        /// <summary>Gets or sets the rule definition.</summary>
        [JsonProperty("rule")]
        public string Rule { get; set; } = string.Empty;

        /// <summary>Gets or sets the rule's validation message.</summary>
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the field to which the validation message will be associated.
        /// </summary>
        [JsonProperty("fieldId")]
        public Guid FieldId { get; set; }
    }
}