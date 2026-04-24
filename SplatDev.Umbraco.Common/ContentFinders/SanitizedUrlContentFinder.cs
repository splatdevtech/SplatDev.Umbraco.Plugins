using Umbraco.Cms.Core.Routing;

namespace SplatDev.Umbraco.Common.ContentFinders
{
    public class SanitizedUrlContentFinder : IContentFinder
    {
        public const string BASE_FRAGMENT = "/quote/by-";
        public Task<bool> TryFindContent(IPublishedRequestBuilder request)
        {
            var path = request.Uri.AbsolutePath;
            // Strip query parameters for specific routes
            if (path.StartsWith(BASE_FRAGMENT, StringComparison.OrdinalIgnoreCase))
            {
                request.SetRedirect(request.Uri.GetLeftPart(UriPartial.Path));
            }
            return Task.FromResult(false); // Let other finders handle routing
        }
    }
}