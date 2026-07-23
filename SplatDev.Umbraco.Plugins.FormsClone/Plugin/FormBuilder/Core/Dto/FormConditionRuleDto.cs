using FormBuilder.Core.Enums;

namespace FormBuilder.Core.Dto
{
    public class FormConditionRuleDto
    {
        public string Field { get; set; } = string.Empty;

        public FieldConditionRuleOperator Operator { get; set; }

        public string Value { get; set; } = string.Empty;
    }
}