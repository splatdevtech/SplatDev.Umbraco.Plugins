
// Type: Umbraco.Forms.Core.Providers.WorkflowTypes.SendRazorEmail
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Reflection;

using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.Email;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Templates;
using Umbraco.Cms.Core.Web;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Controllers;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Extensions;


#nullable enable
namespace Umbraco.Forms.Core.Providers.WorkflowTypes
{
    public class SendRazorEmail : BaseEmailWorkflowType
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFieldTypeStorage _fieldTypeStorage;
        private readonly IFormService _formService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IPageService _pageService;
        private readonly IWorkflowEmailService _workflowEmailService;
        private readonly MediaFileManager _mediaFileManager;
        private readonly FileSystems _fileSystems;
        private readonly IPublishedUrlProvider _publishedUrlProvider;
        private readonly ILogger<SendRazorEmail> _logger;
        private readonly IPlaceholderParsingService _placeholderParsingService;
        private readonly IPrevalueSourceService _prevalueSourceService;
        private readonly IFieldPreValueSourceTypeService _fieldPreValueSourceTypeService;
        private readonly HtmlLocalLinkParser _htmlLocalLinkParser;
        private readonly EmailTemplateCollection _emailTemplateCollection;

        public SendRazorEmail(
          IHostingEnvironment hostingEnvironment,
          IOptions<GlobalSettings> globalSettings,
          IHttpContextAccessor httpContextAccessor,
          IFieldTypeStorage fieldTypeStorage,
          IFormService formService,
          IUmbracoContextAccessor umbracoContextAccessor,
          IPageService pageService,
          IWorkflowEmailService workflowEmailService,
          MediaFileManager mediaFileManager,
          FileSystems fileSystems,
          IPublishedUrlProvider publishedUrlProvider,
          ILogger<SendRazorEmail> logger,
          IPlaceholderParsingService placeholderParsingService,
          IPrevalueSourceService prevalueSourceService,
          IFieldPreValueSourceTypeService fieldPreValueSourceTypeService,
          HtmlLocalLinkParser htmlLocalLinkParser,
          EmailTemplateCollection emailTemplateCollection)
          : base(hostingEnvironment, globalSettings)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._fieldTypeStorage = fieldTypeStorage;
            this._formService = formService;
            this._umbracoContextAccessor = umbracoContextAccessor;
            this._pageService = pageService;
            this._workflowEmailService = workflowEmailService;
            this._mediaFileManager = mediaFileManager;
            this._fileSystems = fileSystems;
            this._publishedUrlProvider = publishedUrlProvider;
            this._logger = logger;
            this._placeholderParsingService = placeholderParsingService;
            this._prevalueSourceService = prevalueSourceService;
            this._fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
            this._htmlLocalLinkParser = htmlLocalLinkParser;
            this._emailTemplateCollection = emailTemplateCollection;
            this.Id = new Guid("17C61629-D984-4E86-B43B-A8407B3EFEA9");
            this.Name = "Send email with template (Razor)";
            this.Alias = "sendEmailWithRazorTemplate";
            this.Description = "Send the result of the form to an email address/addresses using a Razor .cshtml template";
            this.Icon = "icon-message";
            this.Group = "Email";
        }

        [Setting("Email Template", Description = "The path to the Razor view that you want to use for generating the email. Email templates are stored at /Views/Partials/Forms/Emails", DisplayOrder = 70, IsMandatory = true, View = "Forms.PropertyEditorUi.EmailTemplatePicker")]
        public virtual string RazorViewFilePath { get; set; } = string.Empty;

        [Setting("Header text", Description = "Enter formatted text to be rendered in the email header.", DisplayOrder = 80, HtmlEncodeReplacedPlaceholderValues = true, SupportsHtml = true, SupportsPlaceholders = true, View = "Umb.PropertyEditorUi.Tiptap")]
        public virtual string HeaderHtml { get; set; } = string.Empty;

        [Setting("Body text", Description = "Enter formatted text to be rendered in the email body", DisplayOrder = 90, HtmlEncodeReplacedPlaceholderValues = true, SupportsHtml = true, SupportsPlaceholders = true, View = "Umb.PropertyEditorUi.Tiptap")]
        public virtual string BodyHtml { get; set; } = string.Empty;

        [Setting("Footer text", Description = "Enter formatted text to be rendered in the email footer", DisplayOrder = 100, HtmlEncodeReplacedPlaceholderValues = true, SupportsHtml = true, SupportsPlaceholders = true, View = "Umb.PropertyEditorUi.Tiptap")]
        public virtual string FooterHtml { get; set; } = string.Empty;

        [Setting("Attachments", Description = "Attach file uploads to email", DisplayOrder = 110, View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string Attachment { get; set; } = string.Empty;

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = ValidateSettings();
            if (string.IsNullOrWhiteSpace(this.RazorViewFilePath))
                exceptionList.Add(new ArgumentNullException("RazorViewFilePath", "'Email Template' setting has not been set'"));
            return exceptionList;
        }

        public override async Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            SendRazorEmail sendRazorEmail = this;
            if (context.Record == null)
            {
                ArgumentNullException argumentNullException = new ArgumentNullException("record");
                sendRazorEmail._logger.LogError(argumentNullException, "Record is null");
                return WorkflowExecutionStatus.Failed;
            }
            List<Exception> source = sendRazorEmail.ValidateSettings();
            if (source != null && source.Any<Exception>())
            {
                foreach (Exception exception in source)
                    sendRazorEmail._logger.LogError(exception, exception.Message);
                return WorkflowExecutionStatus.Failed;
            }
            if (sendRazorEmail._fileSystems.PartialViewsFileSystem == null)
            {
                sendRazorEmail._logger.LogError("Partial view file system is null.");
                return WorkflowExecutionStatus.Failed;
            }
            if (!sendRazorEmail._emailTemplateCollection.Any(template => template.FileName == sendRazorEmail.RazorViewFilePath) && !sendRazorEmail._fileSystems.PartialViewsFileSystem.FileExists(sendRazorEmail.RazorViewFilePath))
            {
                FileNotFoundException notFoundException = new FileNotFoundException("Razor view email template not found", sendRazorEmail.RazorViewFilePath);
                sendRazorEmail._logger.LogError(notFoundException, "Razor view email template not found {RazorViewFilePath}", sendRazorEmail.RazorViewFilePath);
                return WorkflowExecutionStatus.Failed;
            }
            try
            {
                string str = await sendRazorEmail.ParseWithRazorView(context, "/Views/Partials/" + sendRazorEmail.RazorViewFilePath).ConfigureAwait(false);
                List<EmailMessageAttachment> attachments = new List<EmailMessageAttachment>();
                if (sendRazorEmail.Attachment == true.ToString())
                {
                    IList<Guid> uploadFieldTypeIds = sendRazorEmail.GetUploadFieldTypeIdsUsedInRecord(context.Record);
                    if (uploadFieldTypeIds.Count > 0)
                    {
                        foreach (KeyValuePair<Guid, RecordField> keyValuePair in context.Record.RecordFields.Where<KeyValuePair<Guid, RecordField>>(x => uploadFieldTypeIds.Contains(x.Value.Field.FieldTypeId)))
                        {
                            if (keyValuePair.Value.HasValue())
                            {
                                foreach (object obj in keyValuePair.Value.Values)
                                {
                                    string filePath = obj.ToString();
                                    EmailMessageAttachment attachment;
                                    if (sendRazorEmail.TryCreateAttachment(sendRazorEmail._mediaFileManager.FileSystem, filePath, out attachment))
                                        attachments.Add(attachment);
                                }
                            }
                        }
                    }
                }
                await sendRazorEmail._workflowEmailService.SendEmailAsync(new SendEmailArgs()
                {
                    SenderEmail = sendRazorEmail.SenderEmail,
                    RecipientEmail = sendRazorEmail.Email,
                    CcEmail = sendRazorEmail.CcEmail,
                    BccEmail = sendRazorEmail.BccEmail,
                    Subject = sendRazorEmail.Subject,
                    Body = str,
                    ReplyToEmail = sendRazorEmail.ReplyToEmail,
                    Attachments = attachments
                }).ConfigureAwait(false);
                foreach (EmailMessageAttachment messageAttachment in attachments)
                    await messageAttachment.Stream.DisposeAsync().ConfigureAwait(false);
                return WorkflowExecutionStatus.Completed;
            }
            catch (Exception ex)
            {
                sendRazorEmail._logger.LogError(ex, "There was a problem sending a Razor email to {Email} from Workflow for Form {FormName} with id {FormId} for Record with unique id {RecordId}", sendRazorEmail.Email, context.Form.Name, context.Form.Id, context.Record.UniqueId);
                return WorkflowExecutionStatus.Failed;
            }
        }

        private IList<Guid> GetUploadFieldTypeIdsUsedInRecord(Record record) => record.RecordFields.Select<KeyValuePair<Guid, RecordField>, Field>(x => x.Value.Field).Select<Field, FieldType>(x => this._fieldTypeStorage.GetFieldTypeByField(x)).Where<FieldType>(x => x != null && x.SupportsUploadTypes).Select<FieldType, Guid>(x => x.Id).ToList<Guid>();

        private async Task<string> ParseWithRazorView(
          WorkflowExecutionContext context,
          string razorViewFilePath)
        {
            if (string.IsNullOrWhiteSpace(razorViewFilePath))
            {
                ArgumentNullException argumentNullException = new ArgumentNullException(nameof(razorViewFilePath));
                this._logger.LogError(argumentNullException, "RazorFilePath cannot be null or empty");
                throw argumentNullException;
            }
            if (context.Record.RecordFields.Values.Any<RecordField>(x => string.IsNullOrWhiteSpace(x.Alias)))
            {
                string message = "An alias without a value was in the recordfields. This should be fixed.";
                InvalidOperationException operationException = new InvalidOperationException(message);
                this._logger.LogError(operationException, message);
                throw operationException;
            }
            this._httpContextAccessor.HttpContext.Items["pageElements"] = this._pageService.GetPageElements(context.Record.UmbracoPageId);
            FormFieldHtmlModel[] array = context.Record.RecordFields.Select<KeyValuePair<Guid, RecordField>, FormFieldHtmlModel>(recordField =>
            {
                FormFieldHtmlModel withRazorView = new FormFieldHtmlModel(recordField.Value.Alias);
                FormFieldHtmlModel formFieldHtmlModel1 = withRazorView;
                RecordField recordField1 = recordField.Value;
                Guid guid = recordField1 != null ? recordField1.FieldId : Guid.Empty;
                formFieldHtmlModel1.Id = guid;
                withRazorView.Name = this._placeholderParsingService.ParsePlaceHolders(recordField.Value?.Field?.Caption ?? string.Empty, false, context);
                withRazorView.FieldType = recordField.Value?.Field == null ? string.Empty : this._fieldTypeStorage.GetFieldTypeByField(recordField.Value.Field)?.FieldTypeViewName ?? string.Empty;
                FormFieldHtmlModel formFieldHtmlModel2 = withRazorView;
                object[] objArray;
                if (recordField.Value == null || !recordField.Value.HasValue())
                    objArray = new object[1] { string.Empty };
                else
                    objArray = recordField.Value.Values.ToArray();
                formFieldHtmlModel2.FieldValue = objArray;
                return withRazorView;
            }).ToArray<FormFieldHtmlModel>();
            if (this._httpContextAccessor.HttpContext == null || !this._umbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext _))
            {
                string message = "Context is not available to parse the record";
                InvalidOperationException operationException = new InvalidOperationException(message);
                this._logger.LogError(operationException, message);
                throw operationException;
            }
            HttpContext httpContext = this._httpContextAccessor.HttpContext;
            FormsHtmlModel model = await this.CreateModelForTemplate(context.Record, array).ConfigureAwait(false);
            ActionContext context1 = new ActionContext(httpContext, new RouteData()
            {
                Values = {
          {
            "controller",
             "RazorEmailView"
          },
          {
            "action",
             "Index"
          }
        }
            }, new ControllerActionDescriptor());
            RazorEmailViewController controller = new RazorEmailViewController();
            controller.ControllerContext = new ControllerContext(context1);
            string withRazorView1 = await controller.RenderViewAsync<FormsHtmlModel>(razorViewFilePath, model).ConfigureAwait(false);
            httpContext = null;
            return withRazorView1;
        }

        private async Task<FormsHtmlModel> CreateModelForTemplate(
          Record record,
          FormFieldHtmlModel[] fields)
        {
            FormsHtmlModel model = new FormsHtmlModel(fields)
            {
                FormId = record.Form,
                EntryUniqueId = record.UniqueId,
                FormSubmittedOn = record.Created
            };
            Form form = this._formService.Get(record.Form);
            if (form != null)
            {
                model.FormName = form.Name;
                FormsHtmlModel formsHtmlModel = model;
                formsHtmlModel.PrevalueMaps = await form.GetPrevalueMaps(this._fieldTypeStorage, this._prevalueSourceService, this._fieldPreValueSourceTypeService).ConfigureAwait(false);
                formsHtmlModel = null;
            }
            model.HeaderHtml = this.GetHtmlContent(this.HeaderHtml);
            model.BodyHtml = this.GetHtmlContent(this.BodyHtml);
            model.FooterHtml = this.GetHtmlContent(this.FooterHtml);
            model.WorkflowSettings = this.GetWorflowSettings();
            if (record.UmbracoPageId > 0)
            {
                model.FormPageId = record.UmbracoPageId;
                model.FormPageUrl = this._publishedUrlProvider.GetUrl(record.UmbracoPageId, UrlMode.Absolute);
            }
            FormsHtmlModel modelForTemplate = model;
            model = null;
            return modelForTemplate;
        }

        private IHtmlContent? GetHtmlContent(string value) => string.IsNullOrWhiteSpace(value) ? null : (IHtmlContent)new HtmlString(this._htmlLocalLinkParser.EnsureInternalLinks(value));

        private IDictionary<string, string> GetWorflowSettings() => this.GetType().GetProperties().Where<PropertyInfo>(x => Attribute.IsDefined(x, typeof(SettingAttribute))).ToDictionary<PropertyInfo, string, string>(x => x.Name, x => x.GetValue(this) is string str ? str : string.Empty);
    }
}
