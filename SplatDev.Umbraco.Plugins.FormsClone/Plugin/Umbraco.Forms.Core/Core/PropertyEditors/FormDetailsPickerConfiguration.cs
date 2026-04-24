
// Type: Umbraco.Forms.Core.PropertyEditors.FormDetailsPickerConfiguration
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Forms.Core.PropertyEditors
{
  public class FormDetailsPickerConfiguration : FormPickerConfiguration
  {
    [ConfigurationField("includeThemePicker")]
    public bool IncludeThemePicker { get; set; }

    [ConfigurationField("includeRedirectPicker")]
    public bool IncludeRedirectPicker { get; set; }
  }
}
