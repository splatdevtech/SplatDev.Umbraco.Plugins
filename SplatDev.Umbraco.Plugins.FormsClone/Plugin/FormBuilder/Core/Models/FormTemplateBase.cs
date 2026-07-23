using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "formTemplateBase")]
    public class FormTemplateBase
    {
        [DataMember(Name = "alias")]
        public string Alias { get; set; } = string.Empty;

        [DataMember(Name = "unique")]
        public string Unique => Alias;

        [DataMember(Name = "entityType")]
        public static string EntityType => "form-template";

        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "description")]
        public string Description { get; set; } = string.Empty;
    }
}