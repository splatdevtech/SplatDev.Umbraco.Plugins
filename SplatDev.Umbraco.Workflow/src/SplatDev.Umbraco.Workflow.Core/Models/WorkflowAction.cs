using SplatDev.Umbraco.Workflow.Core.Contracts;

namespace SplatDev.Umbraco.Workflow.Core.Models;

public class WorkflowAction : IWorkflowAction
{
    public string Key { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public string NextStepKey { get; set; } = string.Empty;

    public AssignmentStrategy Assignment { get; set; }
}
