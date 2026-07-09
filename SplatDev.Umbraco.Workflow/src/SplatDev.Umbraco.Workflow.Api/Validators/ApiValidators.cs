using FluentValidation;
using SplatDev.Umbraco.Workflow.Api.Contracts;

namespace SplatDev.Umbraco.Workflow.Api.Validators;

public sealed class TransitionRequestValidator : AbstractValidator<TransitionRequest>
{
    public TransitionRequestValidator()
    {
        RuleFor(r => r.ActionKey).NotEmpty().MaximumLength(64);
    }
}

public sealed class CreateInstanceRequestValidator : AbstractValidator<CreateInstanceRequest>
{
    public CreateInstanceRequestValidator()
    {
        RuleFor(r => r.WorkflowKey).NotEmpty().MaximumLength(64);
    }
}

public sealed class SetTaskCompletionRequestValidator : AbstractValidator<SetTaskCompletionRequest>
{
    public SetTaskCompletionRequestValidator()
    {
        RuleFor(r => r.Entries).NotEmpty();

        RuleForEach(r => r.Entries).ChildRules(entry =>
        {
            entry.RuleFor(e => e.TaskId).GreaterThan(0);
        });
    }
}

public sealed class WorkflowDefinitionDtoValidator : AbstractValidator<WorkflowDefinitionDto>
{
    public WorkflowDefinitionDtoValidator()
    {
        RuleFor(r => r.Key).NotEmpty().MaximumLength(64);
        RuleFor(r => r.Label).NotEmpty().MaximumLength(256);
        RuleFor(r => r.Version).GreaterThan(0);
        RuleFor(r => r.DefinitionJson).NotEmpty();
    }
}
