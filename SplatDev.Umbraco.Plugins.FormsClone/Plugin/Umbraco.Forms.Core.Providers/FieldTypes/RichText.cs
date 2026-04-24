
// Type: Umbraco.Forms.Core.Providers.FieldTypes.RichText
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
    [Serializable]
    public class RichText : FieldType
    {
        public RichText()
        {
            this.Id = new Guid("1F8D45F8-76E6-4550-A0F5-9637B8454619");
            this.Name = "Rich text";
            this.Alias = "richText";
            this.Description = "Provide some some descriptive text with formatting.";
            this.Icon = "icon-code";
            this.DataType = FieldDataType.LongString;
            this.Category = "Simple";
            this.SortOrder = 110;
            this.FieldTypeViewName = "FieldType.RichText.cshtml";
            this.PreviewView = "Forms.FieldPreview.Richtext";
        }

        [Setting("Rich text", Description = "Enter your formatted text", DisplayOrder = 10, HtmlEncodeReplacedPlaceholderValues = true, SupportsHtml = true, SupportsPlaceholders = true, View = "Umb.PropertyEditorUi.Tiptap")]
        public virtual string Html { get; set; } = string.Empty;

        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 20, PreValues = "false", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; } = string.Empty;

        public override bool HideLabel => this.ShowLabel != "True";

        public override bool StoresData => false;

        public override bool SupportsMandatory => false;
    }
}
