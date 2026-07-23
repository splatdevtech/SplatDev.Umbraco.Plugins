
// Type: Umbraco.Forms.Web.Api.ManagementApi.PrevalueSourceType.GetAllPrevalueSourceTypesController
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
namespace Umbraco.Forms.Web.Api.ManagementApi.PrevalueSourceType
{
  public class GetAllPrevalueSourceTypesController : PrevalueSourceTypeControllerBase
  {
    public GetAllPrevalueSourceTypesController(
      FieldPreValueSourceCollection prevalueSourceTypeCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings)
      : base(prevalueSourceTypeCollection, hostingEnvironment, formDesignSettings)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof (IEnumerable<PreValueSourceTypeWithSettings>), 200)]
    public IActionResult GetAll()
    {
      List<PreValueSourceTypeWithSettings> typeWithSettingsList = new List<PreValueSourceTypeWithSettings>();
      foreach (FieldPreValueSourceType prevalueSourceType in (BuilderCollectionBase<FieldPreValueSourceType>) this.PrevalueSourceTypeCollection)
      {
        PreValueSourceTypeWithSettings typeWithSettings = this.CreatePrevalueSourceTypeWithSettings(prevalueSourceType);
        typeWithSettingsList.Add(typeWithSettings);
      }
      return (IActionResult) this.Ok((object) typeWithSettingsList);
    }
  }
}
