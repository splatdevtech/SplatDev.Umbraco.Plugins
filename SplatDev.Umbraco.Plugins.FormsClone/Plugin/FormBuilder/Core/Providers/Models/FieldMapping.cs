using System.Runtime.Serialization;

namespace FormBuilder.Core.Providers.Models
{
    /// Defines a mapping class used in the     /// </summary>
    [DataContract(Name = "fieldMapping")]
    public class FieldMapping
    {
        /// <summary>Gets or sets the alias for the field mapping.</summary>
        [DataMember(Name = "alias")]
        public string Alias { get; set; } = string.Empty;

        /// <summary>Gets or sets the value to use for the field mapping.</summary>
        [DataMember(Name = "value")]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a static value to use for the field mapping, in preference to the dynamic value provided by         /// </summary>
        [DataMember(Name = "staticValue")]
        public string StaticValue { get; set; } = string.Empty;
    }
}