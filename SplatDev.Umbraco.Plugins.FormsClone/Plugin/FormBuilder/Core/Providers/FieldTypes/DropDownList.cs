using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>Provides a drop down list field type for a form.</summary>
    [Serializable]
    public class DropDownList : FieldType
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public DropDownList()
        {
            Id = new Guid("0DD29D42-A6A5-11DE-A2F2-222256D89593");
            Name = "Dropdown";
            Alias = "dropdown";
            Description = "Renders a list of values in a dropdown";
            Icon = "icon-indent";
            DataType = FieldDataType.String;
            Category = "List";
            SortOrder = 80;
            ShowLabel = "True";
            FieldTypeViewName = "FieldType.DropDownList.cshtml";
            EditView = "Umb.PropertyEditorUi.Dropdown";
            PreviewView = "Forms.FieldPreview.Dropdown";
        }

        /// <summary>Gets or sets a default value for the form field.</summary>
        [Setting("Default Value", Description = "Enter a default value.", DisplayOrder = 10, SupportsPlaceholders = true)]
        public virtual string DefaultValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether multiple values can be selected.
        /// </summary>
        [Setting("Allow multiple selections", Description = "Indicate whether multiple selections from the list are allowed.", DisplayOrder = 20, View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string AllowMultipleSelections { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field label should be shown.
        /// PreValues are a single element, a boolean indicating whether the default for the the checkbox is "checked".
        /// </summary>
        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 30, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; } = string.Empty;

        /// <inheritdoc />
        public override bool HideLabel => ShowLabel == "False";

        /// <summary>
        /// Gets or sets the form field's autocomplete attribute value.
        /// </summary>
        [Setting("Autocomplete attribute", Description = "Optionally enter a value for the autocomplete attribute.", DisplayOrder = 40)]
        public virtual string AutocompleteAttribute { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the selection prompt for the drop-down list.
        /// </summary>
        [Setting("Prompt for selection", Description = "Optionally provide a prompt for the user's selection.", DisplayOrder = 40, SupportsPlaceholders = true)]
        public virtual string SelectPrompt { get; set; } = string.Empty;

        /// <inheritdoc />
        public override bool SupportsPreValues => true;
    }
}