using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Workflows;
using FormBuilder.Web.Api.Management.Controllers;
using FormBuilder.Web.Extensions;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.Email;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Templates;
using Umbraco.Cms.Core.Web;

namespace FormBuilder.Core.Providers.WorkflowTypes
{
    /// <summary>
    /// Provides a     /// </summary>
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
        private readonly IFieldPrevalueSourceTypeService _fieldPreValueSourceTypeService;
        private readonly HtmlLocalLinkParser _htmlLocalLinkParser;
        private readonly EmailTemplateCollection _emailTemplateCollection;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
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
          IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService,
          HtmlLocalLinkParser htmlLocalLinkParser,
          EmailTemplateCollection emailTemplateCollection)
          : base(hostingEnvironment, globalSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _fieldTypeStorage = fieldTypeStorage;
            _formService = formService;
            _umbracoContextAccessor = umbracoContextAccessor;
            _pageService = pageService;
            _workflowEmailService = workflowEmailService;
            _mediaFileManager = mediaFileManager;
            _fileSystems = fileSystems;
            _publishedUrlProvider = publishedUrlProvider;
            _logger = logger;
            _placeholderParsingService = placeholderParsingService;
            _prevalueSourceService = prevalueSourceService;
            _fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
            _htmlLocalLinkParser = htmlLocalLinkParser;
            _emailTemplateCollection = emailTemplateCollection;
            Id = new Guid("17C61629-D984-4E86-B43B-A8407B3EFEA9");
            Name = "Send email with template (Razor)";
            Alias = "sendEmailWithRazorTemplate";
            Description = "Send the result of the form to an email address/addresses using a Razor .cshtml template";
            Icon = "icon-message";
            Group = "Email";
        }

        /// <summary>
        /// Gets or sets a path to the razor view used for generating the email.
        /// </summary>
        [Setting("Email Template", Description = "The path to the Razor view that you want to use for generating the email. Email templates are stored at /Views/Partials/Forms/Emails", DisplayOrder = 70, IsMandatory = true, View = "Forms.PropertyEditorUi.EmailTemplatePicker")]
        public virtual string RazorViewFilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the form field's formatted text to be used in the header of the email.
        /// </summary>
        [Setting("Header text", Description = "Enter formatted text to be rendered in the email header.", DisplayOrder = 80, HtmlEncodeReplacedPlaceholderValues = true, SupportsHtml = true, SupportsPlaceholders = true, View = "Umb.PropertyEditorUi.Tiptap")]
        public virtual string HeaderHtml { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the form field's formatted text to be used in the header of the email.
        /// </summary>
        [Setting("Body text", Description = "Enter formatted text to be rendered in the email body", DisplayOrder = 90, HtmlEncodeReplacedPlaceholderValues = true, SupportsHtml = true, SupportsPlaceholders = true, View = "Umb.PropertyEditorUi.Tiptap")]
        public virtual string BodyHtml { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the form field's formatted text to be used in the header of the email.
        /// </summary>
        [Setting("Footer text", Description = "Enter formatted text to be rendered in the email footer", DisplayOrder = 100, HtmlEncodeReplacedPlaceholderValues = true, SupportsHtml = true, SupportsPlaceholders = true, View = "Umb.PropertyEditorUi.Tiptap")]
        public virtual string FooterHtml { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to attach file uploads to the email.
        /// </summary>
        [Setting("Attachments", Description = "Attach file uploads to email", DisplayOrder = 110, View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string Attachment { get; set; } = string.Empty;

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = base.ValidateSettings();
            if (string.IsNullOrWhiteSpace(RazorViewFilePath))
                exceptionList.Add(new ArgumentNullException("RazorViewFilePath", "'Email Template' setting has not been set'"));
            return exceptionList;
        }

        /// <inheritdoc />
        public override async Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            SendRazorEmail sendRazorEmail = this;
            if (context.Record is null)
            {
                sendRazorEmail._logger.LogError(new ArgumentNullException(nameof(context)), "Record is null");
                return WorkflowExecutionStatus.Failed;
            }
            List<Exception> source = sendRazorEmail.ValidateSettings();
            if (source is not null && source.Count != 0)
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
            if (!sendRazorEmail._emailTemplateCollection.Any(template => template.FileName == sendRazorEmail.RazorViewFilePath)
                && !sendRazorEmail._fileSystems.PartialViewsFileSystem.FileExists(sendRazorEmail.RazorViewFilePath))
            {
                FileNotFoundException notFoundException = new("Razor view email template not found", sendRazorEmail.RazorViewFilePath);
                sendRazorEmail._logger.LogError(notFoundException, "Razor view email template not found {RazorViewFilePath}", sendRazorEmail.RazorViewFilePath);
                return WorkflowExecutionStatus.Failed;
            }

            try
            {
                string str = await sendRazorEmail.ParseWithRazorView(context, "/Views/Partials/" + sendRazorEmail.RazorViewFilePath).ConfigureAwait(false);
                List<EmailMessageAttachment> attachments = [];
                if (sendRazorEmail.Attachment == true.ToString())
                {
                    IList<Guid>? uploadFieldTypeIds = sendRazorEmail.GetUploadFieldTypeIdsUsedInRecord(context.Record);
                    if (uploadFieldTypeIds is not null && uploadFieldTypeIds.Count > 0)
                    {
                        foreach (KeyValuePair<Guid, RecordField> keyValuePair in context.Record.RecordFields.Where(x => uploadFieldTypeIds.Contains(x.Value.Field!.FieldTypeId)))
                        {
                            if (keyValuePair.Value.HasValue())
                            {
                                foreach (object obj in keyValuePair.Value.Values)
                                {
                                    string? filePath = obj?.ToString();
                                    if (!string.IsNullOrEmpty(filePath))
                                        if (sendRazorEmail.TryCreateAttachment(sendRazorEmail._mediaFileManager.FileSystem, filePath, out EmailMessageAttachment? attachment))
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

        private IList<Guid>? GetUploadFieldTypeIdsUsedInRecord(Record record)
        {
            ArgumentNullException.ThrowIfNull(record);
            IList<Guid>? list = (record.RecordFields.Select(x => x.Value.Field)?.Select(_fieldTypeStorage.GetFieldTypeByField!)?.Where(x => x is not null && x.SupportsUploadTypes)?.Select(x => x!.Id))?.ToList();

            return list is not null ? [.. list] : [];
        }

        /// <summary>
        /// Parses a record with a Razor view to generate an HTML output to use in for instance an email.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="razorViewFilePath">A relative path to a Razor view.</param>
        /// <returns>The generated output string usually HTML.</returns>
        private async Task<string> ParseWithRazorView(
          WorkflowExecutionContext context,
          string razorViewFilePath)
        {
            if (string.IsNullOrWhiteSpace(razorViewFilePath))
            {
                ArgumentNullException argumentNullException = new(nameof(razorViewFilePath));
                _logger.LogError(argumentNullException, "RazorFilePath cannot be null or empty");
                throw argumentNullException;
            }
            if (context.Record.RecordFields.Values.Any(x => string.IsNullOrWhiteSpace(x.Alias)))
            {
                string message = "An alias without a value was in the recordfields. This should be fixed.";
                InvalidOperationException operationException = new(message);
                _logger.LogError(operationException, message);
                throw operationException;
            }
            _httpContextAccessor.HttpContext!.Items["pageElements"] = _pageService.GetPageElements(context.Record.UmbracoPageId);
            FormFieldHtmlModel[] array = [.. context.Record.RecordFields.Select(recordField =>
            {
                FormFieldHtmlModel withRazorView = new(recordField.Value.Alias);
                FormFieldHtmlModel formFieldHtmlModel1 = withRazorView;
                RecordField recordField1 = recordField.Value;
                Guid guid = recordField1 is not null ? recordField1.FieldId : Guid.Empty;
                formFieldHtmlModel1.Id = guid;
                withRazorView.Name = _placeholderParsingService.ParsePlaceHolders(recordField.Value?.Field?.Caption ?? string.Empty, false, context.Record);
                withRazorView.FieldType = recordField.Value?.Field == null ? string.Empty : _fieldTypeStorage.GetFieldTypeByField(recordField.Value.Field)?.FieldTypeViewName ?? string.Empty;
                FormFieldHtmlModel formFieldHtmlModel2 = withRazorView;
                object[] objArray;
                if (recordField.Value == null || !recordField.Value.HasValue())
                    objArray = [string.Empty];
                else
                    objArray = [.. recordField.Value.Values];
                formFieldHtmlModel2.FieldValue = objArray;
                return withRazorView;
            })];
            if (_httpContextAccessor.HttpContext == null || !_umbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? _))
            {
                string message = "Context is not available to parse the record";
                InvalidOperationException operationException = new(message);
                _logger.LogError(operationException, message);
                throw operationException;
            }
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            FormsHtmlModel model = await CreateModelForTemplate(context.Record, array).ConfigureAwait(false);
            ActionContext context1 = new(httpContext, new RouteData()
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
            RazorEmailViewController controller = new()
            {
                ControllerContext = new ControllerContext(context1)
            };
            string withRazorView1 = await controller.RenderViewAsync(razorViewFilePath, model).ConfigureAwait(false);
            httpContext = null;
            return withRazorView1;
        }

        private async Task<FormsHtmlModel> CreateModelForTemplate(
          Record record,
          FormFieldHtmlModel[] fields)
        {
            FormsHtmlModel? model = new(fields)
            {
                FormId = record.Form,
                EntryUniqueId = record.UniqueId,
                FormSubmittedOn = record.Created
            };
            Form? form = _formService.Get(record.Form);
            if (form is not null)
            {
                model.FormName = form.Name;
                FormsHtmlModel? formsHtmlModel = model;
                formsHtmlModel.PrevalueMaps = await form.GetPrevalueMaps(_fieldTypeStorage, _prevalueSourceService, _fieldPreValueSourceTypeService).ConfigureAwait(false);
            }
            model.HeaderHtml = GetHtmlContent(HeaderHtml);
            model.BodyHtml = GetHtmlContent(BodyHtml);
            model.FooterHtml = GetHtmlContent(FooterHtml);
            model.WorkflowSettings = GetWorflowSettings();
            if (record.UmbracoPageId > 0)
            {
                model.FormPageId = record.UmbracoPageId;
                model.FormPageUrl = _publishedUrlProvider.GetUrl(record.UmbracoPageId, UrlMode.Absolute);
            }
            FormsHtmlModel modelForTemplate = model;
            return modelForTemplate;
        }

        private HtmlString? GetHtmlContent(string value) => string.IsNullOrWhiteSpace(value) ? null : new HtmlString(_htmlLocalLinkParser.EnsureInternalLinks(value));

        private Dictionary<string, string> GetWorflowSettings() => GetType().GetProperties().Where(x => Attribute.IsDefined(x, typeof(SettingAttribute))).ToDictionary(x => x.Name, x => x.GetValue(this) is string str ? str : string.Empty);
    }
}