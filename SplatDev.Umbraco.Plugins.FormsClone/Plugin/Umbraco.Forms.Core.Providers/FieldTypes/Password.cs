
// Type: Umbraco.Forms.Core.Providers.FieldTypes.Password
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Options;
using System;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
  [Serializable]
  public class Password : FieldType
  {
    private readonly SecuritySettings _config;

    public Password(IOptions<SecuritySettings> config)
    {
      this._config = config.Value;
      this.Id = new Guid("FB37BC60-D41E-11DE-AEAE-37C155D89593");
      this.Name = nameof (Password);
      this.Alias = "password";
      this.Description = "Renders a password field";
      this.Icon = "icon-lock";
      this.DataType = FieldDataType.String;
      this.Category = "Simple";
      this.SortOrder = 60;
      this.FieldTypeViewName = "FieldType.PasswordField.cshtml";
      this.PreviewView = "Forms.FieldPreview.PasswordField";
      this.EditView = "password";
      this.ShowLabel = "True";
    }

    [Setting("Placeholder", Description = "Enter a HTML5 placeholder value.", DisplayOrder = 10, SupportsPlaceholders = true)]
    public virtual string Placeholder { get; set; } = string.Empty;

    [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 20, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
    public virtual string ShowLabel { get; set; }

    public override bool HideLabel => this.ShowLabel == "False";

    public override bool SupportsRegex => true;

    public override bool StoresData => this._config.SavePlainTextPasswords;
  }
}
