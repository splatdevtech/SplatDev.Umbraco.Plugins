using Umbraco.Cms.Core.PropertyEditors;

namespace FormBuilder.Core.PropertyEditors
{
    [DataEditor("FormBuilder.ThemePicker", ValueEditorIsReusable = true)]
    public class ThemePickerPropertyEditor : DataEditor
    {
        public ThemePickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory)
          : base(dataValueEditorFactory)
        {
            SupportsReadOnly = true;
        }
    }
}