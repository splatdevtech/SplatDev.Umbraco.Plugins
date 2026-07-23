
// Type: Umbraco.Forms.Web.Api.ManagementApi.Folder.MoveFolderController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.ManagementApi.Form;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Folder
{
  public class MoveFolderController : FolderControllerBase
  {
    public MoveFolderController(IFolderService folderService)
      : base(folderService)
    {
    }

    [HttpPut("{id:guid}/move")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult Move(Guid id, MoveFolderModel model)
    {
      if (!this.ModelState.IsValid)
        return (IActionResult) this.BadRequest((object) new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(this.ModelState)));
      Umbraco.Forms.Core.Models.Folder folder = this.FolderService.Get(id);
      if (folder == null)
        return (IActionResult) this.NotFound();
      if (this.GetChildFolders(model.ParentId).Any<Umbraco.Forms.Core.Models.Folder>((Func<Umbraco.Forms.Core.Models.Folder, bool>) (x => x.Id != id && string.Equals(x.Name, folder.Name, StringComparison.InvariantCultureIgnoreCase))))
      {
        this.ModelState.AddModelError(string.Empty, "A folder already exists with the name '" + folder.Name + "' at the location selected.");
        return (IActionResult) this.BadRequest((object) new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(this.ModelState)));
      }
      if (model.ParentId.HasValue && Array.IndexOf<string>(this.FolderService.GetPath(model.ParentId.Value).Split(','), folder.Id.ToString()) > -1)
      {
        this.ModelState.AddModelError(string.Empty, "The destination folder selected is not valid.  You can't move a folder to a location below one of it's descendents.");
        return (IActionResult) this.BadRequest((object) new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(this.ModelState)));
      }
      Guid? parentId = folder.ParentId;
      folder.ParentId = model.ParentId;
      if (this.FolderService is FolderService folderService)
      {
        Dictionary<string, object> additionalData = new Dictionary<string, object>()
        {
          {
            "MovedFromFolderId",
            (object) parentId
          }
        };
        folderService.Update(folder, additionalData);
      }
      else
        this.FolderService.Update(folder);
      return (IActionResult) this.Ok();
    }
  }
}
