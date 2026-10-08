using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Security;

#if NET10_0_OR_GREATER
using Umbraco.Cms.Core.Security;
#endif

namespace SplatDev.Umbraco.Plugins.PdfCurator.Controllers.Member;

/// <summary>Public entry points and sign-in flow for the PdfCurator member surface.</summary>
[Route("member")]
public sealed class MemberSurfaceController : Controller
{
    private readonly MemberSignInManager _signInManager;
    private readonly IAntiforgery _antiforgery;

    public MemberSurfaceController(MemberSignInManager signInManager, IAntiforgery antiforgery)
    {
        _signInManager = signInManager;
        _antiforgery = antiforgery;
    }

    [HttpGet("")]
    public IActionResult Index() => Page("Member Library", "library");

    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = "/member") =>
        Content(LoginPage(SafeReturnUrl(returnUrl), null), "text/html");

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(MemberLoginInput input)
    {
        var returnUrl = SafeReturnUrl(input.ReturnUrl);
        if (!ModelState.IsValid)
            return Content(LoginPage(returnUrl, "Enter both your email and password."), "text/html");

        var result = await _signInManager.PasswordSignInAsync(
            input.Email, input.Password, input.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
            return LocalRedirect(returnUrl);

        return Content(LoginPage(returnUrl, "The email or password was not recognised."), "text/html");
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard() => Page("Member Dashboard", "dashboard");

    private IActionResult Page(string title, string route)
    {
        var authenticated = User.Identity?.IsAuthenticated == true;
        var memberName = System.Net.WebUtility.HtmlEncode(User.Identity?.Name ?? "member");
        var content = authenticated
            ? $"<p data-member-content=\"true\">Welcome, {memberName}. Your member library and reading progress are ready.</p><nav><a href=\"/member\">Library</a> <a href=\"/member/dashboard\">Dashboard</a></nav><div id=\"pdfc-app\" data-route=\"{route}\"></div><script type=\"module\" src=\"/App_Plugins/PdfCurator/dist-member/member.js\"></script>"
            : "<p>Sign in to access your member library and reading progress.</p><a href=\"/member/login?returnUrl=/member\">Log in</a>";

        return Content($"<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><title>{title}</title></head><body><main class=\"pdfc-member-area\"><h1>{title}</h1>{content}</main></body></html>", "text/html");
    }

    private string LoginPage(string returnUrl, string? error)
    {
        var errorHtml = string.IsNullOrWhiteSpace(error) ? "" : $"<p role=\"alert\">{System.Net.WebUtility.HtmlEncode(error)}</p>";
        var token = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken ?? string.Empty;
        var encodedReturn = System.Net.WebUtility.HtmlEncode(returnUrl);
        return $"<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><title>Member Login</title></head><body><main class=\"pdfc-member-area\"><h1>Member Login</h1>{errorHtml}<form method=\"post\" action=\"/member/login\"><input type=\"hidden\" name=\"__RequestVerificationToken\" value=\"{System.Net.WebUtility.HtmlEncode(token)}\" /><input type=\"hidden\" name=\"ReturnUrl\" value=\"{encodedReturn}\" /><label for=\"email\">Email</label><input id=\"email\" name=\"Email\" type=\"email\" autocomplete=\"username\" required /><label for=\"password\">Password</label><input id=\"password\" name=\"Password\" type=\"password\" autocomplete=\"current-password\" required /><label><input name=\"RememberMe\" type=\"checkbox\" value=\"true\" /> Remember me</label><button type=\"submit\">Sign in</button></form></main></body></html>";
    }

    private string SafeReturnUrl(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/member";
}

public sealed class MemberLoginInput
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.EmailAddress]
    public string Email { get; set; } = string.Empty;
    [System.ComponentModel.DataAnnotations.Required]
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
    public string ReturnUrl { get; set; } = "/member";
}
