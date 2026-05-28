using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SplatDev.Umbraco.Workflow.Api.Notifications;
using SplatDev.Umbraco.Workflow.Core.Enums;
using SplatDev.Umbraco.Workflow.Core.Models;
using Umbraco.Cms.Core.Mail;
using Xunit;

namespace SplatDev.Umbraco.Workflow.Core.Tests.Notifications;

public sealed class SmtpActionMessageDispatcherTests
{
    [Fact]
    public async Task Dispatch_WhenDisabled_DoesNotSendEmail()
    {
        var emailSender = new Mock<IEmailSender>();
        var options = new SmtpActionMessageDispatcherOptions { Enabled = false };
        var logger = Mock.Of<ILogger<SmtpActionMessageDispatcher>>();
        var dispatcher = new SmtpActionMessageDispatcher(
            emailSender.Object, Options.Create(options), logger);

        var evt = new WorkflowEvent(
            1, WorkflowEventType.Transition, "start", "next", "approve",
            null, "user@example.com", DateTime.UtcNow);

        await dispatcher.DispatchAsync(evt, CancellationToken.None);

        emailSender.Verify(
            s => s.SendAsync(It.IsAny<global::Umbraco.Cms.Core.Models.Email.EmailMessage>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Dispatch_OnTransition_SendsEmail()
    {
        var emailSender = new Mock<IEmailSender>();
        var options = new SmtpActionMessageDispatcherOptions
        {
            Enabled = true,
            FromAddress = "noreply@example.com",
            FallbackRecipientAddress = "admin@example.com",
            NotifyOnTransition = true,
        };
        var logger = Mock.Of<ILogger<SmtpActionMessageDispatcher>>();
        var dispatcher = new SmtpActionMessageDispatcher(
            emailSender.Object, Options.Create(options), logger);

        var evt = new WorkflowEvent(
            42, WorkflowEventType.Transition, "review", "approved", "approve",
            null, "reviewer@example.com", new DateTime(2026, 5, 28, 10, 0, 0, DateTimeKind.Utc));

        await dispatcher.DispatchAsync(evt, CancellationToken.None);

        emailSender.Verify(
            s => s.SendAsync(
                It.Is<global::Umbraco.Cms.Core.Models.Email.EmailMessage>(m =>
                    m.Subject!.Contains("Step transition") &&
                    m.Body!.Contains("42") &&
                    m.Body.Contains("review") &&
                    m.Body.Contains("approved")),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Dispatch_OnCreate_SendsEmail()
    {
        var emailSender = new Mock<IEmailSender>();
        var options = new SmtpActionMessageDispatcherOptions
        {
            Enabled = true,
            FromAddress = "noreply@example.com",
            FallbackRecipientAddress = "admin@example.com",
            NotifyOnCreate = true,
        };
        var logger = Mock.Of<ILogger<SmtpActionMessageDispatcher>>();
        var dispatcher = new SmtpActionMessageDispatcher(
            emailSender.Object, Options.Create(options), logger);

        var evt = new WorkflowEvent(
            1, WorkflowEventType.Created, null, "start", null,
            null, "creator@example.com", DateTime.UtcNow);

        await dispatcher.DispatchAsync(evt, CancellationToken.None);

        emailSender.Verify(
            s => s.SendAsync(
                It.Is<global::Umbraco.Cms.Core.Models.Email.EmailMessage>(m =>
                    m.Subject!.Contains("New instance created")),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Dispatch_OnAssignment_SendsEmail()
    {
        var emailSender = new Mock<IEmailSender>();
        var options = new SmtpActionMessageDispatcherOptions
        {
            Enabled = true,
            FromAddress = "noreply@example.com",
            FallbackRecipientAddress = "admin@example.com",
            NotifyOnAssignment = true,
        };
        var logger = Mock.Of<ILogger<SmtpActionMessageDispatcher>>();
        var dispatcher = new SmtpActionMessageDispatcher(
            emailSender.Object, Options.Create(options), logger);

        var evt = new WorkflowEvent(
            7, WorkflowEventType.Assignment, "review", "review", null,
            null, "manager@example.com", DateTime.UtcNow);

        await dispatcher.DispatchAsync(evt, CancellationToken.None);

        emailSender.Verify(
            s => s.SendAsync(
                It.Is<global::Umbraco.Cms.Core.Models.Email.EmailMessage>(m =>
                    m.Subject!.Contains("New assignment")),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Dispatch_WithCustomSubject_UsesCustomSubject()
    {
        var emailSender = new Mock<IEmailSender>();
        var options = new SmtpActionMessageDispatcherOptions
        {
            Enabled = true,
            FromAddress = "noreply@example.com",
            FallbackRecipientAddress = "admin@example.com",
            NotifyOnTransition = true,
            TransitionNotificationSubject = "Custom: Step advanced",
        };
        var logger = Mock.Of<ILogger<SmtpActionMessageDispatcher>>();
        var dispatcher = new SmtpActionMessageDispatcher(
            emailSender.Object, Options.Create(options), logger);

        var evt = new WorkflowEvent(
            1, WorkflowEventType.Transition, "start", "next", "go",
            null, "user@example.com", DateTime.UtcNow);

        await dispatcher.DispatchAsync(evt, CancellationToken.None);

        emailSender.Verify(
            s => s.SendAsync(
                It.Is<global::Umbraco.Cms.Core.Models.Email.EmailMessage>(m =>
                    m.Subject == "Custom: Step advanced"),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Dispatch_WithMultipleRecipients_JoinsThem()
    {
        var emailSender = new Mock<IEmailSender>();
        var options = new SmtpActionMessageDispatcherOptions
        {
            Enabled = true,
            FromAddress = "noreply@example.com",
            RecipientAddresses = ["a@x.com", "b@x.com", "c@x.com"],
            NotifyOnTransition = true,
        };
        var logger = Mock.Of<ILogger<SmtpActionMessageDispatcher>>();
        var dispatcher = new SmtpActionMessageDispatcher(
            emailSender.Object, Options.Create(options), logger);

        var evt = new WorkflowEvent(
            1, WorkflowEventType.Transition, "start", "next", "go",
            null, "user@example.com", DateTime.UtcNow);

        await dispatcher.DispatchAsync(evt, CancellationToken.None);

        emailSender.Verify(
            s => s.SendAsync(
                It.Is<global::Umbraco.Cms.Core.Models.Email.EmailMessage>(m =>
                    m.To!.Contains("a@x.com") &&
                    m.To.Contains("b@x.com") &&
                    m.To.Contains("c@x.com")),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Dispatch_WhenEmailSenderThrows_DoesNotPropagateException()
    {
        var emailSender = new Mock<IEmailSender>();
        emailSender
            .Setup(s => s.SendAsync(It.IsAny<global::Umbraco.Cms.Core.Models.Email.EmailMessage>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("SMTP server unavailable"));
        var options = new SmtpActionMessageDispatcherOptions
        {
            Enabled = true,
            FromAddress = "noreply@example.com",
            FallbackRecipientAddress = "admin@example.com",
            NotifyOnTransition = true,
        };
        var logger = Mock.Of<ILogger<SmtpActionMessageDispatcher>>();
        var dispatcher = new SmtpActionMessageDispatcher(
            emailSender.Object, Options.Create(options), logger);

        var evt = new WorkflowEvent(
            1, WorkflowEventType.Transition, "start", "next", "go",
            null, "user@example.com", DateTime.UtcNow);

        var act = () => dispatcher.DispatchAsync(evt, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Dispatch_OnComment_WhenNotifyOnCommentDisabled_DoesNotSend()
    {
        var emailSender = new Mock<IEmailSender>();
        var options = new SmtpActionMessageDispatcherOptions
        {
            Enabled = true,
            NotifyOnComment = false,
        };
        var logger = Mock.Of<ILogger<SmtpActionMessageDispatcher>>();
        var dispatcher = new SmtpActionMessageDispatcher(
            emailSender.Object, Options.Create(options), logger);

        var evt = new WorkflowEvent(
            1, WorkflowEventType.Comment, "review", null, null,
            "A comment", "commenter@example.com", DateTime.UtcNow);

        await dispatcher.DispatchAsync(evt, CancellationToken.None);

        emailSender.Verify(
            s => s.SendAsync(It.IsAny<global::Umbraco.Cms.Core.Models.Email.EmailMessage>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Dispatch_WhenNotifyOnCreateDisabled_DoesNotSend()
    {
        var emailSender = new Mock<IEmailSender>();
        var options = new SmtpActionMessageDispatcherOptions
        {
            Enabled = true,
            NotifyOnCreate = false,
        };
        var logger = Mock.Of<ILogger<SmtpActionMessageDispatcher>>();
        var dispatcher = new SmtpActionMessageDispatcher(
            emailSender.Object, Options.Create(options), logger);

        var evt = new WorkflowEvent(
            1, WorkflowEventType.Created, null, "start", null,
            null, "creator@example.com", DateTime.UtcNow);

        await dispatcher.DispatchAsync(evt, CancellationToken.None);

        emailSender.Verify(
            s => s.SendAsync(It.IsAny<global::Umbraco.Cms.Core.Models.Email.EmailMessage>(), It.IsAny<string>()),
            Times.Never);
    }
}
