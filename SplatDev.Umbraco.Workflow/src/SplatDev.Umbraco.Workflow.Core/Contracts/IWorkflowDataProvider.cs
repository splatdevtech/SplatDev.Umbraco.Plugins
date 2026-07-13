namespace SplatDev.Umbraco.Workflow.Core.Contracts;

public record WorkflowDisplayRow(
    long InstanceId,
    string WorkflowKey,
    string CurrentStepLabel,
    WorkflowStatus Status,
    DateTime CreatedAt,
    string CreatedBy,
    string? Assignee,
    IReadOnlyDictionary<string, object?> CustomFields);

public record DisplayColumn(string Key, string Label, int Order);

public record WorkflowQueryFilter(
    string? WorkflowKey,
    WorkflowStatus? Status,
    bool AssignedToMe,
    string? Group,
    string? Department,
    string? Search,
    int Page,
    int PageSize);

public interface IWorkflowDataProvider
{
    IReadOnlyList<WorkflowDisplayRow> GetDisplayRows(WorkflowQueryFilter filter);

    string? GetSearchableValue(long instanceId, string fieldKey);

    IReadOnlyList<DisplayColumn> GetColumns(string workflowKey);
}
