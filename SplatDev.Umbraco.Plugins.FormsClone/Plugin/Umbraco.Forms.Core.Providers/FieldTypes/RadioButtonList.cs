
// Type: Umbraco.Forms.Core.Providers.FieldTypes.RadioButtonList
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
    [Serializable]
    public class RadioButtonList : FieldType
    {
        public RadioButtonList()
        {
            this.Id = Guid.Parse("903DF9B0-A78C-11DE-9FC1-DB7A56D89593");
            this.Name = "Single choice";
            this.Alias = "singleChoice";
            this.Description = "Renders a radio button list to enable a single choice answer";
            this.Icon = "icon-target";
            this.DataType = FieldDataType.String;
            this.Category = "List";
            this.SortOrder = 90;
            this.ShowLabel = "True";
            this.FieldTypeViewName = "FieldType.RadioButtonList.cshtml";
            this.EditView = "Umb.PropertyEditorUi.RadioButtonList";
            this.PreviewView = "Forms.FieldPreview.RadioButtonList";
            this.RenderInputType = RenderInputType.Multiple;
        }

        [Setting("Display Layout", Description = "Indicate whether the the field's layout should be horizontal or vertical (default if not specified).", DisplayOrder = 10, PreValues = "Horizontal,Vertical", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string DisplayLayout { get; set; } = string.Empty;

        [Setting("Default Value", Description = "Enter a default value.", DisplayOrder = 20, SupportsPlaceholders = true)]
        public virtual string DefaultValue { get; set; } = string.Empty;

        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 30, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; }

        public override bool HideLabel => this.ShowLabel == "False";

        public override bool SupportsPreValues => true;
    }
}
