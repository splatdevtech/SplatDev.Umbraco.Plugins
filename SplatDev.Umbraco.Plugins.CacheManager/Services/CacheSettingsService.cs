using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using SplatDev.Umbraco.Plugins.CacheManager.Extensions;
using SplatDev.Umbraco.Plugins.CacheManager.Models;

using static SplatDev.Umbraco.Plugins.CacheManager.Constants.CacheConstants;

namespace SplatDev.Umbraco.Plugins.CacheManager.Services
{
    public class CacheSettingsService : ICacheSettingsService
    {
        public Configuration? Configuration { get; init; }

        private readonly IMemoryCache _memoryCache;
        private readonly IPublishedContentCache _contentCache;

        public CacheSettingsService(IUmbracoContextFactory contextFactory, IPublishedContentCache contentCache, IMemoryCache memoryCache, IConfiguration configuration)
        {
            bool useCache = configuration.GetSection("SettingsCacheOn").Get<bool>();
            using UmbracoContextReference umbracoContextReference = contextFactory.EnsureUmbracoContext();
            _memoryCache = memoryCache;
            _contentCache = contentCache;
            if (useCache)
            {

                if (_memoryCache.TryGetValue("settings-cache", out IPublishedContent? results))
                    Configuration = results as Configuration;
                else
                {
                    Configuration = umbracoContextReference.UmbracoContext.Content!.GetConfiguration();
                    _memoryCache.Set("settings-cache", Configuration, CacheRefresh.THREE_MONTHS);
                }
            }
            else Configuration = umbracoContextReference.UmbracoContext.Content!.GetConfiguration();
        }
    }
}
