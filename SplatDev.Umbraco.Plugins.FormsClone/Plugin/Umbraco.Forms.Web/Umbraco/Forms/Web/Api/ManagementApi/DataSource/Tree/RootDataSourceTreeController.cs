
// Type: Umbraco.Forms.Web.Api.ManagementApi.DataSource.Tree.RootDataSourceTreeController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.ManagementApi.DataSource;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.DataSource.Tree
{
  public class RootDataSourceTreeController : DataSourceTreeControllerBase
  {
    private readonly IDataSourceService _dataourceService;

    public RootDataSourceTreeController(IDataSourceService dataourceService) => this._dataourceService = dataourceService;

    [HttpGet("root")]
    [ProducesResponseType(typeof (PagedViewModel<DataSourceTreeItemResponseModel>), 200)]
    public ActionResult<IEnumerable<DataSourceTreeItemResponseModel>> Root()
    {
      List<DataSourceTreeItemResponseModel> source = new List<DataSourceTreeItemResponseModel>();
      foreach (FormDataSourceSlim dataSource in (IEnumerable<FormDataSourceSlim>) this._dataourceService.GetSlim().OrderBy<FormDataSourceSlim, string>((Func<FormDataSourceSlim, string>) (x => x.Name)))
        source.Add(this.CreateItemResponse(dataSource));
      return (ActionResult<IEnumerable<DataSourceTreeItemResponseModel>>) (ActionResult) this.Ok((object) new PagedViewModel<DataSourceTreeItemResponseModel>()
      {
        Items = (IEnumerable<DataSourceTreeItemResponseModel>) source,
        Total = (long) source.Count<DataSourceTreeItemResponseModel>()
      });
    }

    private DataSourceTreeItemResponseModel CreateItemResponse(
      FormDataSourceSlim dataSource)
    {
      DataSourceTreeItemResponseModel itemResponse = new DataSourceTreeItemResponseModel();
      itemResponse.Id = dataSource.Id;
      itemResponse.HasChildren = false;
      itemResponse.Name = dataSource.Name;
      itemResponse.IsFolder = false;
      return itemResponse;
    }
  }
}
