using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a result type for a record audit entry.</summary>
    [DataContract(Name = "recordAuditEntry")]
    [Serializable]
    public class RecordAuditEntry
    {
        /// <summary>Gets or sets id of the record audit entry.</summary>
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>Gets or sets the datetime when the record was updated.</summary>
        [DataMember(Name = "updatedOn")]
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the name of the back-office user that updated the record.
        /// </summary>
        [DataMember(Name = "updatedBy")]
        public string UpdatedBy { get; set; } = string.Empty;
    }
}