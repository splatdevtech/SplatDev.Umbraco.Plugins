
// Type: Umbraco.Forms.Web.Api.ManagementApi.DataSourceType.GetAllDataSourceTypesController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.DataSourceType
{
  public class GetAllDataSourceTypesController : DataSourceTypeControllerBase
  {
    public GetAllDataSourceTypesController(
      DataSourceTypeCollection dataSourceTypeCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings)
      : base(dataSourceTypeCollection, hostingEnvironment, formDesignSettings)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof (IEnumerable<DataSourceTypeWithSettings>), 200)]
    public IActionResult GetAll()
    {
      List<DataSourceTypeWithSettings> typeWithSettingsList = new List<DataSourceTypeWithSettings>();
      foreach (FormDataSourceType dataSourceType in (BuilderCollectionBase<FormDataSourceType>) this.DataSourceTypeCollection)
      {
        DataSourceTypeWithSettings typeWithSettings = this.CreateDataSourceTypeWithSettings(dataSourceType);
        typeWithSettingsList.Add(typeWithSettings);
      }
      return (IActionResult) this.Ok((object) typeWithSettingsList);
    }
  }
}
