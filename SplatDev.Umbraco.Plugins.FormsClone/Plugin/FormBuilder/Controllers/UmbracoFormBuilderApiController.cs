using Asp.Versioning;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "FormBuilder")]
    public class UmbracoFormBuilderApiController : UmbracoFormBuilderApiControllerBase
    {

        [HttpGet("ping")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        public static string Ping() => "Pong";
    }
}
