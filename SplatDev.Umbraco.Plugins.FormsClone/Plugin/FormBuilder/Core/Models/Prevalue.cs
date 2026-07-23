using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "preValue")]
    public class Prevalue
    {
        [DataMember(Name = "id")]
        public string Id { get; set; } = string.Empty;

        [DataMember(Name = "value")]
        public string Value { get; set; } = string.Empty;

        [DataMember(Name = "caption")]
        public string? Caption { get; set; }

        [DataMember(Name = "sortOrder")]
        public int SortOrder { get; set; }
    }
}