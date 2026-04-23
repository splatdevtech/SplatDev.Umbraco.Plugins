using Umbraco.Cms.Core.PropertyEditors;

namespace FormBuilder.Core.PropertyEditors
{
    [DataEditor("FormBuilder.FolderPicker", ValueEditorIsReusable = true)]
    public class FolderPickerPropertyEditor : DataEditor
    {
        public FolderPickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory)
          : base(dataValueEditorFactory)
        {
            SupportsReadOnly = true;
        }
    }
}