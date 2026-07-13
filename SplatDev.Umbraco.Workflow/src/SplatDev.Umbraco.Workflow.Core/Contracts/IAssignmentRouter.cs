namespace SplatDev.Umbraco.Workflow.Core.Contracts;

public interface IAssignmentRouter
{
    Assignment Route(IWorkflowInstance instance, IWorkflowAction action, string actorUsername);
}

public record Assignment(string? AssignedTo, string? AssignedToGroup, string? Department);
