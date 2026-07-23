
// Type: Umbraco.Forms.Web.Api.ManagementApi.Folder.IsFolderEmptyController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Folder
{
  public class IsFolderEmptyController : FolderControllerBase
  {
    public IsFolderEmptyController(IFolderService folderService)
      : base(folderService)
    {
    }

    [HttpGet("{id:guid}/is-empty")]
    [ProducesResponseType(typeof (bool), 200)]
    public IActionResult IeEmpty(Guid id) => (IActionResult) this.Ok((object) this.FolderService.ExistsAndIsEmpty(id));
  }
}
