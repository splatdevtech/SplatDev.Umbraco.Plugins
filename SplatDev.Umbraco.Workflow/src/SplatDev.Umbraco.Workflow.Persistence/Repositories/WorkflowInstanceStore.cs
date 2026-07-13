using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Core.Models;
using SplatDev.Umbraco.Workflow.Persistence.Entities;
using Umbraco.Cms.Infrastructure.Scoping;

namespace SplatDev.Umbraco.Workflow.Persistence.Repositories;

public class WorkflowInstanceStore(IScopeProvider scopeProvider) : IWorkflowInstanceStore
{
    private readonly IScopeProvider _scopeProvider = scopeProvider;

    public Task<WorkflowInstance> CreateAsync(
        string workflowKey,
        int workflowVersion,
        string initialStepKey,
        string? metadataJson,
        string actorUsername,
        CancellationToken ct)
    {
        using var scope = _scopeProvider.CreateScope(autoComplete: true);
        var now = DateTime.UtcNow;

        var entity = new WorkflowInstanceEntity
        {
            WorkflowKey = workflowKey,
            WorkflowVersion = workflowVersion,
            CurrentStepKey = initialStepKey,
            Status = 0, // Open
            MetadataJson = metadataJson,
            CreatedAt = now,
            CreatedBy = actorUsername,
            UpdatedAt = now,
            UpdatedBy = actorUsername,
        };

        scope.Database.Insert(entity);

        return Task.FromResult(Map(entity)!);
    }

    public Task<WorkflowInstance?> GetByIdAsync(long id, CancellationToken ct)
    {
        using var scope = _scopeProvider.CreateScope(autoComplete: true);
        var entity = scope.Database.SingleOrDefaultById<WorkflowInstanceEntity>(id);
        return Task.FromResult(Map(entity));
    }

    public Task UpdateStepAsync(long id, string newStepKey, string actorUsername, CancellationToken ct)
    {
        using var scope = _scopeProvider.CreateScope(autoComplete: true);
        scope.Database.Execute(
            "UPDATE splatWorkflowInstance SET currentStepKey = @0, updatedAt = @1, updatedBy = @2 WHERE id = @3",
            newStepKey, DateTime.UtcNow, actorUsername, id);

        return Task.CompletedTask;
    }

    public Task UpdateStatusAsync(long id, WorkflowStatus status, string actorUsername, CancellationToken ct)
    {
        using var scope = _scopeProvider.CreateScope(autoComplete: true);
        scope.Database.Execute(
            "UPDATE splatWorkflowInstance SET status = @0, updatedAt = @1, updatedBy = @2 WHERE id = @3",
            (byte)status, DateTime.UtcNow, actorUsername, id);

        return Task.CompletedTask;
    }

    public Task SaveAssignmentAsync(
        long instanceId,
        string? assignedTo,
        string? assignedToGroup,
        string? department,
        CancellationToken ct)
    {
        using var scope = _scopeProvider.CreateScope(autoComplete: true);

        // Deactivate previous assignments
        scope.Database.Execute(
            "UPDATE splatWorkflowAssignment SET isActive = 0 WHERE instanceId = @0 AND isActive = 1",
            instanceId);

        var assignment = new WorkflowAssignmentEntity
        {
            InstanceId = instanceId,
            AssignedTo = assignedTo,
            AssignedToGroup = assignedToGroup,
            Department = department,
            AssignedAt = DateTime.UtcNow,
            IsActive = true,
        };

        scope.Database.Insert(assignment);

        return Task.CompletedTask;
    }

    private static WorkflowInstance? Map(WorkflowInstanceEntity? entity)
    {
        if (entity is null) return null;

        return new WorkflowInstance
        {
            Id = entity.Id,
            WorkflowKey = entity.WorkflowKey,
            WorkflowVersion = entity.WorkflowVersion,
            CurrentStepKey = entity.CurrentStepKey,
            Status = (WorkflowStatus)entity.Status,
            MetadataJson = entity.MetadataJson,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy,
        };
    }
}
