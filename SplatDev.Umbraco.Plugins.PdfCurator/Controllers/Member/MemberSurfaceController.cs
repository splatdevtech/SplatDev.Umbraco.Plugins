using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

#if NET10_0_OR_GREATER
using Umbraco.Cms.Core.Security;
#endif

namespace SplatDev.Umbraco.Plugins.PdfCurator.Controllers.Member;

/// <summary>
/// Public entry points for the PdfCurator member surface. These routes are intentionally
/// ordinary MVC routes rather than Umbraco content routes so the package is usable on a
/// host that has not provisioned seed content nodes.
/// </summary>
[Route("member")]
public sealed class MemberSurfaceController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View("Member", BuildModel("library"));

    [HttpGet("login")]
    public IActionResult Login() => View("Login", new MemberSurfaceModel("login", null, false));

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
#if NET10_0_OR_GREATER
        return View("Member", BuildAuthenticatedModel("dashboard"));
#else
        return View("Member", BuildModel("dashboard"));
#endif
    }

#if NET10_0_OR_GREATER
    private MemberSurfaceModel BuildAuthenticatedModel(string route)
    {
        var member = HttpContext.RequestServices.GetRequiredService<IMemberManager>()
            .GetCurrentMemberAsync().GetAwaiter().GetResult();
        return new MemberSurfaceModel(route, member?.Name ?? member?.Email, member is not null);
    }
#endif

    private MemberSurfaceModel BuildModel(string route) =>
        new(route, User.Identity?.Name, User.Identity?.IsAuthenticated == true);
}

public sealed record MemberSurfaceModel(string Route, string? MemberName, bool IsAuthenticated);
