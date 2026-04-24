
// Type: Umbraco.Forms.Core.Providers.FieldTypes.CheckBoxList
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.AspNetCore.Http;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
    [Serializable]
    public class CheckBoxList : FieldType
    {
        public CheckBoxList()
        {
            this.Id = new Guid("FAB43F20-A6BF-11DE-A28F-9B5755D89593");
            this.Name = "Multiple choice";
            this.Alias = "multipleChoice";
            this.Description = "Renders a collection of checkboxes to select multiple answers";
            this.Icon = "icon-bulleted-list";
            this.DataType = FieldDataType.String;
            this.Category = "List";
            this.SortOrder = 70;
            this.ShowLabel = "True";
            this.FieldTypeViewName = "FieldType.CheckBoxList.cshtml";
            this.EditView = "Umb.PropertyEditorUi.CheckBoxList";
            this.PreviewView = "Forms.FieldPreview.CheckboxList";
            this.RenderInputType = RenderInputType.Multiple;
        }

        [Setting("Display Layout", Description = "Indicate whether the the field's layout should be horizontal or vertical (default if not specified).", DisplayOrder = 10, PreValues = "Horizontal,Vertical", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string DisplayLayout { get; set; } = string.Empty;

        [Setting("Default Value", Description = "Enter a default value.", DisplayOrder = 20)]
        public virtual string DefaultValue { get; set; } = string.Empty;

        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 30, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; }

        public override bool HideLabel => this.ShowLabel == "False";

        public override bool SupportsPreValues => true;

        public override IEnumerable<object> ConvertToRecord(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            List<object> record = new List<object>();
            record.AddRange(postedValues);
            return record;
        }
    }
}
