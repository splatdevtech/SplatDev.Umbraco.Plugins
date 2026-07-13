namespace SplatDev.Umbraco.Workflow.Core.Contracts;

public interface IActionMessageDispatcher
{
    Task DispatchAsync(WorkflowEvent evt, CancellationToken ct);
}
