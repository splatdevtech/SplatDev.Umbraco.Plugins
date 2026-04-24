
// Type: Umbraco.Forms.Core.Providers.WorkflowTypes.SendXsltEmail
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Xml;

using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Data.Helpers;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.WorkflowTypes
{
    public class SendXsltEmail : BaseEmailWorkflowType
    {
        private readonly IXmlService _xmlService;
        private readonly IWorkflowEmailService _workflowEmailService;
        private readonly ILogger<SendXsltEmail> _logger;
        private readonly MediaFileManager _mediaFileManager;

        public SendXsltEmail(
          IHostingEnvironment hostingEnvironment,
          IOptions<GlobalSettings> globalSettings,
          IXmlService xmlService,
          IWorkflowEmailService workflowEmailService,
          ILogger<SendXsltEmail> logger,
          MediaFileManager mediaFileManager)
          : base(hostingEnvironment, globalSettings)
        {
            this.Id = new Guid("616EDFEB-BADF-414B-89DC-D8655EB85998");
            this.Name = "Send XSLT transformed email";
            this.Alias = "sendEmailWithXsltTemplate";
            this.Description = "Send the result of the form to an email address";
            this.Icon = "icon-message";
            this.Group = "Email";
            this._xmlService = xmlService;
            this._workflowEmailService = workflowEmailService;
            this._logger = logger;
            this._mediaFileManager = mediaFileManager;
        }

        [Setting("XSLT File", Description = "Transform the XML before sending the email", DisplayOrder = 70, View = "Umb.PropertyEditorUi.MediaEntityPicker")]
        public virtual string XsltFile { get; set; } = string.Empty;

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = ValidateSettings();
            if (!string.IsNullOrEmpty(this.XsltFile) && !this.XsltFile.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase))
                exceptionList.Add(new Exception("'XSLT File' setting has not been set correctly (a file with an .xslt extension must be selected)."));
            return exceptionList;
        }

        public override async Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            SendXsltEmail sendXsltEmail = this;
            try
            {
                string xml = sendXsltEmail._xmlService.ToXml(context.Record, new XmlDocument()).OuterXml;
                if (!string.IsNullOrEmpty(sendXsltEmail.XsltFile))
                    xml = XsltHelper.TransformXML(xml, sendXsltEmail.XsltFile, sendXsltEmail._mediaFileManager);
                await sendXsltEmail._workflowEmailService.SendEmailAsync(new SendEmailArgs()
                {
                    SenderEmail = sendXsltEmail.SenderEmail,
                    RecipientEmail = sendXsltEmail.Email,
                    CcEmail = sendXsltEmail.CcEmail,
                    BccEmail = sendXsltEmail.BccEmail,
                    Subject = sendXsltEmail.Subject,
                    Body = xml,
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
