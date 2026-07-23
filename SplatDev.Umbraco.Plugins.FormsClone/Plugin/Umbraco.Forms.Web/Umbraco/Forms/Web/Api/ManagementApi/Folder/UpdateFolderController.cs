
// Type: Umbraco.Forms.Web.Api.ManagementApi.Folder.UpdateFolderController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.ManagementApi.Form;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Folder
{
  public class UpdateFolderController : FolderControllerBase
  {
    public UpdateFolderController(IFolderService folderService)
      : base(folderService)
    {
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof (ProblemDetails), 500)]
    public IActionResult Update(Guid id, UpdateFolderModel model)
    {
      if (!this.ModelState.IsValid)
        return (IActionResult) this.BadRequest((object) new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(this.ModelState)));
      Umbraco.Forms.Core.Models.Folder folder = this.FolderService.Get(id);
      if (folder == null)
        return (IActionResult) this.NotFound();
      if (this.GetChildFolders(folder.ParentId).Any<Umbraco.Forms.Core.Models.Folder>((Func<Umbraco.Forms.Core.Models.Folder, bool>) (x => x.Id != id && string.Equals(x.Name, model.Name, StringComparison.InvariantCultureIgnoreCase))))
      {
        this.ModelState.AddModelError("Name", "A folder already exists with the name '" + model.Name + "'.");
        return (IActionResult) this.BadRequest((object) new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(this.ModelState)));
      }
      folder.Name = model.Name;
      this.FolderService.Update(folder);
      return (IActionResult) this.Ok();
    }
  }
}
