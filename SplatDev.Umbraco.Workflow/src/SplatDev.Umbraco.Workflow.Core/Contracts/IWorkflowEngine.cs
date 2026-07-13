namespace SplatDev.Umbraco.Workflow.Core.Contracts;

public record TransitionResult(
    bool Success,
    string? FromStep,
    string? ToStep,
    WorkflowStatus NewStatus,
    string? Error);

public interface IWorkflowEngine
{
    Task<IWorkflowInstance> CreateAsync(
        string workflowKey,
        string? metadataJson,
        string actorUsername,
        CancellationToken ct);

    Task<TransitionResult> TransitionAsync(
        long instanceId,
        string actionKey,
        string actorUsername,
        CancellationToken ct);
}
