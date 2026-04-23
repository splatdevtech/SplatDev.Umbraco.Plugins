using Asp.Versioning;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "FormBuilder.Extension")]
    public class FormBuilderExtensionApiController : FormBuilderExtensionApiControllerBase
    {
        [HttpGet("ping")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        public static string Ping() => "Pong";
    }
}