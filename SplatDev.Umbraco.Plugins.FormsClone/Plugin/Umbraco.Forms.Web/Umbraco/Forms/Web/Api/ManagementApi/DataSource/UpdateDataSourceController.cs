
// Type: Umbraco.Forms.Web.Api.ManagementApi.DataSource.UpdateDataSourceController
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
  public class UpdateDataSourceController : DataSourceControllerBase
  {
    public UpdateDataSourceController(
      IDataSourceService dataSourceService,
      DataSourceTypeCollection dataSourceTypeCollection)
      : base(dataSourceService, dataSourceTypeCollection)
    {
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof (ProblemDetails), 400)]
    [ProducesResponseType(typeof (ProblemDetails), 403)]
    [ProducesResponseType(typeof (ProblemDetails), 500)]
    public IActionResult Update(Guid id, FormDataSource dataSource)
    {
      ProblemDetails problemDetails;
      switch (this.TryValidateProviderType(dataSource, out problemDetails))
      {
        case ProviderValidationResult.FailedTypeNotFound:
          return (IActionResult) this.NotFound();
        case ProviderValidationResult.FailedValidation:
          return (IActionResult) this.BadRequest((object) problemDetails);
        default:
          this.DataSourceService.Update(dataSource);
          return (IActionResult) this.Ok();
      }
    }
  }
}
