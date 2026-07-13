using SplatDev.Umbraco.Workflow.Core.Contracts;

namespace SplatDev.Umbraco.Workflow.Core.Engine;

public class WorkflowEngine : IWorkflowEngine
{
    private readonly IWorkflowDefinitionStore _definitionStore;
    private readonly IWorkflowInstanceStore _instanceStore;
    private readonly IWorkflowEventStore _eventStore;
    private readonly IAssignmentRouter _assignmentRouter;
    private readonly IActionMessageDispatcher? _messageDispatcher;

    public WorkflowEngine(
        IWorkflowDefinitionStore definitionStore,
        IWorkflowInstanceStore instanceStore,
        IWorkflowEventStore eventStore,
        IAssignmentRouter assignmentRouter,
        IActionMessageDispatcher? messageDispatcher = null)
    {
        _definitionStore = definitionStore;
        _instanceStore = instanceStore;
        _eventStore = eventStore;
        _assignmentRouter = assignmentRouter;
        _messageDispatcher = messageDispatcher;
    }

    public async Task<IWorkflowInstance> CreateAsync(
        string workflowKey,
        string? metadataJson,
        string actorUsername,
        CancellationToken ct)
    {
        var definition = await _definitionStore.GetActiveAsync(workflowKey, ct);
        if (definition is null)
        {
            throw new InvalidOperationException($"No active workflow definition found for key '{workflowKey}'.");
        }

        var firstStep = definition.Workflow.Steps.FirstOrDefault()
            ?? throw new InvalidOperationException($"Workflow '{workflowKey}' has no steps defined.");

        var instance = await _instanceStore.CreateAsync(
            workflowKey,
            definition.Version,
            firstStep.Key,
            metadataJson,
            actorUsername,
            ct);

        var evt = new WorkflowEvent(
            instance.Id,
            WorkflowEventType.Created,
            FromStepKey: null,
            ToStepKey: firstStep.Key,
            ActionKey: null,
            PayloadJson: metadataJson,
            actorUsername,
            DateTimeOffset.UtcNow);

        await _eventStore.AppendAsync(evt, ct);

        return instance;
    }

    public async Task<TransitionResult> TransitionAsync(
        long instanceId,
        string actionKey,
        string actorUsername,
        CancellationToken ct)
    {
        var instance = await _instanceStore.GetByIdAsync(instanceId, ct);
        if (instance is null)
        {
            return new TransitionResult(false, null, null, Contracts.WorkflowStatus.Open, "Instance not found.");
        }

        if (instance.Status != Contracts.WorkflowStatus.Open)
        {
            return new TransitionResult(false, null, null, instance.Status, "Instance is not open for transitions.");
        }

        var definition = await _definitionStore.GetByKeyAndVersionAsync(
            instance.WorkflowKey, instance.WorkflowVersion, ct);
        if (definition is null)
        {
            return new TransitionResult(false, null, null, instance.Status, "Workflow definition not found.");
        }

        var currentStep = definition.Workflow.Steps
            .FirstOrDefault(s => s.Key == instance.CurrentStepKey);
        if (currentStep is null)
        {
            return new TransitionResult(false, null, null, instance.Status, "Current step not found in workflow definition.");
        }

        var action = currentStep.Actions.FirstOrDefault(a => a.Key == actionKey);
        if (action is null)
        {
            return new TransitionResult(false, null, null, instance.Status, $"Action '{actionKey}' is not valid from step '{currentStep.Key}'.");
        }

        var fromStep = instance.CurrentStepKey;
        var toStep = action.NextStepKey;

        // Dispatch pre-action messages
        await DispatchMessagesAsync(currentStep.PreActionMessages, instance, actorUsername, ct);

        // Update instance
        await _instanceStore.UpdateStepAsync(instance.Id, toStep, actorUsername, ct);

        // Check if this is a terminal step
        var targetStep = definition.Workflow.Steps.FirstOrDefault(s => s.Key == toStep);
        var newStatus = targetStep?.Actions.Count == 0
            ? Contracts.WorkflowStatus.Completed
            : Contracts.WorkflowStatus.Open;

        if (newStatus == Contracts.WorkflowStatus.Completed)
        {
            await _instanceStore.UpdateStatusAsync(instance.Id, newStatus, actorUsername, ct);
        }

        // Route assignment
        var assignment = _assignmentRouter.Route(instance, action, actorUsername);
        if (assignment.AssignedTo is not null || assignment.AssignedToGroup is not null)
        {
            await _instanceStore.SaveAssignmentAsync(
                instance.Id, assignment.AssignedTo, assignment.AssignedToGroup, assignment.Department, ct);

            await _eventStore.AppendAsync(new WorkflowEvent(
                instance.Id,
                WorkflowEventType.Assignment,
                FromStepKey: null,
                ToStepKey: null,
                ActionKey: null,
                PayloadJson: null,
                actorUsername,
                DateTimeOffset.UtcNow), ct);
        }

        // Record the transition event
        var transitionEvent = new WorkflowEvent(
            instance.Id,
            WorkflowEventType.Transition,
            fromStep,
            toStep,
            actionKey,
            PayloadJson: null,
            actorUsername,
            DateTimeOffset.UtcNow);

        await _eventStore.AppendAsync(transitionEvent, ct);

        // Dispatch post-action messages
        await DispatchMessagesAsync(currentStep.PostActionMessages, instance, actorUsername, ct);

        return new TransitionResult(true, fromStep, toStep, newStatus, Error: null);
    }

    private async Task DispatchMessagesAsync(
        IReadOnlyList<IActionMessage> messages,
        IWorkflowInstance instance,
        string actorUsername,
        CancellationToken ct)
    {
        if (_messageDispatcher is null || messages.Count == 0) return;

        foreach (var msg in messages)
        {
            var evt = new WorkflowEvent(
                instance.Id,
                WorkflowEventType.ActionMessage,
                FromStepKey: null,
                ToStepKey: null,
                ActionKey: null,
                PayloadJson: System.Text.Json.JsonSerializer.Serialize(new { msg.Alias, msg.Audience }),
                actorUsername,
                DateTimeOffset.UtcNow);

            await _messageDispatcher.DispatchAsync(evt, ct);
            await _eventStore.AppendAsync(evt, ct);
        }
    }
}
