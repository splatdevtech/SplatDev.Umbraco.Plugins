using FormBuilder.Core.Enums;
using FormBuilder.Core.Interfaces;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "fieldConditionRule")]
    [Serializable]
    public class FieldConditionRule : IFormObject
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "field")]
        public Guid Field { get; set; }

        [DataMember(Name = "operator")]
        public FieldConditionRuleOperator Operator { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; } = string.Empty;
    }
}