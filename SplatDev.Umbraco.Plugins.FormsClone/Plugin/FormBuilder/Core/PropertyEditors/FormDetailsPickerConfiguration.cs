using Umbraco.Cms.Core.PropertyEditors;

namespace FormBuilder.Core.PropertyEditors
{
    public class FormDetailsPickerConfiguration : FormPickerConfiguration
    {
        [ConfigurationField("includeThemePicker")]
        public bool IncludeThemePicker { get; set; }

        [ConfigurationField("includeRedirectPicker")]
        public bool IncludeRedirectPicker { get; set; }
    }
}