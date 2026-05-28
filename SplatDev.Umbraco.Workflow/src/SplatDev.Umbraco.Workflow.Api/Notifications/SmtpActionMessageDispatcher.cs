using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Core.Models;
using Umbraco.Cms.Core.Mail;
using global::Umbraco.Cms.Core.Models.Email;

namespace SplatDev.Umbraco.Workflow.Api.Notifications;

public sealed class SmtpActionMessageDispatcher(
    IEmailSender emailSender,
    IOptions<SmtpActionMessageDispatcherOptions> options,
    ILogger<SmtpActionMessageDispatcher> logger) : IActionMessageDispatcher
{
    public async Task DispatchAsync(WorkflowEvent evt, CancellationToken ct)
    {
        var config = options.Value;

        if (!config.Enabled)
        {
            return;
        }

        EmailMessage? message = null;

        switch (evt.EventType)
        {
            case Core.Enums.WorkflowEventType.Created:
                message = config.NotifyOnCreate ? BuildCreateNotification(evt, config) : null;
                break;
            case Core.Enums.WorkflowEventType.Transition:
                message = config.NotifyOnTransition ? BuildTransitionNotification(evt, config) : null;
                break;
            case Core.Enums.WorkflowEventType.Comment:
                message = config.NotifyOnComment ? BuildCommentNotification(evt, config) : null;
                break;
            case Core.Enums.WorkflowEventType.Assignment:
                message = config.NotifyOnAssignment ? BuildAssignmentNotification(evt, config) : null;
                break;
            default:
                return;
        }

        if (message is null)
        {
            return;
        }

        try
        {
            await emailSender.SendAsync(message, string.Empty).ConfigureAwait(false);
            logger.LogInformation(
                "Workflow notification sent: instanceId={InstanceId}, eventType={EventType}, actor={Actor}",
                evt.InstanceId,
                evt.EventType,
                evt.ActorUsername);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to send workflow notification: instanceId={InstanceId}, eventType={EventType}",
                evt.InstanceId,
                evt.EventType);
        }
    }

    private static string GetRecipientAddressList(SmtpActionMessageDispatcherOptions config)
    {
        if (config.RecipientAddresses is { Length: > 0 })
        {
            return string.Join(",", config.RecipientAddresses);
        }

        return config.FallbackRecipientAddress;
    }

    private EmailMessage BuildCreateNotification(
        WorkflowEvent evt,
        SmtpActionMessageDispatcherOptions config)
    {
        var subject = string.IsNullOrEmpty(config.CreateNotificationSubject)
            ? "[Workflow] New instance created"
            : config.CreateNotificationSubject;

        var body = string.Format(
            "<p>A new workflow instance ({0}) has been created.</p>" +
            "<p><strong>Workflow:</strong> {1}<br/>" +
            "<strong>Step:</strong> {2}<br/>" +
            "<strong>Created by:</strong> {3}<br/>" +
            "<strong>Timestamp:</strong> {4:yyyy-MM-dd HH:mm:ss} UTC</p>",
            evt.InstanceId,
            config.WorkflowLabel ?? "Unknown",
            evt.ToStepKey,
            evt.ActorUsername,
            evt.OccurredAt);

        return new EmailMessage(
            from: config.FromAddress,
            to: GetRecipientAddressList(config),
            subject: subject,
            body: body,
            isBodyHtml: true);
    }

    private EmailMessage BuildTransitionNotification(
        WorkflowEvent evt,
        SmtpActionMessageDispatcherOptions config)
    {
        var subject = string.IsNullOrEmpty(config.TransitionNotificationSubject)
            ? "[Workflow] Step transition"
            : config.TransitionNotificationSubject;

        var body = string.Format(
            "<p>A workflow instance has been advanced.</p>" +
            "<p><strong>Instance:</strong> {0}<br/>" +
            "<strong>Workflow:</strong> {1}<br/>" +
            "<strong>From:</strong> {2}<br/>" +
            "<strong>To:</strong> {3}<br/>" +
            "<strong>Action:</strong> {4}<br/>" +
            "<strong>Performed by:</strong> {5}<br/>" +
            "<strong>Timestamp:</strong> {6:yyyy-MM-dd HH:mm:ss} UTC</p>",
            evt.InstanceId,
            config.WorkflowLabel ?? "Unknown",
            evt.FromStepKey,
            evt.ToStepKey,
            evt.ActionKey,
            evt.ActorUsername,
            evt.OccurredAt);

        return new EmailMessage(
            from: config.FromAddress,
            to: GetRecipientAddressList(config),
            subject: subject,
            body: body,
            isBodyHtml: true);
    }

    private EmailMessage BuildCommentNotification(
        WorkflowEvent evt,
        SmtpActionMessageDispatcherOptions config)
    {
        var subject = string.IsNullOrEmpty(config.CommentNotificationSubject)
            ? "[Workflow] New comment"
            : config.CommentNotificationSubject;

        var body = string.Format(
            "<p>A comment was added to a workflow instance.</p>" +
            "<p><strong>Instance:</strong> {0}<br/>" +
            "<strong>Workflow:</strong> {1}<br/>" +
            "<strong>Step:</strong> {2}<br/>" +
            "<strong>Author:</strong> {3}<br/>" +
            "<strong>Timestamp:</strong> {4:yyyy-MM-dd HH:mm:ss} UTC</p>",
            evt.InstanceId,
            config.WorkflowLabel ?? "Unknown",
            evt.ToStepKey ?? evt.FromStepKey,
            evt.ActorUsername,
            evt.OccurredAt);

        return new EmailMessage(
            from: config.FromAddress,
            to: GetRecipientAddressList(config),
            subject: subject,
            body: body,
            isBodyHtml: true);
    }

    private EmailMessage BuildAssignmentNotification(
        WorkflowEvent evt,
        SmtpActionMessageDispatcherOptions config)
    {
        var subject = string.IsNullOrEmpty(config.AssignmentNotificationSubject)
            ? "[Workflow] New assignment"
            : config.AssignmentNotificationSubject;

        var body = string.Format(
            "<p>A workflow instance has been assigned.</p>" +
            "<p><strong>Instance:</strong> {0}<br/>" +
            "<strong>Workflow:</strong> {1}<br/>" +
            "<strong>Step:</strong> {2}<br/>" +
            "<strong>Assigned by:</strong> {3}<br/>" +
            "<strong>Timestamp:</strong> {4:yyyy-MM-dd HH:mm:ss} UTC</p>",
            evt.InstanceId,
            config.WorkflowLabel ?? "Unknown",
            evt.ToStepKey ?? evt.FromStepKey,
            evt.ActorUsername,
            evt.OccurredAt);

        return new EmailMessage(
            from: config.FromAddress,
            to: GetRecipientAddressList(config),
            subject: subject,
            body: body,
            isBodyHtml: true);
    }
}
