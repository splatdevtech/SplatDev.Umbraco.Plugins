
// Type: Umbraco.Forms.Web.Api.ManagementApi.DataSource.GetScaffoldDataSourceController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.DataSource
{
  public class GetScaffoldDataSourceController : DataSourceControllerBase
  {
    public GetScaffoldDataSourceController(
      IDataSourceService dataSourceService,
      DataSourceTypeCollection dataSourceTypeCollectione)
      : base(dataSourceService, dataSourceTypeCollectione)
    {
    }

    [HttpGet("scaffold")]
    [ProducesResponseType(typeof (FormDataSource), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetScaffold() => (IActionResult) this.Ok((object) new FormDataSource()
    {
      Id = Guid.NewGuid()
    });
  }
}
