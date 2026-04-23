using Umbraco.Cms.Core.PropertyEditors;
using Microsoft.Extensions.Logging;

namespace UmbracoCms.Plugins.OnOff;

[DataEditor(
    alias: "OnOffButtonEditor",
    name: "On-Off Button",
    view: "~/App_Plugins/OnOff/views/edit.html",
    Group = "Common",
    Icon = "icon-power")]
public class OnOffButtonEditor(IDataValueEditorFactory dataValueEditorFactory)
    : DataEditor(dataValueEditorFactory) { }
