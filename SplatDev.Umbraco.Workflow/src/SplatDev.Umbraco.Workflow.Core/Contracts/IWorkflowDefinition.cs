namespace SplatDev.Umbraco.Workflow.Core.Contracts;

public interface IWorkflowDefinition
{
    string Key { get; }

    string Label { get; }

    int Version { get; }

    IWorkflow Workflow { get; }

    bool IsActive { get; }

    DateTime CreatedAt { get; }

    string CreatedBy { get; }
}
