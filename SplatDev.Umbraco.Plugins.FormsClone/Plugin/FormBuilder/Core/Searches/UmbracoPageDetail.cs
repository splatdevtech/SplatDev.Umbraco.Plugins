using System.Runtime.Serialization;

namespace FormBuilder.Core.Searches
{
    [DataContract(Name = "umbracoPage")]
    [Serializable]
    public class UmbracoPageDetail
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "unique")]
        public Guid Unique { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;
    }
}