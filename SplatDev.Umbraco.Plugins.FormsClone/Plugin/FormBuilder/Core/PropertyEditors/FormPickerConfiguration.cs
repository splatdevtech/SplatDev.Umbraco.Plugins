using Umbraco.Cms.Core.PropertyEditors;

namespace FormBuilder.Core.PropertyEditors
{
    public class FormPickerConfiguration
    {
        [ConfigurationField("allowedFolders")]
        public IEnumerable<string> AllowedFolders { get; set; } = [];

        [ConfigurationField("allowedForms")]
        public IEnumerable<string> AllowedForms { get; set; } = [];
    }
}