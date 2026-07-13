namespace SplatDev.Umbraco.Workflow.Core.Contracts;

public interface IActionMessage
{
    string Alias { get; }

    string Label { get; }

    ActionMessageAudience Audience { get; }
}
