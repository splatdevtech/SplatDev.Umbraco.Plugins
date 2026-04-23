using Microsoft.EntityFrameworkCore;

using SplatDev.Umbraco.Plugins.Settings.Models;

namespace SplatDev.Umbraco.Plugins.Settings.Services
{
    public class SettingsService(SettingsDbContext dbContext) : ISettingsService
    {
        private readonly SettingsDbContext _dbContext = dbContext;

        public async Task<IEnumerable<SettingGroup>> GetAllGroupsAsync()
        {
            return await _dbContext.SettingGroups
                .OrderBy(g => g.SortOrder)
                .ThenBy(g => g.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<SiteSetting>> GetSettingsByGroupAsync(int groupId)
        {
            return await _dbContext.SiteSettings
                .Where(s => s.GroupId == groupId)
                .OrderBy(s => s.Key)
                .ToListAsync();
        }

        public async Task<SiteSetting?> GetSettingAsync(string key)
        {
            return await _dbContext.SiteSettings
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Key == key);
        }

        public async Task<SiteSetting> SetSettingAsync(string key, string value)
        {
            var existing = await _dbContext.SiteSettings.FirstOrDefaultAsync(s => s.Key == key);
            if (existing is not null)
            {
                existing.Value = value;
                _dbContext.SiteSettings.Update(existing);
                await _dbContext.SaveChangesAsync();
                return existing;
            }

            var setting = new SiteSetting
            {
                Key = key,
                Value = value,
                Type = "text"
            };
            _dbContext.SiteSettings.Add(setting);
            await _dbContext.SaveChangesAsync();
            return setting;
        }

        public async Task DeleteSettingAsync(int id)
        {
            var setting = await _dbContext.SiteSettings.FindAsync(id);
            if (setting is not null)
            {
                _dbContext.SiteSettings.Remove(setting);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
