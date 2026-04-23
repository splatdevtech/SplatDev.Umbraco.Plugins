
// Type: Umbraco.Forms.Web.Api.ManagementApi.PrevalueSourceType.GetByKeyPrevalueSourceTypeController
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
namespace Umbraco.Forms.Web.Api.ManagementApi.PrevalueSourceType
{
  public class GetByKeyPrevalueSourceTypeController : PrevalueSourceTypeControllerBase
  {
    public GetByKeyPrevalueSourceTypeController(
      FieldPreValueSourceCollection prevalueSourceTypeCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings)
      : base(prevalueSourceTypeCollection, hostingEnvironment, formDesignSettings)
    {
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof (PreValueSourceTypeWithSettings), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetByKey(Guid id)
    {
      FieldPreValueSourceType type = this.PrevalueSourceTypeCollection.SingleOrDefault<FieldPreValueSourceType>((Func<FieldPreValueSourceType, bool>) (x => x.Id == id));
      return type == null ? (IActionResult) this.NotFound() : (IActionResult) this.Ok((object) this.CreatePrevalueSourceTypeWithSettings(type));
    }
  }
}
