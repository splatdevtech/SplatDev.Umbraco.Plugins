
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.GetRelationsFormController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form
{
  public class GetRelationsFormController : FormControllerBase
  {
    private readonly ITrackedReferencesService _trackedReferencesService;

    public GetRelationsFormController(
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
      ITrackedReferencesService trackedReferencesService)
      : base(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, (ILogger) logger, htmlSanitizer)
    {
      this._trackedReferencesService = trackedReferencesService;
    }

    [HttpGet("{id:guid}/relations")]
    [ProducesResponseType(typeof (PagedModel<RelationItemModel>), 200)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetRelations(Guid id)
    {
      GetRelationsFormController relationsFormController = this;
      Umbraco.Forms.Core.Models.Form form = relationsFormController.FormService.Get(id);
      if (form == null)
        return (IActionResult) relationsFormController.NotFound();
      if (!relationsFormController.ValidateAccessToForm(form))
        return (IActionResult) relationsFormController.Forbid();
      PagedModel<RelationItemModel> relationsForItemAsync = await relationsFormController._trackedReferencesService.GetPagedRelationsForItemAsync(id, 0L, (long) int.MaxValue, false);
      return (IActionResult) relationsFormController.Ok((object) relationsForItemAsync);
    }
  }
}
