using FormBuilder.Core.Enums;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a view model for a fieldset.</summary>
    [Serializable]
    public class FieldsetViewModel
    {
        /// <summary>Gets or sets the fieldset's Id.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets the fieldset's caption.</summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the fieldset has a condition.
        /// </summary>
        public bool HasCondition { get; set; }

        /// <summary>Gets or sets the fieldset's condition's action type.</summary>
        public FieldConditionActionType ConditionActionType { get; set; }

        /// <summary>Gets or sets the fieldset's condition's logic type.</summary>
        public FieldConditionLogicType ConditionLogicType { get; set; }

        /// <summary>Gets or sets the fieldset's condition's rules.</summary>
        public IEnumerable<FieldConditionRule> ConditionRules { get; set; } = [];

        /// <summary>Gets or sets the fieldset's parent's conditions.</summary>
        public IEnumerable<FieldCondition> ParentConditions { get; set; } = [];

        /// <summary>Gets or sets the fieldset's condition.</summary>
        public FieldCondition? Condition { get; set; }

        /// <summary>Gets or sets the fieldset's containers.</summary>
        public IList<FieldsetContainerViewModel> Containers { get; set; } = [];
    }
}