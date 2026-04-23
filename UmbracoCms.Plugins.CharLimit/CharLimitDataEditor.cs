using Umbraco.Cms.Core.PropertyEditors;

namespace UmbracoCms.Plugins.CharLimit;

[DataEditor(
    alias: "UmbracoCms.CharLimit",
    name: "Character Limit",
    view: "/App_Plugins/CharLimit/view.html",
    ValueType = ValueTypes.String,
    Group = "Common",
    Icon = "icon-cursor-text")]
public class CharLimitDataEditor : DataEditor
{
    public CharLimitDataEditor(IDataValueEditorFactory dataValueEditorFactory)
        : base(dataValueEditorFactory) { }

    protected override IConfigurationEditor CreateConfigurationEditor() =>
        new CharLimitConfigurationEditor();
}
