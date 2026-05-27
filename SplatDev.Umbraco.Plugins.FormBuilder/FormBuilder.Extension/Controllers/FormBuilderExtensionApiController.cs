using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Extension.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "FormBuilder.Extension")]
    public class FormBuilderExtensionApiController : FormBuilderExtensionApiControllerBase
    {

        [HttpGet("ping")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        public string Ping() => "Pong";
    }
}
