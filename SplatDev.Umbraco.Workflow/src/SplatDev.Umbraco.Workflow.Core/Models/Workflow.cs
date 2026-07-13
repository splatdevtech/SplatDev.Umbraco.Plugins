using SplatDev.Umbraco.Workflow.Core.Contracts;

namespace SplatDev.Umbraco.Workflow.Core.Models;

public class Workflow : IWorkflow
{
    public string Key { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public IReadOnlyList<IWorkflowStep> Steps { get; set; } = [];
}
