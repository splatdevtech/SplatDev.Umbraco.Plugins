namespace FormBuilder.Core.Configuration
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