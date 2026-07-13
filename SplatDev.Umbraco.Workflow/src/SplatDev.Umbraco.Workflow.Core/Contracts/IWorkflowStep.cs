namespace SplatDev.Umbraco.Workflow.Core.Contracts;

public interface IWorkflowStep
{
    string Key { get; }

    string Label { get; }

    IReadOnlyList<IWorkflowAction> Actions { get; }

    string? Department { get; }

    string? Group { get; }

    IReadOnlyList<IActionMessage> PreActionMessages { get; }

    IReadOnlyList<IActionMessage> PostActionMessages { get; }
}
