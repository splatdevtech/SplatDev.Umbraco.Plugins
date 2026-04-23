using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "workflow", Namespace = "")]
    [Serializable]
    public class WorkflowEntitySlim : BaseEntitySlim
    {
        [DataMember(Name = "form")]
        public Guid FormId { get; set; }
    }
}