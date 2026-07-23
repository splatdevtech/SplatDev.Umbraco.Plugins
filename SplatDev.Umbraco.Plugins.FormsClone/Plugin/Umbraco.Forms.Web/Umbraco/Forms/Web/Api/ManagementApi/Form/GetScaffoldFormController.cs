
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.GetScaffoldFormController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Security;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Exceptions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;
using Umbraco.Forms.Web.Behaviors;
using Umbraco.Forms.Web.Extensions;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form
{
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
            this._hostEnvironment = hostEnvironment;
            this._formDesignSettings = formDesignSettings.Value;
            this._formTemplateStorage = formTemplateStorage;
            this._applyDefaultWorkflowsBehavior = applyDefaultWorkflowsBehavior;
            this._applyDefaultFieldsBehavior = applyDefaultFieldsBehavior;
            this._loggerFactory = loggerFactory;
            this._logger = this._loggerFactory.CreateLogger<GetScaffoldFormController>();
            this._ioHelper = ioHelper;
            this._hostingEnvironment = hostingEnvironment;
            this._fieldCollection = fieldCollection;
        }

        [HttpGet("scaffold")]
        [ProducesResponseType(typeof(FormDesign), 200)]
        [ProducesResponseType(403)]
        public IActionResult GetScaffold() => this.GetScaffold(string.Empty);

        [HttpGet("scaffold/{template}")]
        [ProducesResponseType(typeof(FormDesign), 200)]
        [ProducesResponseType(403)]
        public IActionResult GetScaffold([FromRoute] string template)
        {
            FormDesign formDesign = new FormDesign(this._formDesignSettings.Defaults);
            formDesign.Id = Guid.NewGuid();
            FormDesign form1 = formDesign;
            if (!string.IsNullOrEmpty(template) && template != "undefined")
            {
                (Umbraco.Forms.Core.Models.Form Form, IDictionary<FormState, IEnumerable<Umbraco.Forms.Core.Models.Workflow>> Workflows) templateWithWorkflows = this._formTemplateStorage.GetTemplateWithWorkflows(template);
                Umbraco.Forms.Core.Models.Form form2 = templateWithWorkflows.Form;
                if (form2 != null)
                {
                    RegenerateFormStructureIdsResult regenerateFormStructureIdsResult;
                    this.PrepareFormCreatedFromTemplate(form2, out regenerateFormStructureIdsResult);
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
                    this.LoadWorkflowsForTemplate(form1.FormWorkflows, templateWithWorkflows.Workflows, regenerateFormStructureIdsResult.FieldIdMapping);
                    return this.Ok(form1);
                }
            }
            form1.FormWorkflows = new FormWorkflows()
            {
                OnSubmit = new List<FormWorkflowWithTypeSettings>(),
                OnApprove = new List<FormWorkflowWithTypeSettings>(),
                OnReject = new List<FormWorkflowWithTypeSettings>()
            };
            this._applyDefaultWorkflowsBehavior.ApplyDefaultWorkflows(form1);
            this._applyDefaultFieldsBehavior.ApplyDefaultFields(form1);
            return this.Ok(form1);
        }

        private void PrepareFormCreatedFromTemplate(
          Umbraco.Forms.Core.Models.Form templateForm,
          out RegenerateFormStructureIdsResult regenerateFormStructureIdsResult)
        {
            this.AddDataConsentField(templateForm);
            regenerateFormStructureIdsResult = templateForm.RegenerateFormStructureIds();
        }

        private void LoadWorkflowsForTemplate(
          FormWorkflows formWorkflows,
          IDictionary<FormState, IEnumerable<Umbraco.Forms.Core.Models.Workflow>> workflows,
          Dictionary<Guid, Guid> fieldIdMapping)
        {
            formWorkflows.OnSubmit = this.LoadWorkflowsForTemplate(FormState.Submitted, workflows, fieldIdMapping);
            formWorkflows.OnApprove = this.LoadWorkflowsForTemplate(FormState.Approved, workflows, fieldIdMapping);
            formWorkflows.OnReject = this.LoadWorkflowsForTemplate(FormState.Rejected, workflows, fieldIdMapping);
        }

        private List<FormWorkflowWithTypeSettings> LoadWorkflowsForTemplate(
          FormState formState,
          IDictionary<FormState, IEnumerable<Umbraco.Forms.Core.Models.Workflow>> workflows,
          Dictionary<Guid, Guid> fieldIdMapping)
        {
            List<FormWorkflowWithTypeSettings> withTypeSettingsList = new List<FormWorkflowWithTypeSettings>();
            IEnumerable<Umbraco.Forms.Core.Models.Workflow> workflows1;
            if (!workflows.TryGetValue(formState, out workflows1))
                return withTypeSettingsList;
            foreach (Umbraco.Forms.Core.Models.Workflow workflow in workflows1)
            {
                Umbraco.Forms.Core.WorkflowType workflowType = this.GetWorkflowType(workflow.WorkflowTypeId);
                if (workflowType == null)
                {
                    this._logger.LogWarning("Could not load workflow type from template with Id {WorkflowTypeId}. Workflow will be skipped.", workflow.WorkflowTypeId);
                }
                else
                {
                    List<SettingWithValue> settingsWithValues = workflowType.GetSettingsWithValues(this._hostingEnvironment, this._formDesignSettings.SettingsCustomization.WorkflowTypes.GetValueForProviderType(workflowType), workflow);
                    FormWorkflowWithTypeSettings withTypeSettings = new FormWorkflowWithTypeSettings()
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
                        Settings = settingsWithValues.ToDictionary<SettingWithValue, string, string>(x => x.Alias, x => x.Value)
                    };
                    if (withTypeSettings.Condition != null)
                    {
                        foreach (FieldConditionRule rule in withTypeSettings.Condition.Rules)
                        {
                            if (fieldIdMapping.ContainsKey(rule.Field))
                                rule.Field = fieldIdMapping[rule.Field];
                        }
                    }
                    withTypeSettingsList.Add(withTypeSettings);
                }
            }
            return withTypeSettingsList;
        }

        private Umbraco.Forms.Core.WorkflowType? GetWorkflowType(Guid workflowTypeId)
        {
            try
            {
                return this.WorkflowCollection[workflowTypeId];
            }
            catch (ProviderException)
            {
                return null;
            }
        }

        private void AddDataConsentField(Umbraco.Forms.Core.Models.Form form)
        {
            if (this._formDesignSettings.DisableAutomaticAdditionOfDataConsentField || form.AllFields.Any<Umbraco.Forms.Core.Models.Field>(x => x.Alias == "dataConsent"))
                return;
            form.Pages.Last<Page>().FieldSets.Last<FieldSet>().Containers.Last<FieldsetContainer>().AddDataConsentField(this._formDesignSettings, this._fieldCollection);
        }

        private string GetDefaultEmailTemplatePath()
        {
            string defaultEmailTemplate = this._formDesignSettings.DefaultEmailTemplate;
            if (!string.IsNullOrEmpty(defaultEmailTemplate) && this.EmailTemplateExists(defaultEmailTemplate))
                return defaultEmailTemplate;
            return !this.EmailTemplateExists("/Views/Partials/Forms/Emails", "Example-Template.cshtml") ? string.Empty : "Forms/Emails/Example-Template.cshtml";
        }

        private bool EmailTemplateExists(string relativePath)
        {
            string[] source = relativePath.Split('/');
            if (source.Length == 0)
                return false;
            int num = source.Length == 1 ? 1 : 0;
            return this.EmailTemplateExists("~/Views/Partials/" + (num != 0 ? string.Empty : string.Join("/", source.Take<string>(source.Length - 1))), num != 0 ? source[0] : source.LastOrDefault<string>() ?? string.Empty);
        }

        private bool EmailTemplateExists(string folder, string fileName) => this.CreateEmailFileSystem(this._hostEnvironment.MapPathContentRoot(folder), folder).FileExists(fileName);

        private PhysicalFileSystem CreateEmailFileSystem(
          string rootPath,
          string rootUrl)
        {
            return new PhysicalFileSystem(this._ioHelper, this._hostingEnvironment, this._loggerFactory.CreateLogger<PhysicalFileSystem>(), rootPath, rootUrl);
        }
    }
}
