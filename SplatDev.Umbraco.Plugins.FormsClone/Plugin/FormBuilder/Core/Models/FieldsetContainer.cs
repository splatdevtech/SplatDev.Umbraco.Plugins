using FormBuilder.Core.Interfaces;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "fieldSetContainer")]
    public class FieldsetContainer : IFormObject
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "caption")]
        public string? Caption { get; set; }

        [DataMember(Name = "width")]
        public int Width { get; set; }

        [DataMember(Name = "fields")]
        public List<Field> Fields { get; set; } = [];
    }
}