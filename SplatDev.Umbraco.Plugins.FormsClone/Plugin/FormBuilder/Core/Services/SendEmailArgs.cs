using Umbraco.Cms.Core.Models.Email;

namespace FormBuilder.Core.Services
{
    public class SendEmailArgs
    {
        public string SenderEmail { get; set; } = string.Empty;

        public string ReplyToEmail { get; set; } = string.Empty;

        public string RecipientEmail { get; set; } = string.Empty;

        public string CcEmail { get; set; } = string.Empty;

        public string BccEmail { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public bool IsBodyHtml { get; set; } = true;

        public IEnumerable<EmailMessageAttachment> Attachments { get; set; } = [];
    }
}