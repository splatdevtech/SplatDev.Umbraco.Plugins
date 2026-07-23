using Microsoft.AspNetCore.Http;

using System.Text.RegularExpressions;
using System.Web;

namespace SplatDev.Umbraco.Plugins.RedirectManager.Extensions
{
    public static class PatternUrlRedirector
    {
        public static bool HandleQuotePaginationRedirect(string url, HttpResponse response)
        {
            // Match URLs in the format: /quotes/by-{author}/{pageNumber}
            var pattern = @"^/quotes/by-(?<author>[a-zA-Z-]+)/(?<page>\d+)$";
            var match = Regex.Match(url, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var author = match.Groups["author"].Value;
                var page = match.Groups["page"].Value;

                // URL encode the author name for safety
                var encodedAuthor = HttpUtility.UrlEncode(author);

                // Build new URL format
                var newUrl = $"/quotes/by-{encodedAuthor}?page={page}";

                // Perform permanent redirect (301)
                response.Redirect(newUrl, true);
                return true;
            }

            return false;
        }

        public static bool HandleAboutTopicRedirect(string url, HttpResponse response)
        {
            // Match URLs in the format: /quotes/about-{topic}/{pageNumber}
            var pattern = @"^/quotes/about-(?<topic>[a-zA-Z-]+)/(?<page>\d+)$";
            var match = Regex.Match(url, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var topic = match.Groups["topic"].Value;
                var page = match.Groups["page"].Value;

                // URL encode the topic for special characters
                var encodedTopic = HttpUtility.UrlEncode(topic);

                // Build new URL format with query string
                var newUrl = $"/quotes/about-{encodedTopic}?page={page}";

                // Permanent redirect (301) for SEO preservation
                response.Redirect(newUrl, true);
                return true;
            }

            return false;
        }

        public static bool HandleAuthorTopicRedirect(string url, HttpResponse response)
        {
            // Match pattern: /{author}-quotes-about-{topic}
            var pattern = @"^/(?<author>[a-z0-9-]+)-quotes-about-(?<topic>[a-z0-9-]+)$";
            var match = Regex.Match(url, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var author = match.Groups["author"].Value.ToLowerInvariant();
                var topic = match.Groups["topic"].Value.ToLowerInvariant();

                // Encode special characters
                var encodedAuthor = HttpUtility.UrlEncode(author);
                var encodedTopic = HttpUtility.UrlEncode(topic);

                // Build new URL structure
                var newUrl = $"/quotes/by-{encodedAuthor}?topic={encodedTopic}";

                response.Redirect(newUrl, true);
                return true;
            }

            return false;
        }
    }
}