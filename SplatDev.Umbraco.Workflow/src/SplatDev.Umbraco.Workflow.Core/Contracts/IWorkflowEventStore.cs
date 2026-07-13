namespace SplatDev.Umbraco.Workflow.Core.Contracts;

public record WorkflowEvent(
    long InstanceId,
    WorkflowEventType EventType,
    string? FromStepKey,
    string? ToStepKey,
    string? ActionKey,
    string? PayloadJson,
    string ActorUsername,
    DateTimeOffset OccurredAt);

public interface IWorkflowEventStore
{
    Task AppendAsync(WorkflowEvent evt, CancellationToken ct);

    IReadOnlyList<WorkflowEvent> GetHistory(long instanceId);
}
