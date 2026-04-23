using Umbraco.Plugins.CacheManager.Models;

namespace Umbraco.Plugins.CacheManager.Services
{
    public interface ICacheSettingsService
    {
        Configuration? Configuration { get; init; }
    }
}
