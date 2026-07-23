using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>Provides a radio button list field type for a form.</summary>
    [Serializable]
    public class RadioButtonList : FieldType
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public RadioButtonList()
        {
            Id = Guid.Parse("903DF9B0-A78C-11DE-9FC1-DB7A56D89593");
            Name = "Single choice";
            Alias = "singleChoice";
            Description = "Renders a radio button list to enable a single choice answer";
            Icon = "icon-target";
            DataType = FieldDataType.String;
            Category = "List";
            SortOrder = 90;
            ShowLabel = "True";
            FieldTypeViewName = "FieldType.RadioButtonList.cshtml";
            EditView = "Umb.PropertyEditorUi.RadioButtonList";
            PreviewView = "Forms.FieldPreview.RadioButtonList";
            RenderInputType = RenderInputType.Multiple;
        }

        /// <summary>Gets or sets a display layout for the form field.</summary>
        [Setting("Display Layout", Description = "Indicate whether the the field's layout should be horizontal or vertical (default if not specified).", DisplayOrder = 10, PreValues = "Horizontal,Vertical", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string DisplayLayout { get; set; } = string.Empty;

        /// <summary>Gets or sets a default value for the form field.</summary>
        [Setting("Default Value", Description = "Enter a default value.", DisplayOrder = 20, SupportsPlaceholders = true)]
        public virtual string DefaultValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field label should be shown.
        /// PreValues are a single element, a boolean indicating whether the default for the the checkbox is "checked".
        /// </summary>
        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 30, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; }

        /// <inheritdoc />
        public override bool HideLabel => ShowLabel == "False";

        /// <inheritdoc />
        public override bool SupportsPreValues => true;
    }
}