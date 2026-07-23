
// Type: Umbraco.Forms.Web.Api.ManagementApi.FieldType.GetByKeyFieldTypeController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.FieldType
{
  public class GetByKeyFieldTypeController : GetFieldTypeControllerBase
  {
    public GetByKeyFieldTypeController(
      IFieldTypeStorage fieldTypeStorage,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings,
      FieldCollection fieldCollection)
      : base(fieldTypeStorage, hostingEnvironment, formDesignSettings, fieldCollection)
    {
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof (FieldTypeWithSettings), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetByKey(Guid id)
    {
      Umbraco.Forms.Core.FieldType fieldType = this.FieldCollection.SingleOrDefault<Umbraco.Forms.Core.FieldType>((Func<Umbraco.Forms.Core.FieldType, bool>) (x => x.Id == id));
      return fieldType == null ? (IActionResult) this.NotFound() : (IActionResult) this.Ok((object) this.CreateFieldTypeWithSettings(fieldType));
    }
  }
}
