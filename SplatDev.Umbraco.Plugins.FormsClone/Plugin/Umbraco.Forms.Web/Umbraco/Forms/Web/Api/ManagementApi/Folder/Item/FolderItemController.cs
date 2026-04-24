
// Type: Umbraco.Forms.Web.Api.ManagementApi.Folder.Item.FolderItemController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.ManagementApi;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Folder.Item
{
  [ApiExplorerSettings(GroupName = "Folder")]
  [Authorize(Policy = "BackOfficeAccess")]
  [Route("/umbraco/forms/management/api/v1/item/folder")]
  public class FolderItemController : FormsManagementApiControllerBase
  {
    private readonly IFolderService _folderService;

    public FolderItemController(IFolderService folderService) => this._folderService = folderService;

    [HttpGet]
    [ProducesResponseType(typeof (IEnumerable<FolderItemResponseModel>), 200)]
    public IActionResult Item([FromQuery(Name = "id")] HashSet<Guid> ids)
    {
      List<Umbraco.Forms.Core.Models.Folder> source = new List<Umbraco.Forms.Core.Models.Folder>();
      foreach (Guid id in ids)
      {
        Umbraco.Forms.Core.Models.Folder folder = this._folderService.Get(id);
        if (folder != null)
          source.Add(folder);
      }
      return (IActionResult) this.Ok((object) source.Select<Umbraco.Forms.Core.Models.Folder, FolderItemResponseModel>((Func<Umbraco.Forms.Core.Models.Folder, FolderItemResponseModel>) (x =>
      {
        return new FolderItemResponseModel()
        {
          Id = x.Id,
          Name = x.Name
        };
      })).ToList<FolderItemResponseModel>());
    }
  }
}
