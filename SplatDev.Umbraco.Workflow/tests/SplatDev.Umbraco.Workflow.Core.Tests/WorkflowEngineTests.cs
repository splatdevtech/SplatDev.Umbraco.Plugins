using FluentAssertions;
using Moq;
using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Core.Engine;
using SplatDev.Umbraco.Workflow.Core.Models;
using WorkflowModel = SplatDev.Umbraco.Workflow.Core.Models.Workflow;

namespace SplatDev.Umbraco.Workflow.Core.Tests;

public class WorkflowEngineTests
{
    private readonly Mock<IWorkflowDefinitionStore> _defStore = new();
    private readonly Mock<IWorkflowInstanceStore> _instanceStore = new();
    private readonly Mock<IWorkflowEventStore> _eventStore = new();
    private readonly Mock<IAssignmentRouter> _assignmentRouter = new();

    private IWorkflowEngine CreateEngine()
    {
        return new WorkflowEngine(
            _defStore.Object,
            _instanceStore.Object,
            _eventStore.Object,
            _assignmentRouter.Object);
    }

    private static WorkflowDefinition CreateSimpleWorkflow()
    {
        return new WorkflowDefinition
        {
            Key = "onboarding",
            Label = "Onboarding",
            Version = 1,
            IsActive = true,
            Workflow = new WorkflowModel
            {
                Key = "onboarding",
                Label = "Onboarding",
                Steps =
                [
                    new WorkflowStep
                    {
                        Key = "review", Label = "Review",
                        Actions =
                        [
                            new WorkflowAction
                            {
                                Key = "approve", Label = "Approve",
                                NextStepKey = "final",
                                Assignment = AssignmentStrategy.Manual,
                            },
                            new WorkflowAction
                            {
                                Key = "reject", Label = "Reject",
                                NextStepKey = "review",
                                Assignment = AssignmentStrategy.AssignToSubmitter,
                            },
                        ],
                    },
                    new WorkflowStep
                    {
                        Key = "final", Label = "Final",
                        Actions = [], // terminal
                    },
                ],
            },
        };
    }

    [Fact]
    public async Task CreateAsync_WithValidWorkflow_ReturnsInstance()
    {
        _defStore.Setup(s => s.GetActiveAsync("onboarding", It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSimpleWorkflow());

        _instanceStore.Setup(s => s.CreateAsync(
                "onboarding", 1, "review", null, "admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WorkflowInstance
            {
                Id = 1, WorkflowKey = "onboarding", CurrentStepKey = "review",
                Status = WorkflowStatus.Open, CreatedAt = DateTime.UtcNow,
            });

        var engine = CreateEngine();

        var instance = await engine.CreateAsync("onboarding", null, "admin", CancellationToken.None);

        instance.Should().NotBeNull();
        instance.CurrentStepKey.Should().Be("review");
        instance.Status.Should().Be(WorkflowStatus.Open);

        _eventStore.Verify(s => s.AppendAsync(
            It.Is<WorkflowEvent>(e => e.EventType == WorkflowEventType.Created),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithUnknownWorkflow_Throws()
    {
        _defStore.Setup(s => s.GetActiveAsync("unknown", It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkflowDefinition?)null);

        var engine = CreateEngine();

        var act = () => engine.CreateAsync("unknown", null, "admin", CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*No active workflow*");
    }

    [Fact]
    public async Task TransitionAsync_ValidAction_Succeeds()
    {
        var def = CreateSimpleWorkflow();
        _defStore.Setup(s => s.GetByKeyAndVersionAsync("onboarding", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(def);

        _instanceStore.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WorkflowInstance
            {
                Id = 1, WorkflowKey = "onboarding", WorkflowVersion = 1,
                CurrentStepKey = "review", Status = WorkflowStatus.Open,
            });

        _assignmentRouter.Setup(r => r.Route(
                It.IsAny<IWorkflowInstance>(), It.IsAny<IWorkflowAction>(), "manager"))
            .Returns(new Assignment(null, null, null));

        var engine = CreateEngine();

        var result = await engine.TransitionAsync(1, "approve", "manager", CancellationToken.None);

        result.Success.Should().BeTrue();
        result.FromStep.Should().Be("review");
        result.ToStep.Should().Be("final");
        result.NewStatus.Should().Be(WorkflowStatus.Completed);

        _instanceStore.Verify(s => s.UpdateStepAsync(1, "final", "manager", It.IsAny<CancellationToken>()), Times.Once);
        _instanceStore.Verify(s => s.UpdateStatusAsync(1, WorkflowStatus.Completed, "manager", It.IsAny<CancellationToken>()), Times.Once);
        _eventStore.Verify(s => s.AppendAsync(
            It.Is<WorkflowEvent>(e => e.EventType == WorkflowEventType.Transition),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TransitionAsync_InvalidAction_ReturnsFailure()
    {
        var def = CreateSimpleWorkflow();
        _defStore.Setup(s => s.GetByKeyAndVersionAsync("onboarding", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(def);

        _instanceStore.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WorkflowInstance
            {
                Id = 1, WorkflowKey = "onboarding", WorkflowVersion = 1,
                CurrentStepKey = "review", Status = WorkflowStatus.Open,
            });

        var engine = CreateEngine();

        var result = await engine.TransitionAsync(1, "nonexistent", "manager", CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Contain("not valid");
    }

    [Fact]
    public async Task TransitionAsync_InstanceNotFound_ReturnsFailure()
    {
        _instanceStore.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkflowInstance?)null);

        var engine = CreateEngine();

        var result = await engine.TransitionAsync(1, "approve", "manager", CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("Instance not found.");
    }

    [Fact]
    public async Task TransitionAsync_InstanceNotOpen_ReturnsFailure()
    {
        _instanceStore.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WorkflowInstance
            {
                Id = 1, WorkflowKey = "onboarding", WorkflowVersion = 1,
                CurrentStepKey = "review", Status = WorkflowStatus.Completed,
            });

        var engine = CreateEngine();

        var result = await engine.TransitionAsync(1, "approve", "manager", CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Contain("not open");
    }
}
