using System.Runtime.Serialization;

namespace FormBuilder.Core.Providers.Models
{
    /// <summary>
    /// Defines a mapping class used in the     /// </summary>
    [DataContract(Name = "standardFieldMapping")]
    public class StandardFieldMapping
    {
        /// <summary>Gets or sets the field for the standard field mapping.</summary>
        [DataMember(Name = "field")]
        public StandardField Field { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include the field in the output.
        /// </summary>
        [DataMember(Name = "include")]
        public bool Include { get; set; }

        /// <summary>
        /// Gets or sets a value to use for the key name when sending the value.
        /// </summary>
        [DataMember(Name = "keyName")]
        public string KeyName { get; set; } = string.Empty;
    }
}