using MimeKit;

using Umbraco.Cms.Core.Models.Email;

namespace SplatDev.Umbraco.Plugins.Mailer.Extensions
{
    public static class AzureEmailMessageExtensions
    {
        public static NotificationEmailModel ToNotificationEmail(
this EmailMessage emailMessage,
string? configuredFromAddress)
        {
            var fromEmail = string.IsNullOrEmpty(emailMessage.From) ? configuredFromAddress : emailMessage.From;

            NotificationEmailAddress? from = ToNotificationAddress(fromEmail);

            return new NotificationEmailModel(
                from,
                GetNotificationAddresses(emailMessage.To),
                GetNotificationAddresses(emailMessage.Cc),
                GetNotificationAddresses(emailMessage.Bcc),
                GetNotificationAddresses(emailMessage.ReplyTo),
                emailMessage.Subject,
                emailMessage.Body,
                emailMessage.Attachments,
                emailMessage.IsBodyHtml);
        }

        private static NotificationEmailAddress? ToNotificationAddress(string? address)
        {
            if (InternetAddress.TryParse(address, out InternetAddress internetAddress))
            {
                if (internetAddress is MailboxAddress mailboxAddress)
                {
                    return new NotificationEmailAddress(mailboxAddress.Address, internetAddress.Name);
                }
            }

            return null;
        }

        private static IEnumerable<NotificationEmailAddress>? GetNotificationAddresses(IEnumerable<string?>? addresses)
        {
            if (addresses is null)
            {
                return null;
            }

            var notificationAddresses = new List<NotificationEmailAddress>();

            foreach (var address in addresses)
            {
                NotificationEmailAddress? notificationAddress = ToNotificationAddress(address);
                if (notificationAddress is not null)
                {
                    notificationAddresses.Add(notificationAddress);
                }
            }

            return notificationAddresses;
        }
    }
}
