
// Type: Umbraco.Forms.Core.PropertyEditors.FolderPickerPropertyEditor
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.PropertyEditors;


#nullable enable
namespace Umbraco.Forms.Core.PropertyEditors
{
  [DataEditor("UmbracoForms.FolderPicker", ValueEditorIsReusable = true)]
  public class FolderPickerPropertyEditor : DataEditor
  {
    public FolderPickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory)
      : base(dataValueEditorFactory)
    {
      this.SupportsReadOnly = true;
    }
  }
}
