using SplatDev.Umbraco.Workflow.Core.Contracts;

namespace SplatDev.Umbraco.Workflow.Core.Models;

public class WorkflowInstance : IWorkflowInstance
{
    public long Id { get; set; }

    public string WorkflowKey { get; set; } = string.Empty;

    public string CurrentStepKey { get; set; } = string.Empty;

    public WorkflowStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public string UpdatedBy { get; set; } = string.Empty;

    public string? MetadataJson { get; set; }

    public int WorkflowVersion { get; set; }
}
