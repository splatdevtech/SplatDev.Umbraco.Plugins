using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "fieldPrevalue")]
    [Serializable]
    public class FieldPrevalue
    {
        [DataMember(Name = "value")]
        public string Value { get; set; } = string.Empty;

        [DataMember(Name = "caption")]
        public string? Caption { get; set; }
    }
}