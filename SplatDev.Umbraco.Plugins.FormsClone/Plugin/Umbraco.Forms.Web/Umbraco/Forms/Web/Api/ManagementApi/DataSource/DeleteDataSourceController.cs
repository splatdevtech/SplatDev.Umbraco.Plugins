
// Type: Umbraco.Forms.Web.Api.ManagementApi.DataSource.DeleteDataSourceController
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
  public class DeleteDataSourceController : DataSourceControllerBase
  {
    public DeleteDataSourceController(
      IDataSourceService dataSourceService,
      DataSourceTypeCollection dataSourceTypeCollection)
      : base(dataSourceService, dataSourceTypeCollection)
    {
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public IActionResult Delete(Guid id)
    {
      FormDataSource formDataSource = this.DataSourceService.Get(id);
      if (formDataSource == null)
        return (IActionResult) this.NotFound();
      this.DataSourceService.Delete(formDataSource);
      return (IActionResult) this.Ok();
    }
  }
}
