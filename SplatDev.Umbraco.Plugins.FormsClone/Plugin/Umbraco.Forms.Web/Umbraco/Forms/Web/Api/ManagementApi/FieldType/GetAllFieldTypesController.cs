
// Type: Umbraco.Forms.Web.Api.ManagementApi.FieldType.GetAllFieldTypesController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.FieldType
{
  public class GetAllFieldTypesController : GetFieldTypeControllerBase
  {
    public GetAllFieldTypesController(
      IFieldTypeStorage fieldTypeStorage,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings,
      FieldCollection fieldCollection)
      : base(fieldTypeStorage, hostingEnvironment, formDesignSettings, fieldCollection)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof (IEnumerable<FieldTypeWithSettings>), 200)]
    public IActionResult GetAll()
    {
      List<FieldTypeWithSettings> source = new List<FieldTypeWithSettings>();
      foreach (Umbraco.Forms.Core.FieldType field in (BuilderCollectionBase<Umbraco.Forms.Core.FieldType>) this.FieldCollection)
      {
        FieldTypeWithSettings typeWithSettings = this.CreateFieldTypeWithSettings(field);
        source.Add(typeWithSettings);
      }
      return (IActionResult) this.Ok((object) source.OrderBy<FieldTypeWithSettings, string>((Func<FieldTypeWithSettings, string>) (x => x.Name)));
    }
  }
}
