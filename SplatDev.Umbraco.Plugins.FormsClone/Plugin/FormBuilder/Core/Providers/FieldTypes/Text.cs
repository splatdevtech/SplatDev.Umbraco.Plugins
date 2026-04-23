using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>
    /// Provides a an text field type for a form (containing a caption and body text).
    /// </summary>
    [Serializable]
    public class Text : FieldType
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public Text()
        {
            Id = new Guid("E3FBF6C4-F46C-495E-AFF8-4B3C227B4A98");
            Name = "Title and description";
            Alias = "titleAndDescription";
            Description = "This is used to enter some descriptive text";
            Icon = "icon-edit";
            DataType = FieldDataType.String;
            Category = "Simple";
            SortOrder = 100;
            FieldTypeViewName = "FieldType.Text.cshtml";
            PreviewView = "Forms.FieldPreview.TitleAndDescription";
        }

        /// <summary>Gets or sets the tag to use for the headline.</summary>
        [Setting("Headline Tag", Description = "Select the tag to use when rendering the headline.", DisplayOrder = 10, PreValues = "h2,h3,h4,h5,h6", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string CaptionTag { get; set; } = "h2";

        /// <summary>Gets or sets the form field's caption.</summary>
        [Setting("Headline", Description = "Enter a Headline", DisplayOrder = 20, SupportsPlaceholders = true)]
        public virtual string Caption { get; set; } = string.Empty;

        /// <summary>Gets or sets the form field's body text.</summary>
        [Setting("Body Text", Description = "Enter your copy text", DisplayOrder = 30, SupportsPlaceholders = true, View = "Umb.PropertyEditorUi.TextArea")]
        public virtual string BodyText { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field label should be shown.
        /// PreValues are a single element, a boolean indicating whether the default for the the checkbox is "checked".
        /// </summary>
        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 40, PreValues = "false", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; } = string.Empty;

        /// <inheritdoc />
        public override bool HideLabel => ShowLabel != "True";

        /// <inheritdoc />
        public override bool StoresData => false;

        /// <inheritdoc />
        public override bool SupportsMandatory => false;
    }
}