
// Type: Umbraco.Forms.Web.Api.ManagementApi.Updates.GetVersionController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Umbraco.Forms.Web.Helpers;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Updates
{
  public class GetVersionController : UpdatesControllerBase
  {
    [HttpGet("version")]
    [ProducesResponseType(typeof (string), 200)]
    public IActionResult GetVersion() => (IActionResult) this.Ok((object) VersionHelper.GetInstalledPackageVersion());
  }
}
