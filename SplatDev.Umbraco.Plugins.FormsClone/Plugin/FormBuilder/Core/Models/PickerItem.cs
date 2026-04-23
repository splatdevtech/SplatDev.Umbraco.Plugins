using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a picker item representation.</summary>
    [DataContract(Name = "item")]
    [Serializable]
    public class PickerItem
    {
        /// <summary>Gets or sets the Id.</summary>
        [DataMember(Name = "id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets the value.</summary>
        [DataMember(Name = "value")]
        public string Value { get; set; } = string.Empty;
    }
}