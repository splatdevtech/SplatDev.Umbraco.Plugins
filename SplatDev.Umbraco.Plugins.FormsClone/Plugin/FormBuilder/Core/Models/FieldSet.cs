using FormBuilder.Core.Interfaces;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "fieldSet")]
    [Serializable]
    public class FieldSet : IFormObject, IConditioned
    {
        [DataMember(Name = "caption")]
        public string? Caption { get; set; }

        [DataMember(Name = "sortOrder")]
        public int SortOrder { get; set; }

        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "page")]
        public Guid Page { get; set; }

        [DataMember(Name = "containers")]
        public List<FieldsetContainer> Containers { get; set; } = [];

        [DataMember(Name = "condition")]
        public FieldCondition? Condition { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        [JsonIgnore]
        public List<Field> AllFields
        {
            get
            {
                List<Field> allFields = [];
                foreach (FieldsetContainer container in Containers)
                {
                    foreach (Field field in container.Fields)
                        allFields.Add(field);
                }
                return allFields;
            }
        }
    }
}