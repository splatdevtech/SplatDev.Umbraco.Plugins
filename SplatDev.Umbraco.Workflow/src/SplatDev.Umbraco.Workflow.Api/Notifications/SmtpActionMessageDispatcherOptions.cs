namespace SplatDev.Umbraco.Workflow.Api.Notifications;

public sealed class SmtpActionMessageDispatcherOptions
{
    public bool Enabled { get; set; } = true;

    public string FromAddress { get; set; } = "workflow@localhost";

    public string FallbackRecipientAddress { get; set; } = "admin@localhost";

    public string[]? RecipientAddresses { get; set; }

    public string? WorkflowLabel { get; set; }

    public bool NotifyOnCreate { get; set; } = true;

    public bool NotifyOnTransition { get; set; } = true;

    public bool NotifyOnComment { get; set; } = false;

    public bool NotifyOnAssignment { get; set; } = true;

    public string? CreateNotificationSubject { get; set; }

    public string? TransitionNotificationSubject { get; set; }

    public string? CommentNotificationSubject { get; set; }

    public string? AssignmentNotificationSubject { get; set; }
}
