
// Type: Umbraco.Forms.Web.Api.ManagementApi.DataSource.GetCollectionDataSourceController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.DataSource
{
  public class GetCollectionDataSourceController : DataSourceControllerBase
  {
    public GetCollectionDataSourceController(
      IDataSourceService dataSourceService,
      DataSourceTypeCollection dataSourceTypeCollectione)
      : base(dataSourceService, dataSourceTypeCollectione)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof (PagedViewModel<FormDataSource>), 200)]
    public IActionResult GetCollection(int skip = 0, int take = 2147483647)
    {
      List<FormDataSource> list = this.DataSourceService.Get().ToList<FormDataSource>();
      return (IActionResult) this.Ok((object) new PagedViewModel<FormDataSource>()
      {
        Total = (long) list.Count,
        Items = list.Skip<FormDataSource>(skip).Take<FormDataSource>(take)
      });
    }
  }
}
