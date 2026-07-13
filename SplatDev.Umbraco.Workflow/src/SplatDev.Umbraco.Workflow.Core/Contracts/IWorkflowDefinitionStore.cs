using SplatDev.Umbraco.Workflow.Core.Models;

namespace SplatDev.Umbraco.Workflow.Core.Contracts;

public interface IWorkflowDefinitionStore
{
    Task<WorkflowDefinition?> GetActiveAsync(string workflowKey, CancellationToken ct);

    Task<WorkflowDefinition?> GetByKeyAndVersionAsync(string workflowKey, int version, CancellationToken ct);

    Task<IReadOnlyList<WorkflowDefinition>> ListActiveAsync(CancellationToken ct);

    Task<WorkflowDefinition> SaveAsync(WorkflowDefinition definition, string actorUsername, CancellationToken ct);
}
