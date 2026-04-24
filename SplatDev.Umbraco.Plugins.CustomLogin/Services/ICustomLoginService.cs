using SplatDev.Umbraco.Plugins.CustomLogin.Models;

namespace SplatDev.Umbraco.Plugins.CustomLogin.Services;

public interface ICustomLoginService
{
    Task<CustomLoginSettings> GetSettingsAsync();
    Task SaveSettingsAsync(CustomLoginSettings settings);
    Task<bool> LoginAsync(string username, string password);
    Task<bool> ValidateMemberAsync(string username);
}
