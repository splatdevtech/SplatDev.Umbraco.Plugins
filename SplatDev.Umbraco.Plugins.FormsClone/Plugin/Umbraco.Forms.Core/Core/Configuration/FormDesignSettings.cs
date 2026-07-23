
// Type: Umbraco.Forms.Core.Configuration.FormDesignSettings
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Forms.Core.Configuration.Validation;


#nullable enable
namespace Umbraco.Forms.Core.Configuration
{
  public class FormDesignSettings
  {
    public DefaultFormSettings Defaults { get; set; } = new DefaultFormSettings();

    public bool DisableAutomaticAdditionOfDataConsentField { get; set; }

    public bool DisableDefaultWorkflow { get; set; }

    public int MaxNumberOfColumnsInFormGroup { get; set; } = 12;

    public string DefaultTheme { get; set; } = "default";

    public string DefaultEmailTemplate { get; set; } = "Forms/Emails/Example-Template.cshtml";

    public bool RemoveProvidedFormTemplates { get; set; }

    public string FormElementHtmlIdPrefix { get; set; } = string.Empty;

    public SettingsCustomization SettingsCustomization { get; set; } = new SettingsCustomization();

    public bool MandatoryFieldsetLegends { get; set; }
  }
}
