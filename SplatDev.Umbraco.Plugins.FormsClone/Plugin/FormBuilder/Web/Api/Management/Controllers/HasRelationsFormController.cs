using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Mapping;

using Umbraco.Cms.Core.Security;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving whether a form has relations.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class HasRelationsFormController(
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
      ITrackedReferencesService trackedReferencesService) : FormControllerBase(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, logger, htmlSanitizer)
    {
        private readonly ITrackedReferencesService _trackedReferencesService = trackedReferencesService;

        /// <summary>
        /// Management API endpoint for retrieving whether a form has relations.
        /// </summary>
        [HttpGet("{id:guid}/has-relations")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> HasRelations(Guid id)
        {
            HasRelationsFormController relationsFormController = this;
            Form? form = relationsFormController.FormService.Get(id);
            if (form is null)
                return relationsFormController.NotFound();
            if (!relationsFormController.ValidateAccessToForm(form))
                return relationsFormController.Forbid();
            bool flag = (await relationsFormController._trackedReferencesService.GetPagedRelationsForItemAsync(form.Id, 0L, 1L, false)).Total > 0L;
            return relationsFormController.Ok(flag);
        }
    }
}