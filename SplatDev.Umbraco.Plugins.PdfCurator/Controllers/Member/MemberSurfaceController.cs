using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Security;

#if NET10_0_OR_GREATER
using Umbraco.Cms.Core.Security;
#endif

namespace SplatDev.Umbraco.Plugins.PdfCurator.Controllers.Member;

/// <summary>Public entry points and host-independent sign-in flow for PdfCurator members.</summary>
[Route("member")]
public sealed class MemberSurfaceController : Controller
{
    private readonly MemberSignInManager _signInManager;
#if NET10_0_OR_GREATER
    private readonly IMemberManager _memberManager;
#endif

    public MemberSurfaceController(
        MemberSignInManager signInManager
#if NET10_0_OR_GREATER
        , IMemberManager memberManager
#endif
    )
    {
        _signInManager = signInManager;
#if NET10_0_OR_GREATER
        _memberManager = memberManager;
#endif
    }

    [HttpGet("")]
    public IActionResult Index() => View("Member", BuildModel("library"));

    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = "/member") =>
        View("Login", new MemberLoginModel(returnUrl ?? "/member", null));

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(MemberLoginInput input)
    {
        var returnUrl = SafeReturnUrl(input.ReturnUrl);
        if (!ModelState.IsValid)
        {
            return View("Login", new MemberLoginModel(returnUrl, "Enter both your email and password."));
        }

        var result = await _signInManager.PasswordSignInAsync(
            input.Email,
            input.Password,
            input.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }

        return View("Login", new MemberLoginModel(returnUrl, "The email or password was not recognised."));
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard() => View("Member", BuildModel("dashboard"));

    private MemberSurfaceModel BuildModel(string route)
    {
#if NET10_0_OR_GREATER
        var member = _memberManager.GetCurrentMemberAsync().GetAwaiter().GetResult();
        return new MemberSurfaceModel(route, member?.Name ?? member?.Email, member is not null);
#else
        return new MemberSurfaceModel(route, User.Identity?.Name, User.Identity?.IsAuthenticated == true);
#endif
    }

    private string SafeReturnUrl(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? returnUrl
            : "/member";
}

public sealed record MemberSurfaceModel(string Route, string? MemberName, bool IsAuthenticated);

public sealed record MemberLoginModel(string ReturnUrl, string? Error);

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
