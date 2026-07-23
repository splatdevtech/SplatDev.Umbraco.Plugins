
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.DeleteFormController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Security;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form
{
  public class DeleteFormController : FormControllerBase
  {
    private readonly IUserFormSecurityStorage _userFormSecurityStorage;

    public DeleteFormController(
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
      IUserFormSecurityStorage userFormSecurityStorage)
      : base(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, (ILogger) logger, htmlSanitizer)
    {
      this._userFormSecurityStorage = userFormSecurityStorage;
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public IActionResult Delete(Guid id)
    {
      Umbraco.Forms.Core.Models.Form form = this.FormService.Get(id);
      if (form == null)
        return (IActionResult) this.NotFound();
      if (!this.ValidateAccessToForm(form))
        return (IActionResult) this.Forbid();
      this.WorkflowService.Delete(form);
      this.FormService.Delete(form);
      this._userFormSecurityStorage.DeleteAllUserFormSecurityForForm(id);
      return (IActionResult) this.Ok();
    }
  }
}
