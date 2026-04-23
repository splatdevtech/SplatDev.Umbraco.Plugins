
// Type: Umbraco.Forms.Core.Attributes.SettingAttribute
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;


#nullable enable
namespace Umbraco.Forms.Core.Attributes
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class SettingAttribute : Attribute
  {
    public SettingAttribute(string name)
    {
      this.Name = name;
      this.Description = string.Empty;
      this.PreValues = string.Empty;
      this.Alias = string.Empty;
      this.View = "Umb.PropertyEditorUi.TextBox";
      this.SupportsPlaceholders = false;
      this.HtmlEncodeReplacedPlaceholderValues = false;
    }

    public string Name { get; set; }

    public string Description { get; set; }

    public string PreValues { get; set; }

    public string View { get; set; }

    public string Alias { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsHidden { get; set; }

    public bool IsMandatory { get; set; }

    public bool SupportsPlaceholders { get; set; }

    public bool SupportsHtml { get; set; }

    public bool HtmlEncodeReplacedPlaceholderValues { get; set; }

    public List<string> GetPreValues()
    {
      List<string> preValues = new List<string>();
      preValues.AddRange((IEnumerable<string>) this.PreValues.Split(',', StringSplitOptions.RemoveEmptyEntries));
      return preValues;
    }
  }
}
