using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
#if !NET10_0_OR_GREATER
using Umbraco.Cms.Web.Common.Security;
#endif

namespace UmbracoCms.Plugins.MemberLogin.Services;

public class MemberLoginService : IMemberLoginService
{
    private readonly IMemberService _memberService;
    private readonly IPublicAccessService _publicAccessService;
#if !NET10_0_OR_GREATER
    private readonly IMemberSignInManager _signInManager;
#endif
    private readonly IMemberManager _memberManager;
    private readonly ILogger<MemberLoginService> _logger;

#if NET10_0_OR_GREATER
    public MemberLoginService(
        IMemberService memberService,
        IPublicAccessService publicAccessService,
        IMemberManager memberManager,
        ILogger<MemberLoginService> logger)
    {
        _memberService = memberService;
        _publicAccessService = publicAccessService;
        _memberManager = memberManager;
        _logger = logger;
    }
#else
    public MemberLoginService(
        IMemberService memberService,
        IPublicAccessService publicAccessService,
        IMemberSignInManager signInManager,
        IMemberManager memberManager,
        ILogger<MemberLoginService> logger)
    {
        _memberService = memberService;
        _publicAccessService = publicAccessService;
        _signInManager = signInManager;
        _memberManager = memberManager;
        _logger = logger;
    }
#endif

    public async Task<LoginResult> LoginAsync(string username, string password, bool rememberMe)
    {
#if NET10_0_OR_GREATER
        // IMemberSignInManager was removed in Umbraco 17.
        // A proper implementation should use the new authentication APIs.
        throw new NotSupportedException(
            "IMemberSignInManager is not available in Umbraco 17 (net10.0). " +
            "Member sign-in must be implemented using the new authentication approach.");
#else
        try
        {
            var member = _memberService.GetByUsername(username)
                      ?? _memberService.GetByEmail(username);

            if (member is null)
                return new LoginResult(false, false, "Invalid username or password.");

            if (member.IsLockedOut)
                return new LoginResult(false, true, "Account is locked. Please contact support.");

            if (!member.IsApproved)
                return new LoginResult(false, false, "Account is not yet approved.");

            var result = await _signInManager.PasswordSignInAsync(username, password, rememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("Member {Username} logged in successfully.", username);
                return new LoginResult(true, false, null);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Member {Username} account locked after failed login attempts.", username);
                return new LoginResult(false, true, "Account has been locked due to too many failed attempts.");
            }

            return new LoginResult(false, false, "Invalid username or password.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Username}.", username);
            return new LoginResult(false, false, "An error occurred during login.");
        }
#endif
    }

    public async Task LogoutAsync()
    {
#if NET10_0_OR_GREATER
        // IMemberSignInManager was removed in Umbraco 17.
        // A proper implementation should use the new authentication APIs.
        throw new NotSupportedException(
            "IMemberSignInManager is not available in Umbraco 17 (net10.0). " +
            "Member sign-out must be implemented using the new authentication approach.");
#else
        await _signInManager.SignOutAsync();
        _logger.LogInformation("Member signed out.");
#endif
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        try
        {
            var member = _memberService.GetByEmail(email);
            if (member is null)
            {
                // Return true to avoid user enumeration
                _logger.LogInformation("Password reset requested for unknown email {Email}.", email);
                return true;
            }

            var token = Guid.NewGuid().ToString("N");
            member.SetValue("passwordResetToken", token);
            member.SetValue("passwordResetExpiry", DateTime.UtcNow.AddHours(2).ToString("O"));
            _memberService.Save(member);

            _logger.LogInformation("Password reset token generated for member {Email}.", email);
            // In a real implementation, send email with reset link
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating password reset for {Email}.", email);
            return false;
        }
    }

    public async Task<ResetPasswordResult> ResetPasswordAsync(string email, string token, string newPassword)
    {
        try
        {
            var member = _memberService.GetByEmail(email);
            if (member is null)
                return new ResetPasswordResult(false, "Invalid reset request.");

            var storedToken = member.GetValue<string>("passwordResetToken");
            var expiryStr = member.GetValue<string>("passwordResetExpiry");

            if (string.IsNullOrEmpty(storedToken) || storedToken != token)
                return new ResetPasswordResult(false, "Invalid or expired reset token.");

            if (DateTime.TryParse(expiryStr, out var expiry) && expiry < DateTime.UtcNow)
                return new ResetPasswordResult(false, "Reset token has expired.");

            // Set password via IMemberManager (SavePassword was removed in Umbraco 13+)
            var identityUser = await _memberManager.FindByEmailAsync(email);
            if (identityUser is null)
                return new ResetPasswordResult(false, "Unable to update password.");

            var resetToken = await _memberManager.GeneratePasswordResetTokenAsync(identityUser);
            var passwordResult = await _memberManager.ResetPasswordAsync(identityUser, resetToken, newPassword);
            if (!passwordResult.Succeeded)
                return new ResetPasswordResult(false, "Password does not meet requirements.");

            member.SetValue("passwordResetToken", string.Empty);
            member.SetValue("passwordResetExpiry", string.Empty);
            member.IsLockedOut = false;
            _memberService.Save(member);

            _logger.LogInformation("Password successfully reset for member {Email}.", email);
            return new ResetPasswordResult(true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for {Email}.", email);
            return new ResetPasswordResult(false, "An error occurred while resetting the password.");
        }
    }
}
