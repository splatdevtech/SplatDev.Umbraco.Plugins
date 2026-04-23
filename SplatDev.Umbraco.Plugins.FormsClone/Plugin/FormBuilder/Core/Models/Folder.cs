using FormBuilder.Core.Interfaces;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "folder", Namespace = "")]
    [Serializable]
    public class Folder : IType
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [Required]
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "created")]
        public DateTime Created { get; set; }

        [DataMember(Name = "parentId")]
        public Guid? ParentId { get; set; }
    }
}