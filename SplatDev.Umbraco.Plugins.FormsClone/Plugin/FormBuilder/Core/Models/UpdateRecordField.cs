using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>
    /// Defines a representation of field data for a record update command.
    /// </summary>
    [DataContract(Name = "updateRecordField")]
    [Serializable]
    public class UpdateRecordField
    {
        /// <summary>Gets or sets the Id of the field.</summary>
        [DataMember(Name = "fieldId")]
        public Guid FieldId { get; set; }

        /// <summary>Gets or sets the values for the field.</summary>
        [DataMember(Name = "values")]
        public object[] Values { get; set; } = [];
    }
}