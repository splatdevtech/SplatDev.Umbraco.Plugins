
// Type: Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource.DeletePrevalueSourceController
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
  public class DeletePrevalueSourceController : PrevalueSourceControllerBase
  {
    public DeletePrevalueSourceController(
      IPrevalueSourceService prevalueSourceService,
      FieldPreValueSourceCollection fieldPreValueSources,
      IFieldPreValueSourceTypeService fieldPreValueSourceTypeService)
      : base(fieldPreValueSources, prevalueSourceService, fieldPreValueSourceTypeService)
    {
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public IActionResult DeleteByGuid(Guid id)
    {
      FieldPreValueSource fieldPreValueSource = this.PrevalueSourceService.Get(id);
      if (fieldPreValueSource == null)
        return (IActionResult) this.NotFound();
      this.PrevalueSourceService.Delete(fieldPreValueSource);
      return (IActionResult) this.Ok();
    }
  }
}
