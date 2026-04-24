using System.Runtime.Serialization;

namespace FormBuilder.Core.Searches
{
    [DataContract(Name = "entrySearchResultMetadata")]
    public class EntrySearchResultMetadata
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "lastSubmittedDate")]
        public string LastSubmittedDate { get; set; } = string.Empty;
    }
}