using Umbraco.Cms.Core.Models;

namespace SplatDev.Umbraco.Plugins.MemberRegistration.Services;

public interface IMemberRegistrationService
{
    Task<RegistrationResult> RegisterAsync(string name, string email, string username, string password, string memberTypeAlias = "Member");
    Task<bool> VerifyEmailAsync(string email, string token);
    Task<bool> ApproveAsync(int memberId);
    Task<IEnumerable<IMember>> GetPendingAsync();
}

public record RegistrationResult(bool Success, int MemberId, string? ErrorMessage);
