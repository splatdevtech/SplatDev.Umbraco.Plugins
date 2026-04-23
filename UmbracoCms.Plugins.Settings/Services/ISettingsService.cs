using UmbracoCms.Plugins.Settings.Models;

namespace UmbracoCms.Plugins.Settings.Services
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
