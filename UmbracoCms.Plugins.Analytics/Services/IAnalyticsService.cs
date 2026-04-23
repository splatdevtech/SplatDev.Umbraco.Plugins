using UmbracoCms.Plugins.Analytics.Models;

namespace UmbracoCms.Plugins.Analytics.Services;

public interface IAnalyticsService
{
    Task<AnalyticsSettings> GetSettingsAsync();
    Task SaveSettingsAsync(AnalyticsSettings settings);
    Task<IEnumerable<object>> GetPageViewsAsync(string measurementId);
}
