
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.CopyFormController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.ManagementApi.Form;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form
{
    public class CopyFormController : FormControllerBase
    {
        public CopyFormController(
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

        [HttpPost("{id:guid}/copy")]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult Copy(Guid id, CopyFormModel model)
        {
            Umbraco.Forms.Core.Models.Form formToCopy = this.FormService.Get(id);
            if (formToCopy == null)
                return this.NotFound();
            if (!string.IsNullOrEmpty(model.CopyToFolderId))
                formToCopy.FolderId = !(model.CopyToFolderId == "-1") ? new Guid?(Guid.Parse(model.CopyToFolderId)) : null;
            if (!string.IsNullOrEmpty(model.CopyToFolderId))
                formToCopy.FolderId = !(model.CopyToFolderId == "-1") ? new Guid?(Guid.Parse(model.CopyToFolderId)) : null;
            string newFormName = string.IsNullOrEmpty(model.NewName) ? formToCopy.Name : model.NewName;
            Umbraco.Forms.Core.Models.Form[] array = this.FormService.Get().ToArray<Umbraco.Forms.Core.Models.Form>();
            if (array.Any<Umbraco.Forms.Core.Models.Form>(x =>
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
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape(newFormName) + "\\s*\\(\\s*(\\d+)\\s*\\)$");
                int num = array.Select<Umbraco.Forms.Core.Models.Form, Match>(x => regex.Match(x.Name)).Where<Match>(x => x.Success).Select<Match, int>(x => int.Parse(x.Groups[1].Value)).DefaultIfEmpty<int>(0).Max() + 1;
                newFormName = newFormName + " (" + num.ToString() + ")";
            }
            List<Umbraco.Forms.Core.Models.Workflow> workflowList = this.WorkflowService.Get(formToCopy);
            RegenerateFormStructureIdsResult structureIdsResult = formToCopy.RegenerateFormStructureIds();
            formToCopy.Name = newFormName;
            formToCopy.Id = Guid.NewGuid();
            Umbraco.Forms.Core.Models.Form form1 = this.FormService.Insert(formToCopy);
            if (form1 == null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 1);
                interpolatedStringHandler.AppendLiteral("Failed to create copied form from form with Id ");
                interpolatedStringHandler.AppendFormatted<Guid>(id);
                interpolatedStringHandler.AppendLiteral(".");
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            Umbraco.Forms.Core.Models.Form form2 = form1;
            foreach (Umbraco.Forms.Core.Models.Workflow workflow in workflowList)
            {
                if (model.CopyWorkflows || workflow.IsMandatory)
                {
                    workflow.Id = Guid.Empty;
                    workflow.Form = form2.Id;
                    this.UpdateFieldIdsInWorkflowSetting(workflow, structureIdsResult.FieldIdMapping);
                    this.WorkflowService.Insert(form2, workflow);
                }
            }
            return this.CreatedAtAction<GetByKeyFormController>(controller => "GetByKey", form2.Id);
        }
    }
}
