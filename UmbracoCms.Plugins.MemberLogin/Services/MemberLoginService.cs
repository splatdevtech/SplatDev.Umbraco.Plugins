using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace UmbracoCms.Plugins.MemberLogin.Services;

public class MemberLoginService : IMemberLoginService
{
    private readonly IMemberService _memberService;
    private readonly IPublicAccessService _publicAccessService;
    private readonly IMemberSignInManagerWrapper _signInManager;
    private readonly ILogger<MemberLoginService> _logger;

    public MemberLoginService(
        IMemberService memberService,
        IPublicAccessService publicAccessService,
        IMemberSignInManagerWrapper signInManager,
        ILogger<MemberLoginService> logger)
    {
        _memberService = memberService;
        _publicAccessService = publicAccessService;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<LoginResult> LoginAsync(string username, string password, bool rememberMe)
    {
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
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("Member signed out.");
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

            _memberService.SavePassword(member, newPassword);
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
