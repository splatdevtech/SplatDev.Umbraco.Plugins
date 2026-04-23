
// Type: Umbraco.Forms.Web.Api.ManagementApi.DataSourceType.GetByKeyDataSourceTypeController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.DataSourceType
{
  public class GetByKeyDataSourceTypeController : DataSourceTypeControllerBase
  {
    public GetByKeyDataSourceTypeController(
      DataSourceTypeCollection dataSourceTypeCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings)
      : base(dataSourceTypeCollection, hostingEnvironment, formDesignSettings)
    {
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof (DataSourceTypeWithSettings), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetByKey(Guid id)
    {
      FormDataSourceType type = this.DataSourceTypeCollection.SingleOrDefault<FormDataSourceType>((Func<FormDataSourceType, bool>) (x => x.Id == id));
      return type == null ? (IActionResult) this.NotFound() : (IActionResult) this.Ok((object) this.CreateDataSourceTypeWithSettings(type));
    }
  }
}
