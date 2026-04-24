
// Type: Umbraco.Forms.Core.Providers.FieldTypes.Text
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
    [Serializable]
    public class Text : FieldType
    {
        public Text()
        {
            this.Id = new Guid("E3FBF6C4-F46C-495E-AFF8-4B3C227B4A98");
            this.Name = "Title and description";
            this.Alias = "titleAndDescription";
            this.Description = "This is used to enter some descriptive text";
            this.Icon = "icon-edit";
            this.DataType = FieldDataType.String;
            this.Category = "Simple";
            this.SortOrder = 100;
            this.FieldTypeViewName = "FieldType.Text.cshtml";
            this.PreviewView = "Forms.FieldPreview.TitleAndDescription";
        }

        [Setting("Headline Tag", Description = "Select the tag to use when rendering the headline.", DisplayOrder = 10, PreValues = "h2,h3,h4,h5,h6", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string CaptionTag { get; set; } = "h2";

        [Setting("Headline", Description = "Enter a Headline", DisplayOrder = 20, SupportsPlaceholders = true)]
        public virtual string Caption { get; set; } = string.Empty;

        [Setting("Body Text", Description = "Enter your copy text", DisplayOrder = 30, SupportsPlaceholders = true, View = "Umb.PropertyEditorUi.TextArea")]
        public virtual string BodyText { get; set; } = string.Empty;

        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 40, PreValues = "false", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; } = string.Empty;

        public override bool HideLabel => this.ShowLabel != "True";

        public override bool StoresData => false;

        public override bool SupportsMandatory => false;
    }
}
