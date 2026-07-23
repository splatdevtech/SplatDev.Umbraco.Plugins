using FormBuilder.Core.Attributes;
using FormBuilder.Core.Configuration;
using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Workflows;
using FormBuilder.Web.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with forms.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Form")]
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "ManageForms")]
    [Route("/formBuilder/management/api/v1/form")]
    public abstract class FormControllerBase(
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
      IHtmlSanitizer htmlSanitizer) : FormsManagementApiControllerBase
    {
        private readonly IFieldService _fieldService = fieldService;
        private readonly IHtmlSanitizer _htmlSanitizer = htmlSanitizer;
        internal const string FolderPrefix = "folder-";

        /// <summary>
        /// Gets the         /// </summary>
        protected IFormService FormService { get; } = formService;

        /// <summary>
        /// Gets the         /// </summary>
        protected IFolderService FolderService { get; } = folderService;

        /// <summary>
        /// Gets the         /// </summary>
        protected IWorkflowService WorkflowService { get; } = workflowService;

        /// <summary>
        /// Gets the         /// </summary>
        protected IFieldTypeStorage FieldTypeStorage { get; } = fieldTypeStorage;

        /// <summary>
        /// Gets the         /// </summary>
        protected WorkflowCollection WorkflowCollection { get; } = workflowCollection;

        /// <summary>
        /// Gets the         /// </summary>
        protected IFormsSecurity FormsSecurity { get; } = formsSecurity;

        /// <summary>
        /// Gets the         /// </summary>
        protected IBackOfficeSecurityAccessor BackOfficeSecurityAccessor { get; } = backOfficeSecurityAccessor;

        /// <summary>
        /// Gets the         /// </summary>
        protected IUmbracoMapper Mapper { get; } = mapper;

        /// <summary>
        /// Gets the         /// </summary>
        protected ILogger Logger { get; } = logger;

        /// <summary>
        /// Checks to see if a form is valid based on data provided and the model state.
        /// </summary>
        /// <returns></returns>
        protected bool IsFormValid(FormDesign formData, FormDesignSettings formDesignSettings)
        {
            _fieldService.GetDuplicates(formData.AllFields).ToList().ForEach(p => ModelState.AddModelError("field_" + p, "The form contains a field with a duplicate alias: " + p));
            if (formDesignSettings.MandatoryFieldsetLegends && formData.AllFieldSets.Any(x => string.IsNullOrEmpty(x.Caption)))
                ModelState.AddModelError(string.Empty, "One or more groups do not have a completed caption. These are required to provide an accessible form.");
            return ModelState.IsValid;
        }

        private void SetPath(FormDesign form)
        {
            if (form.FolderId.HasValue)
            {
                string folderPath = GetFolderPath(form.FolderId.Value);
                FormDesign formDesign = form;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(1, 2);
                interpolatedStringHandler.AppendFormatted(folderPath);
                interpolatedStringHandler.AppendLiteral(",");
                interpolatedStringHandler.AppendFormatted(form.Id);
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                formDesign.Path = stringAndClear;
            }
            else
            {
                FormDesign formDesign = form;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(1, 2);
                interpolatedStringHandler.AppendFormatted("-1");
                interpolatedStringHandler.AppendLiteral(",");
                interpolatedStringHandler.AppendFormatted(form.Id);
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                formDesign.Path = stringAndClear;
            }
        }

        private string GetFolderPath(Guid folderId)
        {
            string path = FolderService.GetPath(folderId, "folder-");
            IList<Guid> foldersForCurrentUser = GetStartFoldersForCurrentUser();
            return foldersForCurrentUser.Count == 0 ? path : path.ModifyFolderPathForStartFolders(foldersForCurrentUser, "folder-");
        }

        private IList<Guid> GetStartFoldersForCurrentUser() => [.. FormsSecurity.GetStartFolderKeysForCurrentUser()];

        /// <summary>Gets the workflows for a form by execution stage.</summary>
        protected FormWorkflows GetWorkflowsForForm(Form form)
        {
            List<Workflow> workflows = WorkflowService.Get(form);
            List<FormWorkflowWithTypeSettings> workflowsForForm1 = GetWorkflowsForForm(workflows, FormState.Submitted);
            List<FormWorkflowWithTypeSettings> workflowsForForm2 = GetWorkflowsForForm(workflows, FormState.Approved);
            List<FormWorkflowWithTypeSettings> workflowsForForm3 = GetWorkflowsForForm(workflows, FormState.Rejected);
            return new FormWorkflows()
            {
                OnSubmit = workflowsForForm1,
                OnApprove = workflowsForForm2,
                OnReject = workflowsForForm3
            };
        }

        private List<FormWorkflowWithTypeSettings> GetWorkflowsForForm(
          List<Workflow> workflows,
          FormState formState)
        {
            List<FormWorkflowWithTypeSettings> workflowsForForm = [];
            foreach (Workflow workflow1 in (IEnumerable<Workflow>)workflows.Where(x => x.ExecutesOn == formState).OrderBy(x => x.SortOrder))
            {
                WorkflowType workflow2 = WorkflowCollection[workflow1.WorkflowTypeId];
                FormWorkflowWithTypeSettings withTypeSettings = new()
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

        /// <summary>
        /// Replaces references to field IDs in workflow settings with the equivalents in the provided mapping.
        /// </summary>
        protected static void UpdateFieldIdsInWorkflowSetting(
          Workflow workflow,
          Dictionary<Guid, Guid> fieldIdMapping)
        {
            Dictionary<string, string> settings = new(workflow.Settings, StringComparer.OrdinalIgnoreCase);
            bool flag = false;
            foreach (KeyValuePair<string, string> setting in workflow.Settings)
            {
                if (UpdateFieldIdInWorkflowSetting(settings, setting.Key, settings[setting.Key], fieldIdMapping))
                    flag = true;
            }
            if (!flag)
                return;
            workflow.Settings = settings;
        }

        private static bool UpdateFieldIdInWorkflowSetting(
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

        /// <summary>
        /// Creates the         /// </summary>
        protected void CreateFormAndWorkflowsForPersistence(
          FormDesign formData,
          out Form form,
          out List<Workflow> workflows)
        {
            if (!formData.FolderId.HasValue)
            {
                IList<Guid> foldersForCurrentUser = GetStartFoldersForCurrentUser();
                if (foldersForCurrentUser.Count == 1)
                    formData.FolderId = new Guid?(foldersForCurrentUser.First());
            }
            Form? form1 = Mapper.Map<Form>(formData);
            if (form1 is null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(54, 2);
                interpolatedStringHandler.AppendLiteral("Could not map an instance of ");
                interpolatedStringHandler.AppendFormatted("Form");
                interpolatedStringHandler.AppendLiteral(" from form data with Id ");
                interpolatedStringHandler.AppendFormatted(formData.Id);
                interpolatedStringHandler.AppendLiteral(".");
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            form = form1;
            workflows = Mapper.Map<FormDesign, List<Workflow>>(formData) ?? [];
            EnsurePreValuesAreValidForFields(form);
            SanitizeHtmlForFormAndWorkflows(form, workflows);
        }

        private void EnsurePreValuesAreValidForFields(Form form)
        {
            foreach (Field field in form.AllFields.Where(x => x.PreValues is not null && x.PreValues.Any()).ToList())
            {
                FieldType? fieldTypeByField = FieldTypeStorage.GetFieldTypeByField(field);
                if (fieldTypeByField is null || !fieldTypeByField.SupportsPreValues)
                {
                    field.PreValues = [];
                    field.PreValueSourceId = Guid.Empty;
                }
            }
        }

        private void SanitizeHtmlForFormAndWorkflows(Form form, List<Workflow> workflows)
        {
            if (form.MessageOnSubmitIsHtml && !string.IsNullOrWhiteSpace(form.MessageOnSubmit))
                form.MessageOnSubmit = _htmlSanitizer.Sanitize(form.MessageOnSubmit);
            foreach (Field allField in form.AllFields)
            {
                FieldType? fieldTypeByField = FieldTypeStorage.GetFieldTypeByField(allField);
                if (fieldTypeByField is not null)
                    SanitizeHtmlSettings(allField.Settings, fieldTypeByField.Settings());
            }
            foreach (Workflow workflow1 in workflows)
            {
                WorkflowType workflow2 = WorkflowCollection[workflow1.WorkflowTypeId];
                if (workflow2 is not null)
                    SanitizeHtmlSettings(workflow1.Settings, workflow2.Settings());
            }

            void SanitizeHtmlSettings(
              IDictionary<string, string> settings,
              IDictionary<string, SettingAttribute> settingAttributes)
            {
                foreach (string key in (IEnumerable<string>)settings.Keys)
                {
                    if (settingAttributes.TryGetValue(key, out SettingAttribute? settingAttribute) && settingAttribute.HtmlEncodeReplacedPlaceholderValues)
                        settings[key] = _htmlSanitizer.Sanitize(settings[key]);
                }
            }
        }

        /// <summary>Validates access to a form for the current user.</summary>
        protected bool ValidateAccessToForm(Form form)
        {
            if (!FormsSecurity.HasAccessToForm(form.Id))
                return false;
            IList<Guid> foldersForCurrentUser = GetStartFoldersForCurrentUser();
            if (foldersForCurrentUser.Count == 0)
                return true;
            if (!form.FolderId.HasValue)
                return false;
            List<string> formFolderPath = [.. FolderService.GetPath(form.FolderId.Value).Split(',')];
            return foldersForCurrentUser.Any(x => formFolderPath.Contains(x.ToString()));
        }

        protected static BasicForm CreateBasicForm(Form form)
        {
            new BasicForm().Id = form.Id;
            new BasicForm().Name = form.Name;
            new BasicForm().Fields = form.GetFormFieldSummary();
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(23, 2);
            interpolatedStringHandler.AppendFormatted(form.Pages.Count);
            interpolatedStringHandler.AppendLiteral(" page form with ");
            interpolatedStringHandler.AppendFormatted(form.AllFields.Count);
            interpolatedStringHandler.AppendLiteral(" fields");
            new BasicForm().Summary = interpolatedStringHandler.ToStringAndClear();
            return new BasicForm();
        }

        protected void TryUpdateFormAndWorkflows(Form form, List<Workflow> workflows)
        {
            try
            {
                try
                {
                    WorkflowService.Delete(form);
                }
                finally
                {
                    workflows.Where(q => !q.Deleted).ToList().ForEach(p => WorkflowService.Insert(form, p));
                    FormService.Update(form);
                }
            }
            catch (OperationCanceledException ex)
            {
                Logger.LogDebug(ex, "Form save operation was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception thrown on update of form so falling back to inserting.");
                FormService.Insert(form);
                WorkflowService.Insert(form, workflows);
            }
        }
    }
}