namespace FormBuilder.Core.Models
{
    /// <summary>Defines the a mapped property for a document type.</summary>
    public class MappedDocumentTypePropertyModel
    {
        /// <summary>Gets or sets the Id.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets the value.</summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>Gets or sets the field.</summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>Gets or sets the static value.</summary>
        public string StaticValue { get; set; } = string.Empty;
    }
}