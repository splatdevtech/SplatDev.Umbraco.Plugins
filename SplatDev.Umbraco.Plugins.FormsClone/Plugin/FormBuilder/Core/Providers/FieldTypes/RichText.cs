using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>Provides a an rich text field type for a form.</summary>
    [Serializable]
    public class RichText : FieldType
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public RichText()
        {
            Id = new Guid("1F8D45F8-76E6-4550-A0F5-9637B8454619");
            Name = "Rich text";
            Alias = "richText";
            Description = "Provide some some descriptive text with formatting.";
            Icon = "icon-code";
            DataType = FieldDataType.LongString;
            Category = "Simple";
            SortOrder = 110;
            FieldTypeViewName = "FieldType.RichText.cshtml";
            PreviewView = "Forms.FieldPreview.Richtext";
        }

        /// <summary>Gets or sets the form field's body text.</summary>
        [Setting("Rich text", Description = "Enter your formatted text", DisplayOrder = 10, HtmlEncodeReplacedPlaceholderValues = true, SupportsHtml = true, SupportsPlaceholders = true, View = "Umb.PropertyEditorUi.Tiptap")]
        public virtual string Html { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field label should be shown.
        /// PreValues are a single element, a boolean indicating whether the default for the the checkbox is "checked".
        /// </summary>
        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 20, PreValues = "false", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; } = string.Empty;

        /// <inheritdoc />
        public override bool HideLabel => ShowLabel != "True";

        /// <inheritdoc />
        public override bool StoresData => false;

        /// <inheritdoc />
        public override bool SupportsMandatory => false;
    }
}