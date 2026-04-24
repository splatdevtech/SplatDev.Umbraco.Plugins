using FormBuilder.Core.Mapping;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Definitions
{
    [DataContract(Name = "formDataSourceDefinition")]
    public class FormDataSourceDefinition
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "mappings")]
        public List<FormDataSourceMapping> Mappings { get; set; } = [];

        public virtual FormDataSourceMapping? GetMapping(string fieldKey)
        {
            IEnumerable<FormDataSourceMapping> source = Mappings.Where(map => map.DataFieldKey == fieldKey);
            return !source.Any() ? null : source.First();
        }
    }
}