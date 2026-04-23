using FormBuilder.Core.Enums;
using FormBuilder.Core.Interfaces;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "workflow")]
    public class Workflow : IType
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "created")]
        public DateTime Created { get; set; }

        [DataMember(Name = "form")]
        [JsonPropertyName("form")]
        public Guid Form { get; set; }

        [DataMember(Name = "active")]
        public bool Active { get; set; }

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

        [IgnoreDataMember]
        [JsonIgnore]
        public bool Deleted { get; set; }

        [DataMember(Name = "isMandatory")]
        public bool IsMandatory { get; set; }

        [DataMember(Name = "condition")]
        public FieldCondition? Condition { get; set; }

        public bool ExcludeSensitiveData() => (WorkflowTypeId == Guid.Parse("E96BADD7-05BE-4978-B8D9-B3D733DE70A5") || WorkflowTypeId == Guid.Parse("17C61629-D984-4E86-B43B-A8407B3EFEA9") || WorkflowTypeId == Guid.Parse("616EDFEB-BADF-414B-89DC-D8655EB85998") || WorkflowTypeId == Guid.Parse("CCBFB0D5-ADAA-4729-8B4C-4BB439DC0202") || WorkflowTypeId == Guid.Parse("BC52AB28-D3FF-42EE-AF75-A5D49BE83040")) && IncludeSensitiveData == IncludeSensitiveData.Undefined || IncludeSensitiveData == IncludeSensitiveData.Undefined || IncludeSensitiveData != IncludeSensitiveData.True;
    }
}