
// Type: Umbraco.Forms.Web.Api.ManagementApi.Folder.GetByKeyFolderController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Folder
{
  public class GetByKeyFolderController : FolderControllerBase
  {
    public GetByKeyFolderController(IFolderService folderService)
      : base(folderService)
    {
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof (Umbraco.Forms.Core.Models.Folder), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetByKey(Guid id)
    {
      Umbraco.Forms.Core.Models.Folder folder = this.FolderService.Get(id);
      return folder == null ? (IActionResult) this.NotFound() : (IActionResult) this.Ok((object) folder);
    }
  }
}
