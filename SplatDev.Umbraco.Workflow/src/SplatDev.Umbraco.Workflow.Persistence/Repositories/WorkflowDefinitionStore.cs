using System.Text.Json;
using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Core.Models;
using SplatDev.Umbraco.Workflow.Persistence.Entities;
using Umbraco.Cms.Infrastructure.Scoping;
using WorkflowModel = SplatDev.Umbraco.Workflow.Core.Models.Workflow;

namespace SplatDev.Umbraco.Workflow.Persistence.Repositories;

public class WorkflowDefinitionStore(IScopeProvider scopeProvider) : IWorkflowDefinitionStore
{
    private readonly IScopeProvider _scopeProvider = scopeProvider;

    public Task<WorkflowDefinition?> GetActiveAsync(string workflowKey, CancellationToken ct)
    {
        using var scope = _scopeProvider.CreateScope(autoComplete: true);
        var entity = scope.Database.FirstOrDefault<WorkflowDefinitionEntity>(
            "WHERE [key] = @0 AND isActive = 1 ORDER BY version DESC", workflowKey);

        return Task.FromResult(Map(entity));
    }

    public Task<WorkflowDefinition?> GetByKeyAndVersionAsync(string workflowKey, int version, CancellationToken ct)
    {
        using var scope = _scopeProvider.CreateScope(autoComplete: true);
        var entity = scope.Database.FirstOrDefault<WorkflowDefinitionEntity>(
            "WHERE [key] = @0 AND version = @1", workflowKey, version);

        return Task.FromResult(Map(entity));
    }

    public Task<IReadOnlyList<WorkflowDefinition>> ListActiveAsync(CancellationToken ct)
    {
        using var scope = _scopeProvider.CreateScope(autoComplete: true);
        var entities = scope.Database.Fetch<WorkflowDefinitionEntity>(
            "WHERE isActive = 1 ORDER BY [key], version DESC");

        return Task.FromResult<IReadOnlyList<WorkflowDefinition>>(
            entities.Select(Map).Where(d => d is not null).Cast<WorkflowDefinition>().ToList());
    }

    public Task<WorkflowDefinition> SaveAsync(WorkflowDefinition definition, string actorUsername, CancellationToken ct)
    {
        using var scope = _scopeProvider.CreateScope(autoComplete: true);
        var entity = new WorkflowDefinitionEntity
        {
            Key = definition.Key,
            Label = definition.Label,
            Version = definition.Version,
            DefinitionJson = JsonSerializer.Serialize(definition.Workflow),
            IsActive = definition.IsActive,
            CreatedAt = definition.CreatedAt,
            CreatedBy = actorUsername,
        };

        scope.Database.Insert(entity);
        definition.Key = entity.Key;

        return Task.FromResult(definition);
    }

    private static WorkflowDefinition? Map(WorkflowDefinitionEntity? entity)
    {
        if (entity is null) return null;

        var workflow = !string.IsNullOrWhiteSpace(entity.DefinitionJson)
            ? JsonSerializer.Deserialize<WorkflowModel>(entity.DefinitionJson)
            : null;

        return new WorkflowDefinition
        {
            Key = entity.Key,
            Label = entity.Label,
            Version = entity.Version,
            Workflow = workflow ?? new WorkflowModel(),
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
        };
    }
}
