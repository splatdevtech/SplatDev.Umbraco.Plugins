using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "form", Namespace = "")]
    [Serializable]
    public class FormEntitySlim : BaseEntitySlim
    {
        [DataMember(Name = "folderId")]
        public Guid? FolderId { get; set; }

        [DataMember(Name = "nodeId")]
        public int NodeId { get; set; }
    }
}