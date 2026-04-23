using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Mapping;

using Umbraco.Cms.Core.Security;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for copying the workflows of a form.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class CopyFormWorkflowsController(
      IFormService formService,
      IFolderService folderService,
      IWorkflowService workflowService,
      IFieldService fieldService,
      IFieldTypeStorage fieldTypeStorage,
      WorkflowCollection workflowCollection,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IFormsSecurity formsSecurity,
      IUmbracoMapper mapper,
      ILogger<MoveFormController> logger,
      IHtmlSanitizer htmlSanitizer) : FormControllerBase(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, logger, htmlSanitizer)
    {
        /// <summary>
        /// Management API endpoint for copying the workflows of a form.
        /// </summary>
        [HttpPost("{id:guid}/copy-workflows")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult CopyWorkflows(Guid id, CopyFormWorkflowsModel model)
        {
            Form? form1 = FormService.Get(id);
            if (form1 is null)
                return NotFound();
            Form? form2 = FormService.Get(model.DestinationId);
            if (form2 is null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(50, 1);
                interpolatedStringHandler.AppendLiteral("Could not find form to copy workflows to with Id ");
                interpolatedStringHandler.AppendFormatted(model.DestinationId);
                interpolatedStringHandler.AppendLiteral(".");
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            Form form3 = form2;
            FormWorkflows workflowsForForm1 = GetWorkflowsForForm(form1);
            FormWorkflows workflowsForForm2 = GetWorkflowsForForm(form3);
            Dictionary<FormState, int> dictionary = new()
              {
                {
                  FormState.Submitted,
                  workflowsForForm2.OnSubmit.Count
                },
                {
                  FormState.Approved,
                  workflowsForForm2.OnApprove.Count
                },
                {
                  FormState.Rejected,
                  workflowsForForm2.OnReject.Count
                }
              };
            foreach (Guid workflowId in model.WorkflowIds)
            {
                (FormWorkflowWithTypeSettings withTypeSettings, FormState key) = GetWorkflowFromFormWorkflows(workflowsForForm1, workflowId);
                Workflow workflow = new()
                {
                    Active = withTypeSettings.Active,
                    Condition = withTypeSettings.Condition,
                    ExecutesOn = key,
                    Form = form3.Id,
                    Id = Guid.Empty,
                    IncludeSensitiveData = withTypeSettings.IncludeSensitiveData,
                    IsMandatory = withTypeSettings.IsMandatory,
                    Name = withTypeSettings.Name,
                    Settings = withTypeSettings.Settings.ToDictionary(x => x.Key, x => x.Value),
                    SortOrder = dictionary[key] + 1,
                    WorkflowTypeId = withTypeSettings.WorkflowTypeId
                };
                Dictionary<Guid, Guid> formFieldMapping = GetCopiedFormFieldMapping(form1, form3);
                UpdateFieldIdsInWorkflowSetting(workflow, formFieldMapping);
                dictionary[key]++;
                WorkflowService.Insert(form3, workflow);
                Logger.LogInformation("Workflow with Id {WorkflowId} copied from form {SourceFormId} to {DestinationFormId}", withTypeSettings.Id, form1.Id, form3.Id);
            }
            return Ok();
        }

        private static (FormWorkflowWithTypeSettings, FormState) GetWorkflowFromFormWorkflows(
          FormWorkflows workflowsForForm,
          Guid workflowId)
        {
            FormWorkflowWithTypeSettings? withTypeSettings1 = workflowsForForm.OnSubmit.SingleOrDefault(x => x.Id == workflowId);
            if (withTypeSettings1 is not null)
                return (withTypeSettings1, FormState.Submitted);
            FormWorkflowWithTypeSettings? withTypeSettings2 = workflowsForForm.OnApprove.SingleOrDefault(x => x.Id == workflowId);
            if (withTypeSettings2 is not null)
                return (withTypeSettings2, FormState.Approved);
            FormWorkflowWithTypeSettings? withTypeSettings3 = workflowsForForm.OnReject.SingleOrDefault(x => x.Id == workflowId);
            if (withTypeSettings3 is not null)
                return (withTypeSettings3, FormState.Rejected);

            DefaultInterpolatedStringHandler interpolatedStringHandler = new(33, 1);
            interpolatedStringHandler.AppendLiteral("Could not find workflow with Id ");
            interpolatedStringHandler.AppendFormatted(workflowId);
            interpolatedStringHandler.AppendLiteral(".");
            throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
        }

        private static Dictionary<Guid, Guid> GetCopiedFormFieldMapping(
          Form sourceForm,
          Form destinationForm)
        {
            Dictionary<Guid, Guid> formFieldMapping = [];
            foreach (Field allField in sourceForm.AllFields)
            {
                Field sourceField = allField;
                var field = destinationForm.AllFields.SingleOrDefault(x => x.Alias == sourceField.Alias);
                formFieldMapping.Add(sourceField.Id, field is not null ? field.Id : Guid.Empty);
            }
            return formFieldMapping;
        }
    }
}