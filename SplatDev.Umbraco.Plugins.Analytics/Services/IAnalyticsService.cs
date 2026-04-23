using SplatDev.Umbraco.Plugins.Analytics.Models;

namespace SplatDev.Umbraco.Plugins.Analytics.Services;

public interface IAnalyticsService
{
    Task<AnalyticsSettings> GetSettingsAsync();
    Task SaveSettingsAsync(AnalyticsSettings settings);
    Task<IEnumerable<object>> GetPageViewsAsync(string measurementId);
}
