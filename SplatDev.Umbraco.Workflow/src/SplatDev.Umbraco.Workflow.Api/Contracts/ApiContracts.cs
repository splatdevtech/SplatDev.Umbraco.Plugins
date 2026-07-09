namespace SplatDev.Umbraco.Workflow.Api.Contracts;

/// <summary>API response DTO for a workflow definition.</summary>
public sealed record WorkflowDefinitionDto(
    string Key, string Label, int Version, bool IsActive, string DefinitionJson, DateTime CreatedAt);

/// <summary>API response DTO for a workflow instance.</summary>
public sealed record WorkflowInstanceDto(
    long Id, string WorkflowKey, int WorkflowVersion, string CurrentStepKey,
    int Status, string? MetadataJson, DateTime CreatedAt, string CreatedBy,
    DateTime UpdatedAt, string UpdatedBy);

/// <summary>Request DTO to create a new workflow instance.</summary>
public sealed record CreateInstanceRequest(string WorkflowKey, string? MetadataJson);

/// <summary>Request DTO to transition a workflow instance.</summary>
public sealed record TransitionRequest(string ActionKey);

/// <summary>Request DTO to set task completion statuses.</summary>
public sealed record SetTaskCompletionRequest(IReadOnlyList<TaskCompletionEntry> Entries);

/// <summary>A single task completion entry within a batch update.</summary>
public sealed record TaskCompletionEntry(long TaskId, bool IsCompleted);

/// <summary>Generic paged result wrapper for list endpoints.</summary>
/// <typeparam name="T">The type of items in the page.</typeparam>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
