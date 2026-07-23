using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a representation of a record action execution.</summary>
    [DataContract(Name = "recordActionExecution")]
    [Serializable]
    public class RecordActionExecution
    {
        /// <summary>Gets or sets the record keys to apply the action to.</summary>
        [DataMember(Name = "recordKeys")]
        public IEnumerable<Guid> RecordKeys { get; set; } = [];
    }
}