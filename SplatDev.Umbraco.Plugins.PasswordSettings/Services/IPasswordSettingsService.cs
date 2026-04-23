using SplatDev.Umbraco.Plugins.PasswordSettings.Models;

namespace SplatDev.Umbraco.Plugins.PasswordSettings.Services;

public interface IPasswordSettingsService
{
    Task<PasswordPolicy?> GetPolicyAsync();
    Task<PasswordPolicy> SavePolicyAsync(PasswordPolicy policy);
    Task<(bool Valid, string[] Errors)> ValidatePasswordAsync(string password, PasswordPolicy policy);
    Task RecordPasswordChangeAsync(int memberId, string passwordHash);
    Task<bool> IsPasswordReusedAsync(int memberId, string passwordHash, int historyCount);
}
