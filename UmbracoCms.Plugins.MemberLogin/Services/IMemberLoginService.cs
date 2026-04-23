namespace UmbracoCms.Plugins.MemberLogin.Services;

public interface IMemberLoginService
{
    Task<LoginResult> LoginAsync(string username, string password, bool rememberMe);
    Task LogoutAsync();
    Task<bool> ForgotPasswordAsync(string email);
    Task<ResetPasswordResult> ResetPasswordAsync(string email, string token, string newPassword);
}

public record LoginResult(bool Success, bool IsLockedOut, string? ErrorMessage);
public record ResetPasswordResult(bool Success, string? ErrorMessage);
