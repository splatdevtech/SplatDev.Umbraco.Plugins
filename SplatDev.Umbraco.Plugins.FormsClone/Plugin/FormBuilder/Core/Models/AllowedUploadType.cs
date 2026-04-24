using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    public class AllowedUploadType
    {
        [DataMember(Name = "type")]
        public string Type { get; set; } = string.Empty;

        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "checked")]
        public string Checked { get; set; } = string.Empty;
    }
}