using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

#if NET10_0_OR_GREATER
using Umbraco.Cms.Core.Security;
#endif

namespace SplatDev.Umbraco.Plugins.PdfCurator.Controllers.Member;

/// <summary>Public entry points for the PdfCurator member surface.</summary>
[Route("member")]
public sealed class MemberSurfaceController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => Page("Member Library", "library");

    [HttpGet("login")]
    public IActionResult Login() => Content(LoginPage(), "text/html");

    [HttpGet("dashboard")]
    public IActionResult Dashboard() => Page("Member Dashboard", "dashboard");

    private IActionResult Page(string title, string route)
    {
        var memberName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : null;
        var memberContent = memberName is null
            ? "<p>Sign in to access your member library and reading progress.</p><a href=\"/member/login?returnUrl=/member\">Log in</a>"
            : $"<p data-member-content=\"true\">Welcome, {System.Net.WebUtility.HtmlEncode(memberName)}. Your member library is ready.</p><nav><a href=\"/member\">Library</a> <a href=\"/member/dashboard\">Dashboard</a></nav>";

        return Content($"<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><title>{title}</title></head><body><main class=\"pdfc-member-area\"><h1>{title}</h1>{memberContent}</main></body></html>", "text/html");
    }

    private static string LoginPage() => "<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><title>Member Login</title></head><body><main class=\"pdfc-member-area\"><h1>Member Login</h1><p>Member authentication is provided by the host site.</p><a href=\"/login?returnUrl=/member\">Continue to login</a></main></body></html>";
}
