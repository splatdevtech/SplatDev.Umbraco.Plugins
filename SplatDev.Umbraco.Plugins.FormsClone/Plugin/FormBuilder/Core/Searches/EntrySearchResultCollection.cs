using System.Runtime.Serialization;

namespace FormBuilder.Core.Searches
{
    [DataContract(Name = "entrySearchResultCollection")]
    public class EntrySearchResultCollection
    {
        [DataMember(Name = "totalNumberOfResults")]
        public long TotalNumberOfResults { get; set; }

        [DataMember(Name = "totalNumberOfPages")]
        public long TotalNumberOfPages { get; set; }

        [DataMember(Name = "schema")]
        public IEnumerable<EntrySearchResultSchema> Schema { get; set; } = [];

        [DataMember(Name = "results")]
        public IEnumerable<EntrySearchResult> Results { get; set; } = [];
    }
}