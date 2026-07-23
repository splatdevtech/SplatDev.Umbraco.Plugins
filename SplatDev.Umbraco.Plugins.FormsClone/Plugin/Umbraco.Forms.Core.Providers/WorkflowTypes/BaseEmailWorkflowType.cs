
// Type: Umbraco.Forms.Core.Providers.WorkflowTypes.BaseEmailWorkflowType
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Options;

using System.Diagnostics.CodeAnalysis;

using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.Email;
using Umbraco.Forms.Core.Attributes;


#nullable enable
namespace Umbraco.Forms.Core.Providers.WorkflowTypes
{
    public abstract class BaseEmailWorkflowType : WorkflowType
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly GlobalSettings _globalSettings;

        protected BaseEmailWorkflowType(
          IHostingEnvironment hostingEnvironment,
          IOptions<GlobalSettings> globalSettings)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._globalSettings = globalSettings.Value;
        }

        [Setting("Recipient Email", Description = "Enter the recipient email address(es)", DisplayOrder = 10, IsMandatory = true, SupportsPlaceholders = true)]
        public virtual string Email { get; set; } = string.Empty;

        [Setting("CC Email", Description = "Enter the recipient in CC email addresses (if required)", DisplayOrder = 20, SupportsPlaceholders = true)]
        public virtual string CcEmail { get; set; } = string.Empty;

        [Setting("BCC Email", Description = "Enter the recipient in BCC email addresses (if required)", DisplayOrder = 30, SupportsPlaceholders = true)]
        public virtual string BccEmail { get; set; } = string.Empty;

        [Setting("Sender Email", Description = "Enter the sender email (if not provided the default email address from your Umbraco configuration will be used)", DisplayOrder = 40, SupportsPlaceholders = true)]
        public virtual string SenderEmail { get; set; } = string.Empty;

        [Setting("Reply To Email", Description = "Enter the email address to be used as the reply-to address (if required)", DisplayOrder = 50, SupportsPlaceholders = true)]
        public virtual string ReplyToEmail { get; set; } = string.Empty;

        [Setting("Subject", Description = "Enter the subject", DisplayOrder = 60, IsMandatory = true, SupportsPlaceholders = true)]
        public virtual string Subject { get; set; } = string.Empty;

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = new List<Exception>();
            if (string.IsNullOrWhiteSpace(this.Email))
                exceptionList.Add(new ArgumentNullException("Email", "'Recipient Email' setting has not been set"));
            if (string.IsNullOrWhiteSpace(this.Subject))
                exceptionList.Add(new ArgumentNullException("Subject", "'Subject' setting has not been set'"));
            if (this.SenderEmail.TrimStart().StartsWith('{') && this.SenderEmail.TrimEnd().EndsWith('}'))
                exceptionList.Add(new ArgumentNullException("SenderEmail", "'Sender Email' should not use a placeholder that retrieves a value from the form submission (as it risks email spoofing). Use 'Reply To Email' instead."));
            return exceptionList;
        }

        protected bool TryCreateAttachment(
          IFileSystem fileSystem,
          string filePath,
          [NotNullWhen(true)] out EmailMessageAttachment? attachment)
        {
            string str1 = this._hostingEnvironment.ToAbsolute(this._globalSettings.UmbracoMediaPath).TrimEnd(Umbraco.Cms.Core.Constants.CharArrays.ForwardSlash);
            if (filePath.StartsWith(str1))
            {
                string str2 = filePath;
                int length = str1.Length;
                filePath = str2.Substring(length, str2.Length - length);
            }
            if (fileSystem.FileExists(filePath))
            {
                Stream stream = fileSystem.OpenFile(filePath);
                string fileName = Path.GetFileName(filePath);
                attachment = new EmailMessageAttachment(stream, fileName);
                return true;
            }
            attachment = null;
            return false;
        }
    }
}
