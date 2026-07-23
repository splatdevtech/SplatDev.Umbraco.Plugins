
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.HasRelationsFormController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form
{
  public class HasRelationsFormController : FormControllerBase
  {
    private readonly ITrackedReferencesService _trackedReferencesService;

    public HasRelationsFormController(
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

    [HttpGet("{id:guid}/has-relations")]
    [ProducesResponseType(typeof (bool), 200)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> HasRelations(Guid id)
    {
      HasRelationsFormController relationsFormController = this;
      Umbraco.Forms.Core.Models.Form form = relationsFormController.FormService.Get(id);
      if (form == null)
        return (IActionResult) relationsFormController.NotFound();
      if (!relationsFormController.ValidateAccessToForm(form))
        return (IActionResult) relationsFormController.Forbid();
      bool flag = (await relationsFormController._trackedReferencesService.GetPagedRelationsForItemAsync(form.Id, 0L, 1L, false)).Total > 0L;
      return (IActionResult) relationsFormController.Ok((object) flag);
    }
  }
}
