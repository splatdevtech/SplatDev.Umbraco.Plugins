using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Helpers;
using FormBuilder.Core.Services;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Workflows;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Text;
using System.Xml;
using System.Xml.XPath;

using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.Email;

namespace FormBuilder.Core.Providers.WorkflowTypes
{
    /// <summary>
    /// Provides a     /// </summary>
    public class SendEmail : BaseEmailWorkflowType
    {
        private readonly IXmlService _xmlService;
        private readonly IWorkflowEmailService _workflowEmailService;
        private readonly MediaFileManager _mediaFileManager;
        private readonly IDictionaryHelper _dictionaryHelper;
        private readonly ILogger<SendEmail> _logger;
        private readonly IPlaceholderParsingService _placeholderParsingService;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public SendEmail(
          IHostingEnvironment hostingEnvironment,
          IOptions<GlobalSettings> globalSettings,
          IXmlService xmlService,
          IWorkflowEmailService workflowEmailService,
          MediaFileManager mediaFileManager,
          IDictionaryHelper dictionaryHelper,
          ILogger<SendEmail> logger,
          IPlaceholderParsingService placeholderParsingService)
          : base(hostingEnvironment, globalSettings)
        {
            Id = new Guid("E96BADD7-05BE-4978-B8D9-B3D733DE70A5");
            Name = "Send email";
            Alias = "sendEmail";
            Description = "Send the result of the form to an email address";
            Icon = "icon-message";
            Group = "Email";
            _xmlService = xmlService;
            _workflowEmailService = workflowEmailService;
            _mediaFileManager = mediaFileManager;
            _dictionaryHelper = dictionaryHelper;
            _logger = logger;
            _placeholderParsingService = placeholderParsingService;
        }

        /// <summary>Gets or sets the introductory message for the email.</summary>
        [Setting("Message", Description = "Enter the introductory message", DisplayOrder = 70, IsMandatory = true, SupportsPlaceholders = true, View = "Umb.PropertyEditorUi.TextArea")]
        public virtual string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to attach file uploads to the email.
        /// </summary>
        [Setting("Attachments", Description = "Attach file uploads to email", DisplayOrder = 80, View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string Attachment { get; set; } = string.Empty;

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = base.ValidateSettings();
            if (string.IsNullOrEmpty(Message))
                exceptionList.Add(new Exception("'Message' setting has not been set'"));
            return exceptionList;
        }

        /// <inheritdoc />
        public override async Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            SendEmail sendEmail = this;
            try
            {
                XPathNavigator? navigator = sendEmail._xmlService.ToXml(context.Record, new XmlDocument()).CreateNavigator();
                if (navigator == null)
                {
                    sendEmail._logger.LogError("There was a problem sending an email from workflow associated to Form {FormName} with unique id {FormId}. Unable to create XPath Navigator.", context.Form.Name, context.Form.Id);
                    return WorkflowExecutionStatus.Failed;
                }
                XPathExpression expr = navigator.Compile("//fields/child::*");
                expr.AddSort("@pageindex", XmlSortOrder.Ascending, XmlCaseOrder.None, string.Empty, XmlDataType.Number);
                expr.AddSort("@fieldsetindex", XmlSortOrder.Ascending, XmlCaseOrder.None, string.Empty, XmlDataType.Number);
                expr.AddSort("@sortorder", XmlSortOrder.Ascending, XmlCaseOrder.None, string.Empty, XmlDataType.Number);
                XPathNodeIterator xpathNodeIterator1 = navigator.Select(expr);
                StringBuilder stringBuilder = new("<dl>");
                List<EmailMessageAttachment> attachments = [];
                while (xpathNodeIterator1.MoveNext() && xpathNodeIterator1.Current is not null)
                {
                    XPathNavigator? xpathNavigator = xpathNodeIterator1.Current.SelectSingleNode("caption");
                    if (xpathNavigator is not null)
                        stringBuilder.AppendFormat("<dt><strong>{0}</strong></dt><dd>", sendEmail._placeholderParsingService.ParsePlaceHolders(sendEmail._dictionaryHelper.GetText(xpathNavigator.Value), false, context.Record));
                    XPathNodeIterator xpathNodeIterator2 = xpathNodeIterator1.Current.Select(".//value");
                    while (xpathNodeIterator2.MoveNext() && xpathNodeIterator2.Current is not null)
                    {
                        string str = xpathNodeIterator2.Current.Value.Trim();
                        if (xpathNodeIterator1.Current.SelectSingleNode("datatype")?.Value == FieldDataType.DateTime.ToString() && DateTime.TryParse(str, out DateTime result))
                            str = result.ToString("MMM d, yyyy");
                        stringBuilder.Append(sendEmail._dictionaryHelper.GetText(str).Replace("\n", "<br/>")).Append("<br/>");
                        if (!(sendEmail.Attachment != true.ToString()) && str.Contains("/forms/upload") && sendEmail.TryCreateAttachment(sendEmail._mediaFileManager.FileSystem, str, out EmailMessageAttachment? attachment))
                            attachments.Add(attachment);
                    }
                    stringBuilder.Append("</dd>");
                }
                stringBuilder.Append("</dl>");
                string str1 = "<p>" + sendEmail.Message.Replace("\n", "<br/>") + "</p>" + stringBuilder.ToString();
                await sendEmail._workflowEmailService.SendEmailAsync(new SendEmailArgs()
                {
                    SenderEmail = sendEmail.SenderEmail,
                    RecipientEmail = sendEmail.Email,
                    CcEmail = sendEmail.CcEmail,
                    BccEmail = sendEmail.BccEmail,
                    Subject = sendEmail.Subject,
                    Body = str1,
                    ReplyToEmail = sendEmail.ReplyToEmail,
                    Attachments = attachments
                }).ConfigureAwait(false);
                foreach (EmailMessageAttachment messageAttachment in attachments)
                    await messageAttachment.Stream.DisposeAsync().ConfigureAwait(false);
                return WorkflowExecutionStatus.Completed;
            }
            catch (Exception ex)
            {
                sendEmail._logger.LogError(ex, "There was a problem sending an email to {Email} from Workflow for Form {FormName} with id {FormId} for Record with unique id {RecordId}", sendEmail.Email, context.Form.Name, context.Form.Id, context.Record.UniqueId);
                return WorkflowExecutionStatus.Failed;
            }
        }
    }
}