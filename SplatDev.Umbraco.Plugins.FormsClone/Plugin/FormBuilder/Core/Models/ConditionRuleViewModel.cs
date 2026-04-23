using FormBuilder.Core.Enums;
using FormBuilder.Core.Services.Interfaces;

using System.Text.Json.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a view model for a condition rule.</summary>
    [Serializable]
    public class ConditionRuleViewModel
    {
        /// <summary>Gets or sets the conditions rule's Id.</summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <summary>Gets or sets the conditions rule's fieldset Id.</summary>
        [JsonPropertyName("fieldsetId")]
        public Guid FieldsetId { get; set; }

        /// <summary>Gets or sets the conditions rule's field Id.</summary>
        [JsonPropertyName("field")]
        public Guid Field { get; set; }

        /// <summary>Gets or sets the conditions rule's operator.</summary>
        [JsonPropertyName("operator")]
        public FieldConditionRuleOperator Operator { get; set; }

        /// <summary>Gets or sets the conditions rule's value.</summary>
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Creates a new instance of the         /// </summary>
        public static ConditionRuleViewModel Build(
          Form form,
          FieldConditionRule rule,
          IPlaceholderParsingService placeholderParsingService)
        {
            return new ConditionRuleViewModel()
            {
                Id = rule.Id,
                Field = rule.Field,
                FieldsetId = FindFieldsetId(form, rule),
                Operator = rule.Operator,
                Value = placeholderParsingService.ParsePlaceHolders(rule.Value, false, form: form)
            };
        }

        private static Guid FindFieldsetId(Form form, FieldConditionRule rule) => form.Pages.SelectMany(p => p.FieldSets).Where(fs => fs.Containers.Any(c => c.Fields.Any(f => f.Id == rule.Field))).Select(fs => fs.Id).FirstOrDefault();
    }
}