using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for moving a form.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class MoveFormController(
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
        /// <summary>Management API endpoint for moving a form.</summary>
        [HttpPut("{id:guid}/move")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Move(Guid id, MoveFormModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new SimpleValidationModel(ModelState.ToErrorDictionary()));
            Form? form = FormService.Get(id);
            if (form is null)
                return NotFound();
            if (GetChildForms(model.ParentId).Any(x => x.Id != id && string.Equals(x.Name, form.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                ModelState.AddModelError(string.Empty, "A folder already exists with the name '" + form.Name + "' at the location selected.");
                return BadRequest(new SimpleValidationModel(ModelState.ToErrorDictionary()));
            }
            Guid? folderId = form.FolderId;
            form.FolderId = model.ParentId;
            if (FormService is FormService formService)
            {
                Dictionary<string, object?>? additionalData = new()
                {
                  {
                    "MovedFromFolderId",
                     folderId
                  }
                };
                formService.Update(form, additionalData);
            }
            else
                FormService.Update(form);
            return Ok();
        }

        private IEnumerable<Form> GetChildForms(Guid? parentId) => parentId.HasValue ? FormService.GetFromFolder(parentId.Value) : FormService.GetAtRoot();
    }
}