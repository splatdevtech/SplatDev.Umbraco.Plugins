using FormBuilder.Core.Configuration;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Exceptions;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;
using FormBuilder.Core.Workflows;
using FormBuilder.Web.Behaviors.Interfaces;
using FormBuilder.Web.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Security;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a form scaffold.
    /// </summary>
    public class GetScaffoldFormController : FormControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly FormDesignSettings _formDesignSettings;
        private readonly IFormTemplateStorage _formTemplateStorage;
        private readonly IApplyDefaultWorkflowsBehavior _applyDefaultWorkflowsBehavior;
        private readonly IApplyDefaultFieldsBehavior _applyDefaultFieldsBehavior;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<GetScaffoldFormController> _logger;
        private readonly IIOHelper _ioHelper;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly FieldCollection _fieldCollection;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public GetScaffoldFormController(
          IFormService formService,
          IFolderService folderService,
          IWorkflowService workflowService,
          IFieldService fieldService,
          IFieldTypeStorage fieldTypeStorage,
          WorkflowCollection workflowCollection,
          IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
          IFormsSecurity formsSecurity,
          IUmbracoMapper mapper,
          ILogger<GetByKeyFormController> logger,
          IHtmlSanitizer htmlSanitizer,
          IHostEnvironment hostEnvironment,
          IOptions<FormDesignSettings> formDesignSettings,
          IFormTemplateStorage formTemplateStorage,
          IApplyDefaultWorkflowsBehavior applyDefaultWorkflowsBehavior,
          IApplyDefaultFieldsBehavior applyDefaultFieldsBehavior,
          ILoggerFactory loggerFactory,
          IIOHelper ioHelper,
          Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
          FieldCollection fieldCollection)
          : base(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, logger, htmlSanitizer)
        {
            _hostEnvironment = hostEnvironment;
            _formDesignSettings = formDesignSettings.Value;
            _formTemplateStorage = formTemplateStorage;
            _applyDefaultWorkflowsBehavior = applyDefaultWorkflowsBehavior;
            _applyDefaultFieldsBehavior = applyDefaultFieldsBehavior;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<GetScaffoldFormController>();
            _ioHelper = ioHelper;
            _hostingEnvironment = hostingEnvironment;
            _fieldCollection = fieldCollection;
        }

        /// <summary>Management API endpoint for retrieving a form scaffold.</summary>
        [HttpGet("scaffold")]
        [ProducesResponseType(typeof(FormDesign), 200)]
        [ProducesResponseType(403)]
        public IActionResult GetScaffold() => GetScaffold(string.Empty);

        /// <summary>
        /// Management API endpoint for retrieving a form scaffold from a template.
        /// </summary>
        [HttpGet("scaffold/{template}")]
        [ProducesResponseType(typeof(FormDesign), 200)]
        [ProducesResponseType(403)]
        public IActionResult GetScaffold([FromRoute] string template)
        {
            FormDesign formDesign = new(_formDesignSettings.Defaults)
            {
                Id = Guid.NewGuid()
            };
            FormDesign form1 = formDesign;
            if (!string.IsNullOrEmpty(template) && template != "undefined")
            {
                (Form? Form, IDictionary<FormState, IEnumerable<Workflow>> Workflows) templateWithWorkflows = _formTemplateStorage.GetTemplateWithWorkflows(template);
                Form? form2 = templateWithWorkflows.Form;
                if (form2 is not null)
                {
                    PrepareFormCreatedFromTemplate(form2, out RegenerateFormStructureIdsResult regenerateFormStructureIdsResult);
                    form1.Name = form2.Name;
                    form1.AutocompleteAttribute = form2.AutocompleteAttribute;
                    form1.CssClass = form2.CssClass;
                    form1.DisableDefaultStylesheet = form2.DisableDefaultStylesheet;
                    form1.FieldIndicationType = form2.FieldIndicationType;
                    form1.GoToPageOnSubmit = form2.GoToPageOnSubmit;
                    form1.HideFieldValidation = form2.HideFieldValidation;
                    form1.Indicator = form2.Indicator;
                    form1.InvalidErrorMessage = form2.InvalidErrorMessage;
                    form1.ManualApproval = form2.ManualApproval;
                    form1.MessageOnSubmit = form2.MessageOnSubmit;
                    form1.MessageOnSubmitIsHtml = form2.MessageOnSubmitIsHtml;
                    form1.NextLabel = form2.NextLabel;
                    form1.PagingDetailsFormat = form2.PagingDetailsFormat;
                    form1.PageCaptionFormat = form2.PageCaptionFormat;
                    form1.Pages = form2.Pages;
                    form1.PrevLabel = form2.PrevLabel;
                    form1.RequiredErrorMessage = form2.RequiredErrorMessage;
                    form1.ShowPagingOnMultiPageForms = form2.ShowPagingOnMultiPageForms;
                    form1.ShowSummaryPageOnMultiPageForms = form2.ShowSummaryPageOnMultiPageForms;
                    form1.ShowValidationSummary = form2.ShowValidationSummary;
                    form1.StoreRecordsLocally = form2.StoreRecordsLocally;
                    form1.SubmitLabel = form2.SubmitLabel;
                    form1.SummaryLabel = form2.SummaryLabel;
                    form1.XPathOnSubmit = form2.XPathOnSubmit;
                    form1.FormWorkflows = new FormWorkflows();
                    LoadWorkflowsForTemplate(form1.FormWorkflows, templateWithWorkflows.Workflows, regenerateFormStructureIdsResult.FieldIdMapping);
                    return Ok(form1);
                }
            }
            form1.FormWorkflows = new FormWorkflows()
            {
                OnSubmit = [],
                OnApprove = [],
                OnReject = []
            };
            _applyDefaultWorkflowsBehavior.ApplyDefaultWorkflows(form1);
            _applyDefaultFieldsBehavior.ApplyDefaultFields(form1);
            return Ok(form1);
        }

        private void PrepareFormCreatedFromTemplate(
          Form templateForm,
          out RegenerateFormStructureIdsResult regenerateFormStructureIdsResult)
        {
            AddDataConsentField(templateForm);
            regenerateFormStructureIdsResult = templateForm.RegenerateFormStructureIds();
        }

        private void LoadWorkflowsForTemplate(
          FormWorkflows formWorkflows,
          IDictionary<FormState, IEnumerable<Workflow>> workflows,
          Dictionary<Guid, Guid> fieldIdMapping)
        {
            formWorkflows.OnSubmit = LoadWorkflowsForTemplate(FormState.Submitted, workflows, fieldIdMapping);
            formWorkflows.OnApprove = LoadWorkflowsForTemplate(FormState.Approved, workflows, fieldIdMapping);
            formWorkflows.OnReject = LoadWorkflowsForTemplate(FormState.Rejected, workflows, fieldIdMapping);
        }

        private List<FormWorkflowWithTypeSettings> LoadWorkflowsForTemplate(
          FormState formState,
          IDictionary<FormState, IEnumerable<Workflow>> workflows,
          Dictionary<Guid, Guid> fieldIdMapping)
        {
            List<FormWorkflowWithTypeSettings> withTypeSettingsList = [];
            if (!workflows.TryGetValue(formState, out IEnumerable<Workflow>? workflows1))
                return withTypeSettingsList;
            foreach (Workflow workflow in workflows1)
            {
                WorkflowType? workflowType = GetWorkflowType(workflow.WorkflowTypeId);
                if (workflowType is null)
                {
                    _logger.LogWarning("Could not load workflow type from template with Id {WorkflowTypeId}. Workflow will be skipped.", workflow.WorkflowTypeId);
                }
                else
                {
                    List<SettingWithValue> settingsWithValues = workflowType.GetSettingsWithValues(_formDesignSettings.SettingsCustomization.WorkflowTypes.GetValueForProviderType(workflowType), workflow);
                    FormWorkflowWithTypeSettings withTypeSettings = new()
                    {
                        Active = workflow.Active,
                        Condition = workflow.Condition,
                        Form = workflow.Form,
                        Id = Guid.NewGuid(),
                        IncludeSensitiveData = workflow.IncludeSensitiveData,
                        IsMandatory = workflow.IsMandatory,
                        Name = workflow.Name,
                        SortOrder = workflow.SortOrder,
                        WorkflowTypeId = workflow.WorkflowTypeId,
                        WorkflowTypeDescription = workflowType.Description,
                        WorkflowTypeGroup = workflowType.Group,
                        WorkflowTypeIcon = workflowType.Icon,
                        WorkflowTypeName = workflowType.Name,
                        Settings = settingsWithValues.ToDictionary(x => x.Alias, x => x.Value)
                    };
                    if (withTypeSettings.Condition is not null)
                    {
                        foreach (FieldConditionRule rule in withTypeSettings.Condition.Rules)
                        {
                            if (fieldIdMapping.TryGetValue(rule.Field, out Guid value))
                                rule.Field = value;
                        }
                    }
                    withTypeSettingsList.Add(withTypeSettings);
                }
            }
            return withTypeSettingsList;
        }

        private WorkflowType? GetWorkflowType(Guid workflowTypeId)
        {
            try
            {
                return WorkflowCollection[workflowTypeId];
            }
            catch (ProviderException)
            {
                return null;
            }
        }

        private void AddDataConsentField(Form form)
        {
            if (_formDesignSettings.DisableAutomaticAdditionOfDataConsentField || form.AllFields.Any(x => x.Alias == "dataConsent"))
                return;
            form.Pages.Last().FieldSets.Last().Containers.Last().AddDataConsentField(_formDesignSettings, _fieldCollection);
        }

        private string GetDefaultEmailTemplatePath()
        {
            string defaultEmailTemplate = _formDesignSettings.DefaultEmailTemplate;
            if (!string.IsNullOrEmpty(defaultEmailTemplate) && EmailTemplateExists(defaultEmailTemplate))
                return defaultEmailTemplate;
            return !EmailTemplateExists("/Views/Partials/Forms/Emails", "Example-Template.cshtml") ? string.Empty : "Forms/Emails/Example-Template.cshtml";
        }

        private bool EmailTemplateExists(string relativePath)
        {
            string[] source = relativePath.Split('/');
            if (source.Length == 0)
                return false;
            int num = source.Length == 1 ? 1 : 0;
            return EmailTemplateExists("~/Views/Partials/" + (num != 0 ? string.Empty : string.Join("/", source.Take(source.Length - 1))), num != 0 ? source[0] : source.LastOrDefault() ?? string.Empty);
        }

        private bool EmailTemplateExists(string folder, string fileName) => CreateEmailFileSystem(_hostEnvironment.MapPathContentRoot(folder), folder).FileExists(fileName);

        private PhysicalFileSystem CreateEmailFileSystem(
          string rootPath,
          string rootUrl)
        {
            return new PhysicalFileSystem(_ioHelper, _hostingEnvironment, _loggerFactory.CreateLogger<PhysicalFileSystem>(), rootPath, rootUrl);
        }
    }
}