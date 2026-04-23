
// Type: Umbraco.Forms.Web.Api.ManagementApi.DataSource.GetByKeyDataSourceController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.DataSource
{
  public class GetByKeyDataSourceController : DataSourceControllerBase
  {
    public GetByKeyDataSourceController(
      IDataSourceService dataSourceService,
      DataSourceTypeCollection dataSourceTypeCollection)
      : base(dataSourceService, dataSourceTypeCollection)
    {
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof (FormDataSource), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetByKey(Guid id)
    {
      FormDataSource datasource = this.DataSourceService.Get(id);
      if (datasource == null)
        return (IActionResult) this.NotFound();
      FormDataSourceType formDataSourceType = datasource.GetFormDataSourceType(this.DataSourcesTypeCollection);
      if (formDataSourceType == null)
        return (IActionResult) this.NotFound();
      formDataSourceType.LoadSettings(datasource);
      datasource.Valid = !formDataSourceType.ValidateSettings().Any<Exception>();
      return (IActionResult) this.Ok((object) datasource);
    }
  }
}
