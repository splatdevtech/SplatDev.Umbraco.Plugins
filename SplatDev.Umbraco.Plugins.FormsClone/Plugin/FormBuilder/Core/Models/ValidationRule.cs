using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "validationRule")]
    [Serializable]
    public class ValidationRule
    {
        [DataMember(Name = "rule")]
        public string Rule { get; set; } = string.Empty;

        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; } = string.Empty;

        [DataMember(Name = "fieldId")]
        public Guid FieldId { get; set; }
    }
}