
// Type: Umbraco.Forms.Core.Providers.FieldTypes.HiddenField
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using System;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
  [Serializable]
  public class HiddenField : FieldType
  {
    public HiddenField()
    {
      this.Id = new Guid("DA206CAE-1C52-434E-B21A-4A7C198AF877");
      this.Name = "Hidden";
      this.Alias = "hidden";
      this.Description = "Renders a HTML hidden field";
      this.Icon = "icon-checkbox-dotted";
      this.DataType = FieldDataType.String;
      this.Category = "Simple";
      this.SortOrder = 120;
      this.FieldTypeViewName = "FieldType.HiddenField.cshtml";
      this.EditView = "textfield";
      this.PreviewView = "Forms.FieldPreview.HiddenField";
      this.RenderInputType = RenderInputType.Custom;
    }

    [Setting("Default Value", Description = "Enter a default value.", DisplayOrder = 10, SupportsPlaceholders = true)]
    public virtual string DefaultValue { get; set; } = string.Empty;

    public override bool HideLabel => true;
  }
}
