using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Helpers;
using FormBuilder.Core.Services;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Workflows;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Xml;

using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;

namespace FormBuilder.Core.Providers.WorkflowTypes
{
    /// <summary>
    /// Provides a     /// </summary>
    public class SendXsltEmail : BaseEmailWorkflowType
    {
        private readonly IXmlService _xmlService;
        private readonly IWorkflowEmailService _workflowEmailService;
        private readonly ILogger<SendXsltEmail> _logger;
        private readonly MediaFileManager _mediaFileManager;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public SendXsltEmail(
          IHostingEnvironment hostingEnvironment,
          IOptions<GlobalSettings> globalSettings,
          IXmlService xmlService,
          IWorkflowEmailService workflowEmailService,
          ILogger<SendXsltEmail> logger,
          MediaFileManager mediaFileManager)
          : base(hostingEnvironment, globalSettings)
        {
            Id = new Guid("616EDFEB-BADF-414B-89DC-D8655EB85998");
            Name = "Send XSLT transformed email";
            Alias = "sendEmailWithXsltTemplate";
            Description = "Send the result of the form to an email address";
            Icon = "icon-message";
            Group = "Email";
            _xmlService = xmlService;
            _workflowEmailService = workflowEmailService;
            _logger = logger;
            _mediaFileManager = mediaFileManager;
        }

        /// <summary>
        /// Gets or sets an XSLT file to transform the record's XML.
        /// </summary>
        [Setting("XSLT File", Description = "Transform the XML before sending the email", DisplayOrder = 70, View = "Umb.PropertyEditorUi.MediaEntityPicker")]
        public virtual string XsltFile { get; set; } = string.Empty;

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = base.ValidateSettings();
            if (!string.IsNullOrEmpty(XsltFile) && !XsltFile.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase))
                exceptionList.Add(new Exception("'XSLT File' setting has not been set correctly (a file with an .xslt extension must be selected)."));
            return exceptionList;
        }

        /// <inheritdoc />
        public override async Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            SendXsltEmail sendXsltEmail = this;
            try
            {
                var xml = sendXsltEmail._xmlService.ToXml(context.Record, new XmlDocument()).OuterXml;
                if (!string.IsNullOrEmpty(sendXsltEmail.XsltFile))
                    xml = XsltHelper.TransformXML(sendXsltEmail._xmlService.ToXml(context.Record, new XmlDocument()).OuterXml, sendXsltEmail.XsltFile, sendXsltEmail._mediaFileManager);
                await sendXsltEmail._workflowEmailService.SendEmailAsync(new SendEmailArgs()
                {
                    SenderEmail = sendXsltEmail.SenderEmail,
                    RecipientEmail = sendXsltEmail.Email,
                    CcEmail = sendXsltEmail.CcEmail,
                    BccEmail = sendXsltEmail.BccEmail,
                    Subject = sendXsltEmail.Subject,
                    Body = sendXsltEmail._xmlService.ToXml(context.Record, new XmlDocument()).OuterXml,
                    ReplyToEmail = sendXsltEmail.ReplyToEmail
                });
                return WorkflowExecutionStatus.Completed;
            }
            catch (Exception ex)
            {
                sendXsltEmail._logger.LogError(ex, "There was a problem sending an XSLT email to {Email} from Workflow for Form {FormName} with id {FormId} for Record with unique id {RecordId}", sendXsltEmail.Email, context.Form.Name, context.Form.Id, context.Record.UniqueId);
                return WorkflowExecutionStatus.Failed;
            }
        }
    }
}