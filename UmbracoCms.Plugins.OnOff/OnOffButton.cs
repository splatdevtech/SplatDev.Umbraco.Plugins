// OnOffButton property editor — only compiled for Umbraco 13 (net8.0).
// Umbraco 17 (net10.0) requires a Lit-based property editor implementation
// and uses a different registration model (no DataEditor attribute).
#if !NET10_0_OR_GREATER
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
#endif
