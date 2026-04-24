using FormBuilder.Web.Helpers;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    public class GetVersionController : UpdatesControllerBase
    {
        /// <summary>Gets the currrent version.</summary>
        [HttpGet("version")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetVersion() => Ok(VersionHelper.GetInstalledPackageVersion());
    }
}