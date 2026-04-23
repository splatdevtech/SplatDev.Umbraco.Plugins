using FormBuilder.Core.Enums;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a field type representation.</summary>
    [DataContract(Name = "fieldtype")]
    [Serializable]
    public class FieldTypeWithSettings : IProviderTypeWithSettings
    {
        /// <summary>Gets or sets the field type's Id.</summary>
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets the unique ID for the field type (with the field name required for front-end rendering).
        /// </summary>
        [DataMember(Name = "unique")]
        public Guid Unique => Id;

        /// <summary>
        /// Gets the field type entity type (required for front-end rendering).
        /// </summary>
        [DataMember(Name = "entityType")]
        public static string EntityType => "field-type";

        /// <summary>Gets or sets the field type's alias.</summary>
        [DataMember(Name = "alias")]
        public string Alias { get; set; } = string.Empty;

        /// <summary>Gets or sets the field type's name.</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the field type's icon.</summary>
        [DataMember(Name = "icon")]
        public string Icon { get; set; } = string.Empty;

        /// <summary>Gets or sets the field type's group.</summary>
        [DataMember(Name = "group")]
        public string Group { get; set; } = string.Empty;

        /// <summary>Gets or sets the field type's sort order.</summary>
        [DataMember(Name = "sortOrder")]
        public int SortOrder { get; set; }

        /// <summary>Gets or sets the field type's description.</summary>
        [DataMember(Name = "description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field type supports prevalues.
        /// </summary>
        [DataMember(Name = "supportsPrevalues")]
        public bool SupportsPrevalues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field type supports file uploads.
        /// </summary>
        [DataMember(Name = "supportsUploadTypes")]
        public bool SupportsUploadTypes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field type supports mandatory validation.
        /// </summary>
        [DataMember(Name = "supportsMandatory")]
        public bool SupportsMandatory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field type supports validation by regular expression.
        /// </summary>
        [DataMember(Name = "supportsRegex")]
        public bool SupportsRegex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field types label should be hidden.
        /// </summary>
        [DataMember(Name = "hideLabel")]
        public bool HideLabel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how the the field should be rendered, as a single input, multiple inputs within an an HTML fieldset (e.g. a radio button or check box list).
        /// </summary>
        [DataMember(Name = "renderInputType")]
        public RenderInputType RenderInputType { get; set; }

        /// <summary>Gets or sets the field type's settings.</summary>
        [DataMember(Name = "settings")]
        public IEnumerable<Setting> Settings { get; set; } = [];

        /// <summary>Gets or sets the field type's view for rendering.</summary>
        [DataMember(Name = "view")]
        public string View { get; set; } = string.Empty;

        /// <summary>Gets or sets the field type's view for preview.</summary>
        [DataMember(Name = "previewView")]
        public string PreviewView { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field should be mandatory by default or not.
        /// </summary>
        [DataMember(Name = "mandatoryByDefault")]
        public bool MandatoryByDefault { get; set; }
    }
}