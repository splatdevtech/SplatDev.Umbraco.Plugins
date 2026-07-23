using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using Umbraco.Cms.Core.Mapping;

using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for copying a form.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class CopyFormController(
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
        /// <summary>Management API endpoint for copying a form.</summary>
        [HttpPost("{id:guid}/copy")]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult Copy(Guid id, CopyFormModel model)
        {
            Form? formToCopy = FormService.Get(id);
            if (formToCopy is null)
                return NotFound();
            if (!string.IsNullOrEmpty(model.CopyToFolderId))
                formToCopy.FolderId = !(model.CopyToFolderId == "-1") ? new Guid?(Guid.Parse(model.CopyToFolderId)) : new Guid?();
            if (!string.IsNullOrEmpty(model.CopyToFolderId))
                formToCopy.FolderId = !(model.CopyToFolderId == "-1") ? new Guid?(Guid.Parse(model.CopyToFolderId)) : new Guid?();
            string newFormName = string.IsNullOrEmpty(model.NewName) ? formToCopy.Name : model.NewName;
            Form[] array = [.. FormService.Get()];
            if (array.Any(x =>
            {
                if (!x.Name.InvariantEquals(newFormName))
                    return false;
                Guid? folderId1 = x.FolderId;
                Guid? folderId2 = formToCopy.FolderId;
                if (folderId1.HasValue != folderId2.HasValue)
                    return false;
                return !folderId1.HasValue || folderId1.GetValueOrDefault() == folderId2.GetValueOrDefault();
            }))
            {
                Regex formName = new(Regex.Escape(newFormName) + "\\s*\\(\\s*(\\d+)\\s*\\)$");
                Regex regex = formName;
                int num = array.Select(x => regex.Match(x.Name)).Where(x => x.Success).Select(x => int.Parse(x.Groups[1].Value)).DefaultIfEmpty(0).Max() + 1;
                newFormName = newFormName + " (" + num.ToString() + ")";
            }
            List<Workflow> workflowList = WorkflowService.Get(formToCopy);
            RegenerateFormStructureIdsResult structureIdsResult = formToCopy.RegenerateFormStructureIds();
            formToCopy.Name = newFormName;
            formToCopy.Id = Guid.NewGuid();
            var form1 = FormService.Insert(formToCopy);
            if (form1 is null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(48, 1);
                interpolatedStringHandler.AppendLiteral("Failed to create copied form from form with Id ");
                interpolatedStringHandler.AppendFormatted(id);
                interpolatedStringHandler.AppendLiteral(".");
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            Form form2 = form1;
            foreach (Workflow workflow in workflowList)
            {
                if (model.CopyWorkflows || workflow.IsMandatory)
                {
                    workflow.Id = Guid.Empty;
                    workflow.Form = form2.Id;
                    UpdateFieldIdsInWorkflowSetting(workflow, structureIdsResult.FieldIdMapping);
                    WorkflowService.Insert(form2, workflow);
                }
            }
            return CreatedAtAction<GetByKeyFormController>(controller => "GetByKey", form2.Id);
        }
    }
}