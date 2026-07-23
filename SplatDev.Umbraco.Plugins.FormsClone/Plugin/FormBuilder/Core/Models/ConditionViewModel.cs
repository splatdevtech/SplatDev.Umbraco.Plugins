using FormBuilder.Core.Enums;
using FormBuilder.Core.Services.Interfaces;

using System.Text.Json.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a view model for a condition.</summary>
    [Serializable]
    public class ConditionViewModel
    {
        /// <summary>Gets or sets the condition's Id.</summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <summary>Gets or sets the condition's action type.</summary>
        [JsonPropertyName("actionType")]
        public FieldConditionActionType ActionType { get; set; }

        /// <summary>Gets or sets the condition's logic type.</summary>
        [JsonPropertyName("logicType")]
        public FieldConditionLogicType LogicType { get; set; }

        /// <summary>Gets or sets the condition's rules.</summary>
        [JsonPropertyName("rules")]
        public IEnumerable<ConditionRuleViewModel> Rules { get; set; } = [];

        /// <summary>
        /// Creates a new instance of the         /// </summary>
        public static ConditionViewModel Build(
          Form form,
          FieldCondition condition,
          IPlaceholderParsingService placeholderParsingService)
        {
            return new ConditionViewModel()
            {
                Id = condition.Id,
                ActionType = condition.ActionType,
                LogicType = condition.LogicType,
                Rules = condition.Rules.Select(rule => ConditionRuleViewModel.Build(form, rule, placeholderParsingService))
            };
        }
    }
}