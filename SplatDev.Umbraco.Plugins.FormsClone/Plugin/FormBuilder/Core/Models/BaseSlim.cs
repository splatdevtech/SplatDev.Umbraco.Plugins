using FormBuilder.Core.Interfaces;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    public abstract class BaseSlim : IType
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "created")]
        public DateTime Created { get; set; }
    }
}