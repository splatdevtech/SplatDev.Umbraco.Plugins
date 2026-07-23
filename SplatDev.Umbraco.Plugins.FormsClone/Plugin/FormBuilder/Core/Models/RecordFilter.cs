using FormBuilder.Core.Enums;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "recordFilter")]
    public class RecordFilter
    {
        internal const int DefaultPageSize = 20;

        [DataMember(Name = "skip")]
        public int Skip { get; set; }

        [DataMember(Name = "take")]
        public int Take { get; set; } = 20;

        [DataMember(Name = "memberKey")]
        public string? MemberKey { get; set; }

        [DataMember(Name = "sortBy")]
        public string SortBy { get; set; } = string.Empty;

        [DataMember(Name = "sortOrder")]
        public RecordSorting SortOrder { get; set; }

        [DataMember(Name = "startDate")]
        public DateTime StartDate { get; set; }

        [DataMember(Name = "endDate")]
        public DateTime EndDate { get; set; }

        [DataMember(Name = "filter")]
        public string? Filter { get; set; }

        [DataMember(Name = "states")]
        public List<FormState> States { get; set; } = [];

        [DataMember(Name = "localTimeOffset")]
        public int LocalTimeOffset { get; set; }

        [DataMember(Name = "recordId")]
        public Guid RecordId { get; set; }
    }
}