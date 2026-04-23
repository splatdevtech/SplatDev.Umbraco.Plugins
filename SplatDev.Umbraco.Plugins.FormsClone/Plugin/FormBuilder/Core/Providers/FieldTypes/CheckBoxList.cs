using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Http;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>Provides a checkbox list field type for a form.</summary>
    [Serializable]
    public class CheckBoxList : FieldType
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public CheckBoxList()
        {
            Id = new Guid("FAB43F20-A6BF-11DE-A28F-9B5755D89593");
            Name = "Multiple choice";
            Alias = "multipleChoice";
            Description = "Renders a collection of checkboxes to select multiple answers";
            Icon = "icon-bulleted-list";
            DataType = FieldDataType.String;
            Category = "List";
            SortOrder = 70;
            ShowLabel = "True";
            FieldTypeViewName = "FieldType.CheckBoxList.cshtml";
            EditView = "Umb.PropertyEditorUi.CheckBoxList";
            PreviewView = "Forms.FieldPreview.CheckboxList";
            RenderInputType = RenderInputType.Multiple;
        }

        /// <summary>Gets or sets a display layout for the form field.</summary>
        [Setting("Display Layout", Description = "Indicate whether the the field's layout should be horizontal or vertical (default if not specified).", DisplayOrder = 10, PreValues = "Horizontal,Vertical", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string DisplayLayout { get; set; } = string.Empty;

        /// <summary>Gets or sets a default value for the form field.</summary>
        [Setting("Default Value", Description = "Enter a default value.", DisplayOrder = 20)]
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

        /// <inheritdoc />
        public override IEnumerable<object> ConvertToRecord(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            List<object> record = [.. postedValues];
            return record;
        }
    }
}