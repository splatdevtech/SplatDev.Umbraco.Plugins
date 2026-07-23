
// Type: Umbraco.Forms.Web.Api.ManagementApi.Folder.DeleteFolderController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Folder
{
  public class DeleteFolderController : FolderControllerBase
  {
    public DeleteFolderController(IFolderService folderService)
      : base(folderService)
    {
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public IActionResult Delete(Guid id)
    {
      Umbraco.Forms.Core.Models.Folder folder = this.FolderService.Get(id);
      if (folder == null)
        return (IActionResult) this.NotFound();
      this.FolderService.Delete(folder);
      return (IActionResult) this.Ok();
    }
  }
}
