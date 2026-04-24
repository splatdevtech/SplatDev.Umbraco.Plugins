namespace FormBuilder.Core.Models
{
    /// <summary>Defines the mapped property data for a document type.</summary>
    public class MappedDocumentTypeModel
    {
        /// <summary>Gets or sets the document type's alias.</summary>
        public string DoctypeAlias { get; set; } = string.Empty;

        /// <summary>Gets or sets the mapped properties.</summary>
        public List<MappedDocumentTypePropertyModel> CurrentProperties { get; set; } = [];
    }
}