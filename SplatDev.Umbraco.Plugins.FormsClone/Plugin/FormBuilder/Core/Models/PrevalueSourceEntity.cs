using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using Umbraco.Cms.Core.Models.Entities;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "preValue", Namespace = "")]
    [Serializable]
    public class PrevalueSourceEntity : EntityBase
    {
        [DataMember(Name = "value")]
        [JsonPropertyName("value")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "createdBy")]
        public int? CreatedBy { get; set; }

        [DataMember(Name = "updatedBy")]
        public int? UpdatedBy { get; set; }

        [DataMember(Name = "settings")]
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        [DataMember(Name = "fieldPreValueSourceTypeId")]
        public Guid FieldPrevalueSourceTypeId { get; set; }

        [DataMember(Name = "cachePrevaluesFor")]
        public TimeSpan CachePrevaluesFor { get; set; } = TimeSpan.FromMilliseconds(-1L, 0L);
    }
}