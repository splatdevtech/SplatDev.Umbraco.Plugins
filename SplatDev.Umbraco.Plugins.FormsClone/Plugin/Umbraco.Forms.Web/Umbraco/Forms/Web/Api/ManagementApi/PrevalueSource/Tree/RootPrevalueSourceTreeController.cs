
// Type: Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource.Tree.RootPrevalueSourceTreeController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Api.Management.ViewModels.Tree;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.ManagementApi.PrevalueSource;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource.Tree
{
  public class RootPrevalueSourceTreeController : PrevalueSourceTreeControllerBase
  {
    private readonly IPrevalueSourceService _prevalueSourceService;

    public RootPrevalueSourceTreeController(IPrevalueSourceService prevalueSourceService) => this._prevalueSourceService = prevalueSourceService;

    [HttpGet("root")]
    [ProducesResponseType(typeof (PagedViewModel<PrevalueSourceTreeItemResponseModel>), 200)]
    public ActionResult<IEnumerable<FolderTreeItemResponseModel>> Root()
    {
      List<PrevalueSourceTreeItemResponseModel> source = new List<PrevalueSourceTreeItemResponseModel>();
      foreach (FieldPreValueSourceSlim prevalueSource in (IEnumerable<FieldPreValueSourceSlim>) this._prevalueSourceService.GetSlim().OrderBy<FieldPreValueSourceSlim, string>((Func<FieldPreValueSourceSlim, string>) (x => x.Name)))
        source.Add(this.CreateItemResponse(prevalueSource));
      return (ActionResult<IEnumerable<FolderTreeItemResponseModel>>) (ActionResult) this.Ok((object) new PagedViewModel<PrevalueSourceTreeItemResponseModel>()
      {
        Items = (IEnumerable<PrevalueSourceTreeItemResponseModel>) source,
        Total = (long) source.Count<PrevalueSourceTreeItemResponseModel>()
      });
    }

    private PrevalueSourceTreeItemResponseModel CreateItemResponse(
      FieldPreValueSourceSlim prevalueSource)
    {
      PrevalueSourceTreeItemResponseModel itemResponse = new PrevalueSourceTreeItemResponseModel();
      itemResponse.Id = prevalueSource.Id;
      itemResponse.HasChildren = false;
      itemResponse.Name = prevalueSource.Name;
      itemResponse.IsFolder = false;
      return itemResponse;
    }
  }
}
