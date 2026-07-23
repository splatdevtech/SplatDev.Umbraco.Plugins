using FormBuilder.Core.Attributes;
using FormBuilder.Core.Workflows;

using Microsoft.Extensions.Options;

using System.Diagnostics.CodeAnalysis;

using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.Email;

namespace FormBuilder.Core.Providers.WorkflowTypes
{
    /// <summary>
    /// Provides common functionality for a     /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public abstract class BaseEmailWorkflowType(
      IHostingEnvironment hostingEnvironment,
      IOptions<GlobalSettings> globalSettings) : WorkflowType
    {
        private readonly IHostingEnvironment _hostingEnvironment = hostingEnvironment;
        private readonly GlobalSettings _globalSettings = globalSettings.Value;

        /// <summary>Gets or sets the receiver email address(es).</summary>
        [Setting("Recipient Email", Description = "Enter the recipient email address(es)", DisplayOrder = 10, IsMandatory = true, SupportsPlaceholders = true)]
        public virtual string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the receiver (CC) email address(es).</summary>
        [Setting("CC Email", Description = "Enter the recipient in CC email addresses (if required)", DisplayOrder = 20, SupportsPlaceholders = true)]
        public virtual string CcEmail { get; set; } = string.Empty;

        /// <summary>Gets or sets the receiver (BCC) email address(es).</summary>
        [Setting("BCC Email", Description = "Enter the recipient in BCC email addresses (if required)", DisplayOrder = 30, SupportsPlaceholders = true)]
        public virtual string BccEmail { get; set; } = string.Empty;

        /// <summary>Gets or sets the sender email address.</summary>
        [Setting("Sender Email", Description = "Enter the sender email (if not provided the default email address from your Umbraco configuration will be used)", DisplayOrder = 40, SupportsPlaceholders = true)]
        public virtual string SenderEmail { get; set; } = string.Empty;

        /// <summary>Gets or sets the email subject.</summary>
        [Setting("Reply To Email", Description = "Enter the email address to be used as the reply-to address (if required)", DisplayOrder = 50, SupportsPlaceholders = true)]
        public virtual string ReplyToEmail { get; set; } = string.Empty;

        /// <summary>Gets or sets the email subject.</summary>
        [Setting("Subject", Description = "Enter the subject", DisplayOrder = 60, IsMandatory = true, SupportsPlaceholders = true)]
        public virtual string Subject { get; set; } = string.Empty;

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = [];
            if (string.IsNullOrWhiteSpace(Email))
                exceptionList.Add(new ArgumentNullException("Email", "'Recipient Email' setting has not been set"));
            if (string.IsNullOrWhiteSpace(Subject))
                exceptionList.Add(new ArgumentNullException("Subject", "'Subject' setting has not been set'"));
            if (SenderEmail.TrimStart().StartsWith('{') && SenderEmail.TrimEnd().EndsWith('}'))
                exceptionList.Add(new ArgumentNullException("SenderEmail", "'Sender Email' should not use a placeholder that retrieves a value from the form submission (as it risks email spoofing). Use 'Reply To Email' instead."));
            return exceptionList;
        }

        /// <summary>
        /// Creates an attachment for an email from a file held in a file system.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="filePath">The path to the file within the media system.</param>
        /// <param name="attachment">The created email attachment.</param>
        protected bool TryCreateAttachment(
          IFileSystem fileSystem,
          string filePath,
          [NotNullWhen(true)] out EmailMessageAttachment? attachment)
        {
            string str1 = _hostingEnvironment.ToAbsolute(_globalSettings.UmbracoMediaPath).TrimEnd(Umbraco.Cms.Core.Constants.CharArrays.ForwardSlash);
            if (filePath.StartsWith(str1))
            {
                string str2 = filePath;
                int length = str1.Length;
                filePath = str2[length..];
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