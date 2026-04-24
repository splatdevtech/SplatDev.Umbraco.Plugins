using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FormBuilder.Core.Providers.Models
{
    /// <summary>
    /// Defines a mapping class used in the     /// </summary>
    [DataContract(Name = "documentMapper")]
    public class DocumentMapper
    {
        /// <summary>Gets or sets the document type alias.</summary>
        [DataMember(Name = "doctype")]
        [JsonPropertyName("doctype")]
        public string DocTypeAlias { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the field to use for the document name when mapping.
        /// </summary>
        [DataMember(Name = "nameField")]
        public string NameField { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the a static value that can be used for the document name in preference to a field defined in         /// </summary>
        [DataMember(Name = "nameStaticValue")]
        public string NameStaticValue { get; set; } = string.Empty;

        /// <summary>Gets or sets the properties.</summary>
        [DataMember(Name = "properties")]
        public IEnumerable<DocumentMapping> Properties { get; set; } = [];
    }
}