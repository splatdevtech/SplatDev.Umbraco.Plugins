using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace FormBuilder.Core.PropertyEditors
{
    public class FormPickerConfigurationEditor(IIOHelper ioHelper) : ConfigurationEditor<FormPickerConfiguration>(ioHelper)
    {
    }
}