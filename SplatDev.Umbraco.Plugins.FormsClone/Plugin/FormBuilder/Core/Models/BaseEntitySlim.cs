using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    public abstract class BaseEntitySlim
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "key")]
        public Guid Key { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "createDate")]
        public DateTime CreateDate { get; set; }
    }
}