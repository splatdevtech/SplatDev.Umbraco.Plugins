using FormBuilder.Core.Enums;
using FormBuilder.Core.Helpers;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using Umbraco.Cms.Core.Models.Entities;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "workflow", Namespace = "")]
    [Serializable]
    public class WorkflowEntity : EntityBase
    {
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "form")]
        [JsonPropertyName("form")]
        public Guid FormId { get; set; }

        [DataMember(Name = "active")]
        public bool Active { get; set; }

        [JsonConverter(typeof(JsonSensitiveDataConverter))]
        [DataMember(Name = "includeSensitiveData")]
        public IncludeSensitiveData IncludeSensitiveData { get; set; } = IncludeSensitiveData.Undefined;

        [DataMember(Name = "workflowTypeId")]
        public Guid WorkflowTypeId { get; set; }

        [DataMember(Name = "executesOn")]
        public FormState ExecutesOn { get; set; }

        [DataMember(Name = "settings")]
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        [DataMember(Name = "sortOrder")]
        public int SortOrder { get; set; }

        [DataMember(Name = "isMandatory")]
        public bool IsMandatory { get; set; }

        [DataMember(Name = "condition")]
        public FieldCondition? Condition { get; set; }
    }
}