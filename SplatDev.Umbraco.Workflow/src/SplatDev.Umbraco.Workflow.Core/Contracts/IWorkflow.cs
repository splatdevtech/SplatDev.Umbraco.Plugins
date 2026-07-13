namespace SplatDev.Umbraco.Workflow.Core.Contracts;

public interface IWorkflow
{
    string Key { get; }

    string Label { get; }

    IReadOnlyList<IWorkflowStep> Steps { get; }
}
