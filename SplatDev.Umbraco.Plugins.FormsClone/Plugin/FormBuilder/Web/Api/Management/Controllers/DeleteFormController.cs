using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Mapping;

using Umbraco.Cms.Core.Security;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for deleting a form.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class DeleteFormController(
      IFormService formService,
      IFolderService folderService,
      IWorkflowService workflowService,
      IFieldService fieldService,
      IFieldTypeStorage fieldTypeStorage,
      WorkflowCollection workflowCollection,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IFormsSecurity formsSecurity,
      IUmbracoMapper mapper,
      ILogger<DeleteFormController> logger,
      IHtmlSanitizer htmlSanitizer,
      IUserFormSecurityStorage userFormSecurityStorage) : FormControllerBase(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, logger, htmlSanitizer)
    {
        private readonly IUserFormSecurityStorage _userFormSecurityStorage = userFormSecurityStorage;

        /// <summary>Management API endpoint for deleting a form.</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult Delete(Guid id)
        {
            Form? form = FormService.Get(id);
            if (form is null)
                return NotFound();
            if (!ValidateAccessToForm(form))
                return Forbid();
            WorkflowService.Delete(form);
            FormService.Delete(form);
            _userFormSecurityStorage.DeleteAllUserFormSecurityForForm(id);
            return Ok();
        }
    }
}