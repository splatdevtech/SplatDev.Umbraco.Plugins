using FormBuilder.Core.Interfaces;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "field")]
    [Serializable]
    public class Field : IFormObject, IConditioned
    {
        [XmlIgnore]
        [IgnoreDataMember]
        [JsonIgnore]
        public List<object> Values { get; set; } = [];

        [DataMember(Name = "caption")]
        public string Caption { get; set; } = string.Empty;

        [DataMember(Name = "tooltip")]
        [JsonPropertyName("tooltip")]
        public string? ToolTip { get; set; }

        [DataMember(Name = "cssClass")]
        public string? CssClass { get; set; }

        [DataMember(Name = "alias")]
        public string Alias { get; set; } = string.Empty;

        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "fieldTypeId")]
        public Guid FieldTypeId { get; set; }

        [DataMember(Name = "prevalueSourceId")]
        [JsonPropertyName("prevalueSourceId")]
        public Guid PreValueSourceId { get; set; }

        [DataMember(Name = "dataSourceFieldKey")]
        public string? DataSourceFieldKey { get; set; }

        [DataMember(Name = "containsSensitiveData")]
        public bool ContainsSensitiveData { get; set; }

        [DataMember(Name = "mandatory")]
        public bool Mandatory { get; set; }

        [DataMember(Name = "regex")]
        [JsonPropertyName("regex")]
        public string? RegEx { get; set; }

        [DataMember(Name = "requiredErrorMessage")]
        public string? RequiredErrorMessage { get; set; }

        [DataMember(Name = "invalidErrorMessage")]
        public string? InvalidErrorMessage { get; set; }

        [DataMember(Name = "condition")]
        public FieldCondition? Condition { get; set; }

        [DataMember(Name = "settings")]
        public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        [DataMember(Name = "preValues")]
        public IEnumerable<FieldPrevalue> PreValues { get; set; } = [];

        [DataMember(Name = "allowedUploadTypes")]
        public IEnumerable<AllowedUploadType>? AllowedUploadTypes { get; set; }

        [DataMember(Name = "allowMultipleFileUploads")]
        public bool AllowMultipleFileUploads { get; set; } = true;
    }
}