
// Type: Umbraco.Forms.Core.PropertyEditors.FormPickerConfigurationEditor
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;


#nullable enable
namespace Umbraco.Forms.Core.PropertyEditors
{
  public class FormPickerConfigurationEditor : ConfigurationEditor<FormPickerConfiguration>
  {
    public FormPickerConfigurationEditor(IIOHelper ioHelper)
      : base(ioHelper)
    {
    }
  }
}
