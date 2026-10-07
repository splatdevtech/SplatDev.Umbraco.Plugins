using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

#if NET10_0_OR_GREATER
using Umbraco.Cms.Core.Security;
#endif

namespace SplatDev.Umbraco.Plugins.PdfCurator.Controllers.Member;

/// <summary>Stable JSON profile endpoint for the member surface.</summary>
[ApiController]
[Route("api/member/profile")]
public sealed class MemberProfileController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
#if NET10_0_OR_GREATER
        var member = await HttpContext.RequestServices
            .GetRequiredService<IMemberManager>()
            .GetCurrentMemberAsync();
        return Ok(new
        {
            authenticated = member is not null,
            name = member?.Name,
            email = member?.Email,
        });
#else
        return Ok(new { authenticated = false, name = (string?)null, email = (string?)null });
#endif
    }
}
