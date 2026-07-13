using SplatDev.Umbraco.Workflow.Core.Contracts;

namespace SplatDev.Umbraco.Workflow.Core.Models;

public class ActionMessage : IActionMessage
{
    public string Alias { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public ActionMessageAudience Audience { get; set; }
}
