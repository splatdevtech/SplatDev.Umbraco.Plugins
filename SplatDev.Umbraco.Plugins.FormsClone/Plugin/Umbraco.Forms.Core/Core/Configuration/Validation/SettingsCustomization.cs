
// Type: Umbraco.Forms.Core.Configuration.Validation.SettingsCustomization
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389


#nullable enable
namespace Umbraco.Forms.Core.Configuration.Validation
{
  public class SettingsCustomization
  {
    public ProviderSettingsCustomization DataSourceTypes { get; set; } = new ProviderSettingsCustomization();

    public ProviderSettingsCustomization FieldTypes { get; set; } = new ProviderSettingsCustomization();

    public ProviderSettingsCustomization PrevalueSourceTypes { get; set; } = new ProviderSettingsCustomization();

    public ProviderSettingsCustomization WorkflowTypes { get; set; } = new ProviderSettingsCustomization();
  }
}
