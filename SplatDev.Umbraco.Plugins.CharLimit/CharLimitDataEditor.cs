using Umbraco.Cms.Core.PropertyEditors;

namespace SplatDev.Umbraco.Plugins.CharLimit;

#if NET10_0_OR_GREATER
[DataEditor(
    alias: "UmbracoCms.CharLimit",
    ValueType = ValueTypes.String)]
#else
[DataEditor(
    alias: "UmbracoCms.CharLimit",
    name: "Character Limit",
    view: "/App_Plugins/CharLimit/view.html",
    ValueType = ValueTypes.String,
    Group = "Common",
    Icon = "icon-cursor-text")]
#endif
public class CharLimitDataEditor : DataEditor
{
#if NET10_0_OR_GREATER
    private readonly global::Umbraco.Cms.Core.IO.IIOHelper _ioHelper;

    public CharLimitDataEditor(IDataValueEditorFactory dataValueEditorFactory,
        global::Umbraco.Cms.Core.IO.IIOHelper ioHelper)
        : base(dataValueEditorFactory)
    {
        _ioHelper = ioHelper;
    }

    protected override IConfigurationEditor CreateConfigurationEditor() =>
        new CharLimitConfigurationEditor(_ioHelper);
#else
    public CharLimitDataEditor(IDataValueEditorFactory dataValueEditorFactory)
        : base(dataValueEditorFactory) { }

    protected override IConfigurationEditor CreateConfigurationEditor() =>
        new CharLimitConfigurationEditor();
#endif
}
