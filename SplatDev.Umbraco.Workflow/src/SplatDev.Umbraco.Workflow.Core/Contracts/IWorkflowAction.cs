namespace SplatDev.Umbraco.Workflow.Core.Contracts;

public interface IWorkflowAction
{
    string Key { get; }

    string Label { get; }

    string NextStepKey { get; }

    AssignmentStrategy Assignment { get; }
}
