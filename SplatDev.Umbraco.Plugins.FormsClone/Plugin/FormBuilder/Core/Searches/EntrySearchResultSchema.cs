using System.Runtime.Serialization;

namespace FormBuilder.Core.Searches
{
    [DataContract(Name = "entrySearchResultSchema")]
    public class EntrySearchResultSchema
    {
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "alias")]
        public string Alias { get; set; } = string.Empty;

        [DataMember(Name = "view")]
        public string View { get; set; } = string.Empty;

        [DataMember(Name = "editView")]
        public string EditView { get; set; } = string.Empty;

        [DataMember(Name = "id")]
        public string Id { get; set; } = string.Empty;

        [DataMember(Name = "containsSensitiveData")]
        public bool ContainsSensitiveData { get; set; }

        [DataMember(Name = "showOnListingScreen")]
        public bool ShowOnListingScreen { get; set; } = true;
    }
}