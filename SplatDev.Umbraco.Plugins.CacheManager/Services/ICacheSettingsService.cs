using SplatDev.Umbraco.Plugins.CacheManager.Models;

namespace SplatDev.Umbraco.Plugins.CacheManager.Services
{
    public interface ICacheSettingsService
    {
        Configuration? Configuration { get; init; }
    }
}
