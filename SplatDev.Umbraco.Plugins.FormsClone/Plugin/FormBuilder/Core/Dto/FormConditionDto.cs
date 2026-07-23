using FormBuilder.Core.Enums;

namespace FormBuilder.Core.Dto
{
    public class FormConditionDto
    {
        public FieldConditionActionType ActionType { get; set; }

        public FieldConditionLogicType LogicType { get; set; }

        public IEnumerable<FormConditionRuleDto> Rules { get; set; } = [];
    }
}