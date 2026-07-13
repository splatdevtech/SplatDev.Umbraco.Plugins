using SplatDev.Umbraco.Workflow.Core.Contracts;

namespace SplatDev.Umbraco.Workflow.Core.Models;

public class WorkflowDefinition : IWorkflowDefinition
{
    public string Key { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public int Version { get; set; }

    public IWorkflow Workflow { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
}
