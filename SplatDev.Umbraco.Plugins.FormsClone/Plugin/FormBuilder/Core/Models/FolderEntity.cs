using System.Runtime.Serialization;

using Umbraco.Cms.Core.Models.Entities;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "folder")]
    [Serializable]
    public class FolderEntity : EntityBase
    {
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "parentKey")]
        public Guid? ParentKey { get; set; }
    }
}