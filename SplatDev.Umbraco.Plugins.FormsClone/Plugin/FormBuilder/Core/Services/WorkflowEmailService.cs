using FormBuilder.Core.Services.Interfaces;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models.Email;

namespace FormBuilder.Core.Services
{
    public class WorkflowEmailService(
      IEmailSender emailSender,
      IOptions<ContentSettings> contentSettingsConfig,
      IOptions<GlobalSettings> globalSettingsConfig,
      ILogger<WorkflowEmailService> logger) : IWorkflowEmailService
    {
        private readonly IEmailSender _emailSender = emailSender;
        private readonly ContentSettings _contentSettings = contentSettingsConfig.Value;
        private readonly GlobalSettings _globalSettings = globalSettingsConfig.Value;
        private readonly ILogger<WorkflowEmailService> _logger = logger;

        public async Task SendEmailAsync(SendEmailArgs args)
        {
            string? senderAddress = GetSenderAddress(args);
            if (!IsValidEmailAddress(senderAddress))
            {
                string message = "Error sending email, invalid sender address.";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
            string[] array1 = [.. ParseMailAddresses(args.RecipientEmail, "recipient")];
            if (array1.Length == 0)
            {
                string message = "Error sending email, invalid recipient address(es).";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
            string[] array2 = [.. ParseMailAddresses(args.CcEmail, "recipient (CC)")];
            string[] array3 = [.. ParseMailAddresses(args.BccEmail, "recipient (BCC)")];
            string[] array4 = [.. ParseMailAddresses(args.ReplyToEmail, "reply to")];
            EmailMessage message1 = new(senderAddress, array1, array2, array3, array4, args.Subject, args.Body, args.IsBodyHtml, args.Attachments);
            if (!_emailSender.CanSendRequiredEmail())
                _logger.LogError("Core email service reports that email message cannot be sent.");
            else
                await _emailSender.SendAsync(message1, "FormBuildersWorkflow").ConfigureAwait(false);
        }

        private string? GetSenderAddress(SendEmailArgs args)
        {
            string? senderAddress = args.SenderEmail;
            if (string.IsNullOrWhiteSpace(senderAddress))
                senderAddress = _contentSettings.Notifications.Email;
            if (string.IsNullOrWhiteSpace(senderAddress))
                senderAddress = _globalSettings.Smtp?.From;
            return senderAddress;
        }

        private static bool IsValidEmailAddress(string? address) => !string.IsNullOrWhiteSpace(address) && address.Contains('@') && !address.EndsWith('@');

        private HashSet<string> ParseMailAddresses(string addresses, string addressType)
        {
            HashSet<string> mailAddresses = [];
            if (addresses is null)
                return mailAddresses;
            foreach (string address in (IEnumerable<string>)addresses.Split(
            [
                ';',
                ','
            ], StringSplitOptions.RemoveEmptyEntries))
            {
                if (!IsValidEmailAddress(address))
                    _logger.LogWarning("Error sending email, invalid {addressType} address.", addressType);
                else
                    mailAddresses.Add(address);
            }
            return mailAddresses;
        }
    }
}