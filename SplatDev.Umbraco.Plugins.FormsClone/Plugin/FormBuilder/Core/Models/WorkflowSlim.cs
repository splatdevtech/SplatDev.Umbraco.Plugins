using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "workflow", Namespace = "")]
    [Serializable]
    public class WorkflowSlim : BaseSlim
    {
        [DataMember(Name = "form")]
        public Guid Form { get; set; }
    }
}