using MimeKit;

namespace Umbraco.Plugins.Mailer.Models
{
    public class EmailModel<T> where T : class
    {
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public MailboxAddress? From { get; set; }
        public MailboxAddress? To { get; set; }
        public string? View { get; set; }
        public T? Model { get; set; }
        public string? DomainUrl { get; set; } = string.Empty;
    }
}
