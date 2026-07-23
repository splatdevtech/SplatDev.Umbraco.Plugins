
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.FormControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Extensions;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form
{
    [ApiExplorerSettings(GroupName = "Form")]
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "ManageForms")]
    [Route("/umbraco/forms/management/api/v1/form")]
    public abstract class FormControllerBase : FormsManagementApiControllerBase
    {
        private readonly IFieldService _fieldService;
        private readonly IHtmlSanitizer _htmlSanitizer;
        internal const string FolderPrefix = "folder-";

        protected FormControllerBase(
          IFormService formService,
          IFolderService folderService,
          IWorkflowService workflowService,
          IFieldService fieldService,
          IFieldTypeStorage fieldTypeStorage,
          WorkflowCollection workflowCollection,
          IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
          IFormsSecurity formsSecurity,
          IUmbracoMapper mapper,
          ILogger logger,
          IHtmlSanitizer htmlSanitizer)
        {
            this.FormService = formService;
            this.FolderService = folderService;
            this.WorkflowService = workflowService;
            this._fieldService = fieldService;
            this.FieldTypeStorage = fieldTypeStorage;
            this.WorkflowCollection = workflowCollection;
            this.BackOfficeSecurityAccessor = backOfficeSecurityAccessor;
            this.FormsSecurity = formsSecurity;
            this.Mapper = mapper;
            this.Logger = logger;
            this._htmlSanitizer = htmlSanitizer;
        }

        protected IFormService FormService { get; }

        protected IFolderService FolderService { get; }

        protected IWorkflowService WorkflowService { get; }

        protected IFieldTypeStorage FieldTypeStorage { get; }

        protected WorkflowCollection WorkflowCollection { get; }

        protected IFormsSecurity FormsSecurity { get; }

        protected IBackOfficeSecurityAccessor BackOfficeSecurityAccessor { get; }

        protected IUmbracoMapper Mapper { get; }

        protected ILogger Logger { get; }

        protected bool IsFormValid(FormDesign formData, FormDesignSettings formDesignSettings)
        {
            this._fieldService.GetDuplicates(formData.AllFields).ToList<string>().ForEach(p => this.ModelState.AddModelError("field_" + p, "The form contains a field with a duplicate alias: " + p));
            if (formDesignSettings.MandatoryFieldsetLegends && formData.AllFieldSets.Any<FieldSet>(x => string.IsNullOrEmpty(x.Caption)))
                this.ModelState.AddModelError(string.Empty, "One or more groups do not have a completed caption. These are required to provide an accessible form.");
            return this.ModelState.IsValid;
        }

        private void SetPath(FormDesign form)
        {
            if (form.FolderId.HasValue)
            {
                string folderPath = this.GetFolderPath(form.FolderId.Value);
                FormDesign formDesign = form;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
                interpolatedStringHandler.AppendFormatted(folderPath);
                interpolatedStringHandler.AppendLiteral(",");
                interpolatedStringHandler.AppendFormatted<Guid>(form.Id);
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                formDesign.Path = stringAndClear;
            }
            else
            {
                FormDesign formDesign = form;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
                interpolatedStringHandler.AppendFormatted("-1");
                interpolatedStringHandler.AppendLiteral(",");
                interpolatedStringHandler.AppendFormatted<Guid>(form.Id);
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                formDesign.Path = stringAndClear;
            }
        }

        private string GetFolderPath(Guid folderId)
        {
            string path = this.FolderService.GetPath(folderId, "folder-");
            IList<Guid> foldersForCurrentUser = this.GetStartFoldersForCurrentUser();
            return foldersForCurrentUser.Count == 0 ? path : path.ModifyFolderPathForStartFolders(foldersForCurrentUser, "folder-");
        }

        private IList<Guid> GetStartFoldersForCurrentUser() => this.FormsSecurity.GetStartFolderKeysForCurrentUser().ToList<Guid>();

        protected FormWorkflows GetWorkflowsForForm(Umbraco.Forms.Core.Models.Form form)
        {
            List<Umbraco.Forms.Core.Models.Workflow> workflows = this.WorkflowService.Get(form);
            List<FormWorkflowWithTypeSettings> workflowsForForm1 = this.GetWorkflowsForForm(workflows, FormState.Submitted);
            List<FormWorkflowWithTypeSettings> workflowsForForm2 = this.GetWorkflowsForForm(workflows, FormState.Approved);
            List<FormWorkflowWithTypeSettings> workflowsForForm3 = this.GetWorkflowsForForm(workflows, FormState.Rejected);
            return new FormWorkflows()
            {
                OnSubmit = workflowsForForm1,
                OnApprove = workflowsForForm2,
                OnReject = workflowsForForm3
            };
        }

        private List<FormWorkflowWithTypeSettings> GetWorkflowsForForm(
          List<Umbraco.Forms.Core.Models.Workflow> workflows,
          FormState formState)
        {
            List<FormWorkflowWithTypeSettings> workflowsForForm = new List<FormWorkflowWithTypeSettings>();
            foreach (Umbraco.Forms.Core.Models.Workflow workflow1 in (IEnumerable<Umbraco.Forms.Core.Models.Workflow>)workflows.Where<Umbraco.Forms.Core.Models.Workflow>(x => x.ExecutesOn == formState).OrderBy<Umbraco.Forms.Core.Models.Workflow, int>(x => x.SortOrder))
            {
                Umbraco.Forms.Core.WorkflowType workflow2 = this.WorkflowCollection[workflow1.WorkflowTypeId];
                FormWorkflowWithTypeSettings withTypeSettings = new FormWorkflowWithTypeSettings()
                {
                    Name = workflow1.Name,
                    Id = workflow1.Id,
                    SortOrder = workflow1.SortOrder,
                    Active = workflow1.Active,
                    IncludeSensitiveData = workflow1.IncludeSensitiveData,
                    IsDeleted = false,
                    Form = workflow1.Form,
                    WorkflowTypeId = workflow1.WorkflowTypeId,
                    WorkflowTypeName = workflow2.Name,
                    WorkflowTypeDescription = workflow2.Description,
                    WorkflowTypeGroup = workflow2.Group,
                    WorkflowTypeIcon = workflow2.Icon,
                    IsMandatory = workflow1.IsMandatory,
                    Condition = workflow1.Condition,
                    Settings = workflow1.Settings
                };
                workflowsForForm.Add(withTypeSettings);
            }
            return workflowsForForm;
        }

        protected void UpdateFieldIdsInWorkflowSetting(
          Umbraco.Forms.Core.Models.Workflow workflow,
          Dictionary<Guid, Guid> fieldIdMapping)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>(workflow.Settings, StringComparer.OrdinalIgnoreCase);
            bool flag = false;
            foreach (KeyValuePair<string, string> setting in workflow.Settings)
            {
                if (this.UpdateFieldIdInWorkflowSetting(settings, setting.Key, settings[setting.Key], fieldIdMapping))
                    flag = true;
            }
            if (!flag)
                return;
            workflow.Settings = settings;
        }

        private bool UpdateFieldIdInWorkflowSetting(
          Dictionary<string, string> settings,
          string settingKey,
          string settingValue,
          Dictionary<Guid, Guid> fieldIdMapping)
        {
            bool flag = false;
            foreach (KeyValuePair<Guid, Guid> keyValuePair in fieldIdMapping)
            {
                if (!string.IsNullOrWhiteSpace(settingValue))
                {
                    string compare = settingValue;
                    Guid key = keyValuePair.Key;
                    string compareTo = key.ToString();
                    if (compare.InvariantContains(compareTo))
                    {
                        string str = settingValue;
                        key = keyValuePair.Key;
                        string oldValue = key.ToString();
                        key = keyValuePair.Value;
                        string newValue = key.ToString();
                        settingValue = str.Replace(oldValue, newValue);
                        settings[settingKey] = settingValue;
                        flag = true;
                    }
                }
            }
            return flag;
        }

        protected void CreateFormAndWorkflowsForPersistence(
          FormDesign formData,
          out Umbraco.Forms.Core.Models.Form form,
          out List<Umbraco.Forms.Core.Models.Workflow> workflows)
        {
            if (!formData.FolderId.HasValue)
            {
                IList<Guid> foldersForCurrentUser = this.GetStartFoldersForCurrentUser();
                if (foldersForCurrentUser.Count == 1)
                    formData.FolderId = new Guid?(foldersForCurrentUser.First<Guid>());
            }
            Umbraco.Forms.Core.Models.Form form1 = this.Mapper.Map<Umbraco.Forms.Core.Models.Form>(formData);
            if (form1 == null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 2);
                interpolatedStringHandler.AppendLiteral("Could not map an instance of ");
                interpolatedStringHandler.AppendFormatted("Form");
                interpolatedStringHandler.AppendLiteral(" from form data with Id ");
                interpolatedStringHandler.AppendFormatted<Guid>(formData.Id);
                interpolatedStringHandler.AppendLiteral(".");
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            workflows = this.Mapper.Map<FormDesign, List<Umbraco.Forms.Core.Models.Workflow>>(formData) ?? new List<Umbraco.Forms.Core.Models.Workflow>();
            this.EnsurePreValuesAreValidForFields(form1);
            this.SanitizeHtmlForFormAndWorkflows(form1, workflows);
            form = form1;
        }

        private void EnsurePreValuesAreValidForFields(Umbraco.Forms.Core.Models.Form form)
        {
            foreach (Umbraco.Forms.Core.Models.Field field in form.AllFields.Where<Umbraco.Forms.Core.Models.Field>(x => x.PreValues != null && x.PreValues.Any<FieldPrevalue>()).ToList<Umbraco.Forms.Core.Models.Field>())
            {
                Umbraco.Forms.Core.FieldType fieldTypeByField = this.FieldTypeStorage.GetFieldTypeByField(field);
                if (fieldTypeByField == null || !fieldTypeByField.SupportsPreValues)
                {
                    field.PreValues = new List<FieldPrevalue>();
                    field.PreValueSourceId = Guid.Empty;
                }
            }
        }

        private void SanitizeHtmlForFormAndWorkflows(Umbraco.Forms.Core.Models.Form form, List<Umbraco.Forms.Core.Models.Workflow> workflows)
        {
            if (form.MessageOnSubmitIsHtml && !string.IsNullOrWhiteSpace(form.MessageOnSubmit))
                form.MessageOnSubmit = this._htmlSanitizer.Sanitize(form.MessageOnSubmit);
            foreach (Umbraco.Forms.Core.Models.Field allField in form.AllFields)
            {
                Umbraco.Forms.Core.FieldType fieldTypeByField = this.FieldTypeStorage.GetFieldTypeByField(allField);
                if (fieldTypeByField != null)
                    SanitizeHtmlSettings(allField.Settings, fieldTypeByField.Settings());
            }
            foreach (Umbraco.Forms.Core.Models.Workflow workflow1 in workflows)
            {
                Umbraco.Forms.Core.WorkflowType workflow2 = this.WorkflowCollection[workflow1.WorkflowTypeId];
                if (workflow2 != null)
                    SanitizeHtmlSettings(workflow1.Settings, workflow2.Settings());
            }

            void SanitizeHtmlSettings(
              IDictionary<string, string> settings,
              IDictionary<string, SettingAttribute> settingAttributes)
            {
                foreach (string key in (IEnumerable<string>)settings.Keys)
                {
                    SettingAttribute settingAttribute;
                    if (settingAttributes.TryGetValue(key, out settingAttribute) && settingAttribute.HtmlEncodeReplacedPlaceholderValues)
                        settings[key] = this._htmlSanitizer.Sanitize(settings[key]);
                }
            }
        }

        protected bool ValidateAccessToForm(Umbraco.Forms.Core.Models.Form form)
        {
            if (!this.FormsSecurity.HasAccessToForm(form.Id))
                return false;
            IList<Guid> foldersForCurrentUser = this.GetStartFoldersForCurrentUser();
            if (foldersForCurrentUser.Count == 0)
                return true;
            if (!form.FolderId.HasValue)
                return false;
            List<string> formFolderPath = this.FolderService.GetPath(form.FolderId.Value).Split(',').ToList<string>();
            return foldersForCurrentUser.Any<Guid>(x => formFolderPath.Contains(x.ToString()));
        }

        protected BasicForm CreateBasicForm(Umbraco.Forms.Core.Models.Form form)
        {
            BasicForm basicForm = new BasicForm();
            basicForm.Id = form.Id;
            basicForm.Name = form.Name;
            basicForm.Fields = form.GetFormFieldSummary();
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 2);
            interpolatedStringHandler.AppendFormatted<int>(form.Pages.Count);
            interpolatedStringHandler.AppendLiteral(" page form with ");
            interpolatedStringHandler.AppendFormatted<int>(form.AllFields.Count);
            interpolatedStringHandler.AppendLiteral(" fields");
            basicForm.Summary = interpolatedStringHandler.ToStringAndClear();
            return basicForm;
        }

        protected void TryUpdateFormAndWorkflows(Umbraco.Forms.Core.Models.Form form, List<Umbraco.Forms.Core.Models.Workflow> workflows)
        {
            try
            {
                try
                {
                    this.WorkflowService.Delete(form);
                }
                finally
                {
                    workflows.Where<Umbraco.Forms.Core.Models.Workflow>(q => !q.Deleted).ToList<Umbraco.Forms.Core.Models.Workflow>().ForEach(p => this.WorkflowService.Insert(form, p));
                    this.FormService.Update(form);
                }
            }
            catch (OperationCanceledException ex)
            {
                this.Logger.LogDebug(ex, "Form save operation was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Exception thrown on update of form so falling back to inserting.");
                this.FormService.Insert(form);
                this.WorkflowService.Insert(form, workflows);
            }
        }
    }
}
