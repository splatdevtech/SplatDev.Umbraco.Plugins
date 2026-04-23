namespace FormBuilder.Core.Configuration
{
    public class SettingsCustomization
    {
        public ProviderSettingsCustomization DataSourceTypes { get; set; } = [];

        public ProviderSettingsCustomization FieldTypes { get; set; } = [];

        public ProviderSettingsCustomization PrevalueSourceTypes { get; set; } = [];

        public ProviderSettingsCustomization WorkflowTypes { get; set; } = [];
    }
}