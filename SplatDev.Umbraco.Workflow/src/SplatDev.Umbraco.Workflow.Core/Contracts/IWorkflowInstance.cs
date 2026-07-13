namespace SplatDev.Umbraco.Workflow.Core.Contracts;

public interface IWorkflowInstance
{
    long Id { get; }

    string WorkflowKey { get; }

    string CurrentStepKey { get; }

    WorkflowStatus Status { get; }

    DateTime CreatedAt { get; }

    DateTime UpdatedAt { get; }

    string CreatedBy { get; }

    string UpdatedBy { get; }

    string? MetadataJson { get; }
}
