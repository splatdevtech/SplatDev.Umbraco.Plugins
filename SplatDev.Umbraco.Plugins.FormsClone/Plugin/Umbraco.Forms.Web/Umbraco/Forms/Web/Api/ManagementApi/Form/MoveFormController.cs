
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.MoveFormController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.ManagementApi.Form;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form
{
  public class MoveFormController : FormControllerBase
  {
    public MoveFormController(
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
      : base(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, (ILogger) logger, htmlSanitizer)
    {
    }

    [HttpPut("{id:guid}/move")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult Move(Guid id, MoveFormModel model)
    {
      if (!this.ModelState.IsValid)
        return (IActionResult) this.BadRequest((object) new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(this.ModelState)));
      Umbraco.Forms.Core.Models.Form form = this.FormService.Get(id);
      if (form == null)
        return (IActionResult) this.NotFound();
      if (this.GetChildForms(model.ParentId).Any<Umbraco.Forms.Core.Models.Form>((Func<Umbraco.Forms.Core.Models.Form, bool>) (x => x.Id != id && string.Equals(x.Name, form.Name, StringComparison.InvariantCultureIgnoreCase))))
      {
        this.ModelState.AddModelError(string.Empty, "A folder already exists with the name '" + form.Name + "' at the location selected.");
        return (IActionResult) this.BadRequest((object) new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(this.ModelState)));
      }
      Guid? folderId = form.FolderId;
      form.FolderId = model.ParentId;
      if (this.FormService is FormService formService)
      {
        Dictionary<string, object> additionalData = new Dictionary<string, object>()
        {
          {
            "MovedFromFolderId",
            (object) folderId
          }
        };
        formService.Update(form, additionalData);
      }
      else
        this.FormService.Update(form);
      return (IActionResult) this.Ok();
    }

    private IEnumerable<Umbraco.Forms.Core.Models.Form> GetChildForms(Guid? parentId) => parentId.HasValue ? this.FormService.GetFromFolder(parentId.Value) : this.FormService.GetAtRoot();
  }
}
