
// Type: Umbraco.Forms.Core.Providers.FieldTypes.DropDownList
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
    [Serializable]
    public class DropDownList : FieldType
    {
        public DropDownList()
        {
            this.Id = new Guid("0DD29D42-A6A5-11DE-A2F2-222256D89593");
            this.Name = "Dropdown";
            this.Alias = "dropdown";
            this.Description = "Renders a list of values in a dropdown";
            this.Icon = "icon-indent";
            this.DataType = FieldDataType.String;
            this.Category = "List";
            this.SortOrder = 80;
            this.ShowLabel = "True";
            this.FieldTypeViewName = "FieldType.DropDownList.cshtml";
            this.EditView = "Umb.PropertyEditorUi.Dropdown";
            this.PreviewView = "Forms.FieldPreview.Dropdown";
        }

        [Setting("Default Value", Description = "Enter a default value.", DisplayOrder = 10, SupportsPlaceholders = true)]
        public virtual string DefaultValue { get; set; } = string.Empty;

        [Setting("Allow multiple selections", Description = "Indicate whether multiple selections from the list are allowed.", DisplayOrder = 20, View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string AllowMultipleSelections { get; set; } = string.Empty;

        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 30, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; } = string.Empty;

        public override bool HideLabel => this.ShowLabel == "False";

        [Setting("Autocomplete attribute", Description = "Optionally enter a value for the autocomplete attribute.", DisplayOrder = 40)]
        public virtual string AutocompleteAttribute { get; set; } = string.Empty;

        [Setting("Prompt for selection", Description = "Optionally provide a prompt for the user's selection.", DisplayOrder = 40, SupportsPlaceholders = true)]
        public virtual string SelectPrompt { get; set; } = string.Empty;

        public override bool SupportsPreValues => true;
    }
}
