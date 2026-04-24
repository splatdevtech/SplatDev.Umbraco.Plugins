using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a result type for an update record operation.</summary>
    [DataContract(Name = "updateRecordResult")]
    [Serializable]
    public class UpdateRecordResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation succeeded.
        /// </summary>
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        /// <summary>Gets or sets the field validation error messages.</summary>
        [DataMember(Name = "fieldMessages")]
        public List<FieldMessage> FieldMessages { get; set; } = [];

        /// <summary>
        /// Defines a field message populated during an update record operation.
        /// </summary>
        [DataContract(Name = "fieldMessage")]
        public class FieldMessage
        {
            /// <summary>Gets or sets a value for the field's Id.</summary>
            [DataMember(Name = "fieldId")]
            public Guid FieldId { get; set; }

            /// <summary>Gets or sets a value for the field's messages.</summary>
            [DataMember(Name = "messages")]
            public List<string> Messages { get; set; } = [];
        }
    }
}