
// Type: Umbraco.Forms.Core.FormDataSourceDefinition
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core
{
    [DataContract(Name = "formDataSourceDefinition")]
    public class FormDataSourceDefinition
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "mappings")]
        public List<FormDataSourceMapping> Mappings { get; set; } = new List<FormDataSourceMapping>();

        public virtual FormDataSourceMapping? GetMapping(string fieldKey)
        {
            IEnumerable<FormDataSourceMapping> source = this.Mappings.Where<FormDataSourceMapping>(map => map.DataFieldKey == fieldKey);
            return !source.Any<FormDataSourceMapping>() ? null : source.First<FormDataSourceMapping>();
        }
    }
}
