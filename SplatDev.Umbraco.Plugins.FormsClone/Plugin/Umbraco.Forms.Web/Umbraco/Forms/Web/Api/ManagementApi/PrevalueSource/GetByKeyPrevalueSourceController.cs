
// Type: Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource.GetByKeyPrevalueSourceController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource
{
  [Authorize(Policy = "ManagePrevalueSources")]
  public class GetByKeyPrevalueSourceController : PrevalueSourceControllerBase
  {
    public GetByKeyPrevalueSourceController(
      IPrevalueSourceService prevalueSourceService,
      FieldPreValueSourceCollection fieldPreValueSources,
      IFieldPreValueSourceTypeService fieldPreValueSourceTypeService)
      : base(fieldPreValueSources, prevalueSourceService, fieldPreValueSourceTypeService)
    {
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof (FieldPreValueSource), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetByKey(Guid id)
    {
      FieldPreValueSource fieldPreValueSource = this.PrevalueSourceService.Get(id);
      return fieldPreValueSource == null ? (IActionResult) this.NotFound() : (IActionResult) this.Ok((object) fieldPreValueSource);
    }
  }
}
