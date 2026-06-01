using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.IO;

namespace SplatDev.Umbraco.Plugins.CharLimit;

#if NET10_0_OR_GREATER
[DataEditor(
    alias: "UmbracoCms.CharLimit",
    ValueType = ValueTypes.String,
    ValueEditorIsReusable = true)]
public class CharLimitDataEditor : DataEditor
{
    private readonly IIOHelper _ioHelper;

    public CharLimitDataEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper)
        : base(dataValueEditorFactory)
    {
        _ioHelper = ioHelper;
    }

    protected override IConfigurationEditor CreateConfigurationEditor() =>
        new CharLimitConfigurationEditor(_ioHelper);
}
#else
[DataEditor(
    alias: "UmbracoCms.CharLimit",
    name: "Character Limit",
    view: "/App_Plugins/CharLimit/view.html",
    ValueType = ValueTypes.String,
    Group = "Common",
    Icon = "icon-cursor-text")]
public class CharLimitDataEditor : DataEditor
{
    private readonly IIOHelper _ioHelper;

    public CharLimitDataEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper)
        : base(dataValueEditorFactory)
    {
        _ioHelper = ioHelper;
    }

    protected override IConfigurationEditor CreateConfigurationEditor() =>
        new CharLimitConfigurationEditor(_ioHelper);
}
#endif
