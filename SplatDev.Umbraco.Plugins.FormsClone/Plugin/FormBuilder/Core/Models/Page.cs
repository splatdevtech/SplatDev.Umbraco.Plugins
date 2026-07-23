using FormBuilder.Core.Interfaces;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "page")]
    [Serializable]
    public class Page : IFormObject
    {
        [DataMember(Name = "fieldSets")]
        public List<FieldSet> FieldSets { get; set; } = [];

        [DataMember(Name = "caption")]
        public string? Caption { get; set; }

        [DataMember(Name = "sortOrder")]
        public int SortOrder { get; set; }

        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "form")]
        public Guid Form { get; set; }

        [DataMember(Name = "buttonCondition")]
        public FieldCondition? ButtonCondition { get; set; }

        public IEnumerable<Field> AllFields() => FieldSets.SelectMany(x => x.Containers).SelectMany(x => x.Fields);
    }
}