using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "form", Namespace = "")]
    [Serializable]
    public class FormSlim : BaseSlim
    {
        [DataMember(Name = "folderId")]
        public Guid? FolderId { get; set; }
    }
}