using FluentValidation;
using SplatDev.Umbraco.Workflow.Api.Contracts;

namespace SplatDev.Umbraco.Workflow.Api.Controllers;

public class CreateDefinitionValidator : AbstractValidator<CreateDefinitionRequest>
{
    public CreateDefinitionValidator()
    {
        RuleFor(x => x.Key).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Label).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Steps).NotEmpty();

        RuleForEach(x => x.Steps).ChildRules(step =>
        {
            step.RuleFor(s => s.Key).NotEmpty().MaximumLength(64);
            step.RuleFor(s => s.Label).NotEmpty().MaximumLength(256);
            step.RuleForEach(s => s.Actions).ChildRules(action =>
            {
                action.RuleFor(a => a.Key).NotEmpty().MaximumLength(64);
                action.RuleFor(a => a.Label).NotEmpty().MaximumLength(256);
                action.RuleFor(a => a.NextStepKey).NotEmpty().MaximumLength(64);
                action.RuleFor(a => a.Assignment).IsInEnum();
            });
        });
    }
}
