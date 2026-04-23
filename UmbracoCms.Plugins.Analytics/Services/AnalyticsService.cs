using Microsoft.Extensions.Configuration;
using UmbracoCms.Plugins.Analytics.Models;

namespace UmbracoCms.Plugins.Analytics.Services;

public class AnalyticsService : IAnalyticsService
{
    private const string SectionKey = "UmbracoCms:Analytics:MeasurementId";
    private const string EnabledKey = "UmbracoCms:Analytics:Enabled";

    private readonly IConfiguration _configuration;

    public AnalyticsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<AnalyticsSettings> GetSettingsAsync()
    {
        var settings = new AnalyticsSettings
        {
            MeasurementId = _configuration[SectionKey] ?? string.Empty,
            Enabled = bool.TryParse(_configuration[EnabledKey], out var enabled) ? enabled : true
        };

        return Task.FromResult(settings);
    }

    public Task SaveSettingsAsync(AnalyticsSettings settings)
    {
        // In-process write via IConfigurationRoot (works for appsettings.json backed stores).
        if (_configuration is IConfigurationRoot root)
        {
            root[SectionKey] = settings.MeasurementId;
            root[EnabledKey] = settings.Enabled.ToString().ToLowerInvariant();
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<object>> GetPageViewsAsync(string measurementId)
    {
        // Placeholder: a real implementation would call the GA Data API.
        // Returns an empty collection so that the endpoint compiles and responds gracefully.
        IEnumerable<object> result = Array.Empty<object>();
        return Task.FromResult(result);
    }
}
