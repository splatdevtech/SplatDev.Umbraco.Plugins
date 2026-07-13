using SplatDev.Umbraco.Workflow.Core.Contracts;

namespace SplatDev.Umbraco.Workflow.Core.Models;

public class WorkflowStep : IWorkflowStep
{
    public string Key { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public IReadOnlyList<IWorkflowAction> Actions { get; set; } = [];

    public string? Department { get; set; }

    public string? Group { get; set; }

    public IReadOnlyList<IActionMessage> PreActionMessages { get; set; } = [];

    public IReadOnlyList<IActionMessage> PostActionMessages { get; set; } = [];
}
