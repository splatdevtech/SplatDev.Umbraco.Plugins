using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;

using SplatDev.Umbraco.Plugins.ShortUrls.Services;

using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Extensions;

namespace SplatDev.Umbraco.Plugins.ShortUrls.Controllers
{
    public class ShortUrlController<IShortUrl>(ILogger<UmbracoPageController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor,
            IShortUrlService shortUrlService,
            IProfiler profiler) : UmbracoPageController(logger, compositeViewEngine), IVirtualPageController
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor = umbracoContextAccessor;
        private readonly IShortUrlService _shortUrlService = shortUrlService;
        private readonly IProfiler _profiler = profiler;
        public const string BASE_URL = "/s/{random}";
        public const string BASE_URL_FORMAT = "/s/{0}";
        public const string ENTITY_ALIAS = "shortUrl";
        public const string BASE_CULTURE = "en-US";
        public const string REDIRECT_URL_FRAGMENT = "/quote/by-{0}/{1}";

        public IPublishedContent? FindContent(ActionExecutingContext actionExecutingContext)
        {
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
            {
#if NET10_0_OR_GREATER
                return null; // GetAtRoot() removed in Umbraco 17; use IPublishedContentQuery instead
#else
                return umbracoContext.Content!.GetAtRoot().DescendantsOrSelfOfType(ENTITY_ALIAS, BASE_CULTURE).FirstOrDefault();
#endif
            }
            return null;
        }

        [HttpGet]
        [ResponseCache(Duration = 0, NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> GetFromShortUrl(string shortUrl)
        {
            return await GetByShortUrl(shortUrl);
        }

        private async Task<IActionResult> GetByShortUrl(string shortUrl)
        {
            var redirectUrl = await Task.FromResult(_shortUrlService.Get(shortUrl));
            return Redirect(redirectUrl);
        }
    }
}