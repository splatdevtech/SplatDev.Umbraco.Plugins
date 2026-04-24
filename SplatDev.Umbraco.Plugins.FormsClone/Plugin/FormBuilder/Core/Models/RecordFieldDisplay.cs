using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "recordFieldDisplay")]
    public class RecordFieldDisplay
    {
        [DataMember(Name = "alias")]
        public string Alias { get; set; } = string.Empty;

        [DataMember(Name = "caption")]
        public string Caption { get; set; } = string.Empty;

        [DataMember(Name = "isSystem")]
        public bool IsSystem { get; set; }
    }
}