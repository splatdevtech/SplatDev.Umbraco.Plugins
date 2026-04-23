using SplatDev.Umbraco.Plugins.Settings.Models;

namespace SplatDev.Umbraco.Plugins.Settings.Services
{
    public interface ISettingsService
    {
        Task<IEnumerable<SettingGroup>> GetAllGroupsAsync();
        Task<IEnumerable<SiteSetting>> GetSettingsByGroupAsync(int groupId);
        Task<SiteSetting?> GetSettingAsync(string key);
        Task<SiteSetting> SetSettingAsync(string key, string value);
        Task DeleteSettingAsync(int id);
    }
}
