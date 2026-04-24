using FormBuilder.Core.Interfaces;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Prevalues
{
    [DataContract(Name = "fieldPreValueSource")]
    public class FieldPrevalueSource : ITypeWithEditorDetails, IType
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "unique")]
        public Guid Unique => Id;

        [DataMember(Name = "entityType")]
        public static string EntityType => "prevaluesource";

        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "created")]
        public DateTime Created { get; set; }

        [DataMember(Name = "createdBy")]
        public int? CreatedBy { get; set; }

        [DataMember(Name = "createdByName")]
        public string? CreatedByName { get; set; }

        [DataMember(Name = "updated")]
        public DateTime Updated { get; set; }

        [DataMember(Name = "updatedBy")]
        public int? UpdatedBy { get; set; }

        [DataMember(Name = "updatedByName")]
        public string? UpdatedByName { get; set; }

        [DataMember(Name = "settings")]
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        [DataMember(Name = "fieldPreValueSourceTypeId")]
        public Guid FieldPrevalueSourceTypeId { get; set; }

        [DataMember(Name = "cachePrevaluesFor")]
        public TimeSpan CachePrevaluesFor { get; set; } = TimeSpan.FromMilliseconds(-1L, 0L);
    }
}