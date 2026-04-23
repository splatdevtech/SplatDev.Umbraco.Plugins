using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Mapping;

using Umbraco.Cms.Core.Models;

using Umbraco.Cms.Core.Security;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving relations for a single form by Id.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetRelationsFormController(
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
        /// Management API endpoint for retrieving a single form by Id.
        /// </summary>
        [HttpGet("{id:guid}/relations")]
        [ProducesResponseType(typeof(PagedModel<RelationItemModel>), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRelations(Guid id)
        {
            GetRelationsFormController relationsFormController = this;
            Form? form = relationsFormController.FormService.Get(id);
            if (form is null)
                return relationsFormController.NotFound();
            if (!relationsFormController.ValidateAccessToForm(form))
                return relationsFormController.Forbid();
            PagedModel<RelationItemModel> relationsForItemAsync = await relationsFormController._trackedReferencesService.GetPagedRelationsForItemAsync(id, 0L, int.MaxValue, false);
            return relationsFormController.Ok(relationsForItemAsync);
        }
    }
}