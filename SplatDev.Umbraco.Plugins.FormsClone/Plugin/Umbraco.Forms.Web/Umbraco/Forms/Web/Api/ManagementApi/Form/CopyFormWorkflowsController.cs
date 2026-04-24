
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.CopyFormWorkflowsController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Security;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;
using Umbraco.Forms.Web.Models.ManagementApi.Form;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form
{
    public class CopyFormWorkflowsController : FormControllerBase
    {
        public CopyFormWorkflowsController(
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
          IHtmlSanitizer htmlSanitizer)
          : base(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, logger, htmlSanitizer)
        {
        }

        [HttpPost("{id:guid}/copy-workflows")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult CopyWorkflows(Guid id, CopyFormWorkflowsModel model)
        {
            Umbraco.Forms.Core.Models.Form form1 = this.FormService.Get(id);
            if (form1 == null)
                return this.NotFound();
            Umbraco.Forms.Core.Models.Form form2 = this.FormService.Get(model.DestinationId);
            if (form2 == null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 1);
                interpolatedStringHandler.AppendLiteral("Could not find form to copy workflows to with Id ");
                interpolatedStringHandler.AppendFormatted<Guid>(model.DestinationId);
                interpolatedStringHandler.AppendLiteral(".");
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            Umbraco.Forms.Core.Models.Form form3 = form2;
            FormWorkflows workflowsForForm1 = this.GetWorkflowsForForm(form1);
            FormWorkflows workflowsForForm2 = this.GetWorkflowsForForm(form3);
            Dictionary<FormState, int> dictionary = new Dictionary<FormState, int>()
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
                (FormWorkflowWithTypeSettings withTypeSettings, FormState key) = CopyFormWorkflowsController.GetWorkflowFromFormWorkflows(workflowsForForm1, workflowId);
                Umbraco.Forms.Core.Models.Workflow workflow = new Umbraco.Forms.Core.Models.Workflow()
                {
                    Active = withTypeSettings.Active,
                    Condition = withTypeSettings.Condition,
                    ExecutesOn = key,
                    Form = form3.Id,
                    Id = Guid.Empty,
                    IncludeSensitiveData = withTypeSettings.IncludeSensitiveData,
                    IsMandatory = withTypeSettings.IsMandatory,
                    Name = withTypeSettings.Name,
                    Settings = withTypeSettings.Settings.ToDictionary<KeyValuePair<string, string>, string, string>(x => x.Key, x => x.Value),
                    SortOrder = dictionary[key] + 1,
                    WorkflowTypeId = withTypeSettings.WorkflowTypeId
                };
                Dictionary<Guid, Guid> formFieldMapping = CopyFormWorkflowsController.GetCopiedFormFieldMapping(form1, form3);
                this.UpdateFieldIdsInWorkflowSetting(workflow, formFieldMapping);
                dictionary[key]++;
                this.WorkflowService.Insert(form3, workflow);
                this.Logger.LogInformation("Workflow with Id {WorkflowId} copied from form {SourceFormId} to {DestinationFormId}", withTypeSettings.Id, form1.Id, form3.Id);
            }
            return this.Ok();
        }

        private static (FormWorkflowWithTypeSettings, FormState) GetWorkflowFromFormWorkflows(
          FormWorkflows workflowsForForm,
          Guid workflowId)
        {
            FormWorkflowWithTypeSettings withTypeSettings1 = workflowsForForm.OnSubmit.SingleOrDefault<FormWorkflowWithTypeSettings>(x => x.Id == workflowId);
            if (withTypeSettings1 != null)
                return (withTypeSettings1, FormState.Submitted);
            FormWorkflowWithTypeSettings withTypeSettings2 = workflowsForForm.OnApprove.SingleOrDefault<FormWorkflowWithTypeSettings>(x => x.Id == workflowId);
            if (withTypeSettings2 != null)
                return (withTypeSettings2, FormState.Approved);
            FormWorkflowWithTypeSettings withTypeSettings3 = workflowsForForm.OnReject.SingleOrDefault<FormWorkflowWithTypeSettings>(x => x.Id == workflowId);
            if (withTypeSettings3 != null)
                return (withTypeSettings3, FormState.Rejected);
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
            interpolatedStringHandler.AppendLiteral("Could not find workflow with Id ");
            interpolatedStringHandler.AppendFormatted<Guid>(workflowId);
            interpolatedStringHandler.AppendLiteral(".");
            throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
        }

        private static Dictionary<Guid, Guid> GetCopiedFormFieldMapping(
          Umbraco.Forms.Core.Models.Form sourceForm,
          Umbraco.Forms.Core.Models.Form destinationForm)
        {
            Dictionary<Guid, Guid> formFieldMapping = new Dictionary<Guid, Guid>();
            foreach (Umbraco.Forms.Core.Models.Field allField in sourceForm.AllFields)
            {
                Umbraco.Forms.Core.Models.Field sourceField = allField;
                Umbraco.Forms.Core.Models.Field field = destinationForm.AllFields.SingleOrDefault<Umbraco.Forms.Core.Models.Field>(x => x.Alias == sourceField.Alias);
                // ISSUE: explicit non-virtual call
                formFieldMapping.Add(sourceField.Id, field != null ? field.Id : Guid.Empty);
            }
            return formFieldMapping;
        }
    }
}
