using System.Runtime.Serialization;

using Umbraco.Cms.Core.Models.Entities;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "dataSource")]
    [Serializable]
    public class DataSourceEntity : EntityBase
    {
        public DataSourceEntity() => Settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "createdBy")]
        public int? CreatedBy { get; set; }

        [DataMember(Name = "updatedBy")]
        public int? UpdatedBy { get; set; }

        [DataMember(Name = "settings")]
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        [DataMember(Name = "formDataSourceTypeId")]
        public Guid FormDataSourceTypeId { get; set; }
    }
}