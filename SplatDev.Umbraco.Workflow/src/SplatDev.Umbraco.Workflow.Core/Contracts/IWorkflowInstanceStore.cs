using SplatDev.Umbraco.Workflow.Core.Models;

namespace SplatDev.Umbraco.Workflow.Core.Contracts;

public interface IWorkflowInstanceStore
{
    Task<WorkflowInstance> CreateAsync(
        string workflowKey,
        int workflowVersion,
        string initialStepKey,
        string? metadataJson,
        string actorUsername,
        CancellationToken ct);

    Task<WorkflowInstance?> GetByIdAsync(long id, CancellationToken ct);

    Task UpdateStepAsync(long id, string newStepKey, string actorUsername, CancellationToken ct);

    Task UpdateStatusAsync(long id, WorkflowStatus status, string actorUsername, CancellationToken ct);

    Task SaveAssignmentAsync(
        long instanceId,
        string? assignedTo,
        string? assignedToGroup,
        string? department,
        CancellationToken ct);
}
