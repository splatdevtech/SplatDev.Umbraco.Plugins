using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FormBuilder.Core.Providers.Models
{
    /// <summary>
    /// Defines a mapping class used by     /// through the     /// </summary>
    [DataContract(Name = "documentMapping")]
    public class DocumentMapping
    {
        /// <summary>Gets or sets the property alias.</summary>
        [DataMember(Name = "id")]
        [JsonPropertyName("id")]
        public string Alias { get; set; } = string.Empty;

        /// <summary>Gets or sets the record field.</summary>
        [DataMember(Name = "field")]
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a static value to use instead of a record field defined in         /// </summary>
        [DataMember(Name = "staticValue")]
        public string StaticValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the mapping destinaion has a value.
        /// </summary>
        public bool HasValue => !string.IsNullOrEmpty(Field) || !string.IsNullOrEmpty(StaticValue);
    }
}