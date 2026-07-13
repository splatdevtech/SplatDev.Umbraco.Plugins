using System.ComponentModel.DataAnnotations;
using SplatDev.Umbraco.Workflow.Core.Contracts;

namespace SplatDev.Umbraco.Workflow.Api.Contracts;

public sealed record CreateDefinitionRequest(
    [Required] string Key,
    [Required] string Label,
    [Required] List<StepRequest> Steps);

public sealed record StepRequest(
    [Required] string Key,
    [Required] string Label,
    string? Department,
    string? Group,
    List<ActionRequest> Actions,
    List<MessageRequest> PreActions,
    List<MessageRequest> PostActions);

public sealed record ActionRequest(
    [Required] string Key,
    [Required] string Label,
    [Required] string NextStepKey,
    AssignmentStrategy Assignment);

public sealed record MessageRequest(
    [Required] string Alias,
    [Required] string Label,
    ActionMessageAudience Audience);

public sealed record CreateInstanceRequest(
    [Required] string WorkflowKey,
    string? MetadataJson);

public sealed record TransitionRequest(
    [Required] string ActionKey);
