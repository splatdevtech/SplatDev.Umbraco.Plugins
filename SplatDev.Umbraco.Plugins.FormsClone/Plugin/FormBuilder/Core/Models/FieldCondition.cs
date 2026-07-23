using FormBuilder.Core.Enums;
using FormBuilder.Core.Interfaces;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "fieldCondition")]
    [Serializable]
    public class FieldCondition : IFormObject
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }

        [DataMember(Name = "actionType")]
        public FieldConditionActionType ActionType { get; set; }

        [DataMember(Name = "logicType")]
        public FieldConditionLogicType LogicType { get; set; }

        [DataMember(Name = "rules")]
        public IEnumerable<FieldConditionRule> Rules { get; set; } = [];
    }
}