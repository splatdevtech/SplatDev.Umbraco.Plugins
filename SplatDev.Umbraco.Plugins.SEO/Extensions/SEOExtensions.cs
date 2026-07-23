using SplatDev.Umbraco.Plugins.SEO.Models;

using System.Text.RegularExpressions;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace SplatDev.Umbraco.Plugins.SEO.Extensions
{
    public static partial class SEOExtensions
    {
        public static OpenGraph GetOpenGraph(this IPublishedContent page)
        {
            var images = page!.Value<IEnumerable<IPublishedContent>>("shareImage");

            return new OpenGraph
            {
                Title = page!.Value<string>("metaTitle") is not "" ? page!.Value<string>("metaTitle") : page!.Name,
                Type = page!.Value<string>("type") is not "" ? page!.Value<string>("type") : "article",
                Url = page!.Value<string>("canonical") is not "" ? page!.Value<string>("canonical") : page!.Url(mode: UrlMode.Absolute),
                Image = images is not null && images.Any() ? images.ElementAt(0)?.Url(mode: UrlMode.Absolute) : "",
                Description = page!.Value<string>("metaDescription") is not "" ? page!.Value<string>("metaDescription") : "",
                Author = page!.Value<string>("author"),
                DateCreated = page!.CreateDate.ToString("dd-MM-yyyy")
            };
        }

        public static bool IsSubdomainOrNonWww(string url)
        {
            Uri uri = new(url);
            string host = uri.Host;

            // Split the host into parts
            string[] parts = host.Split('.');

            // Check if it's a subdomain (more than 2 parts) or non-www
            return parts.Length > 2 || (parts.Length == 2 && parts[0] != "www");
        }


        public static bool IsSubdomain(string url)
        {
            Uri uri = new(url);
            string host = uri.Host;

            // Split the host into parts
            string[] parts = host.Split('.');

            // Check if it's a subdomain (more than 2 parts)
            return parts.Length > 2;
        }

        public static bool IsSubdomainNonWww(string url)
        {
            Uri uri = new(url);
            string host = uri.Host;

            // Split the host into parts
            string[] parts = host.Split('.');

            // Check if it's or non-www
            return parts.Length > 2 && parts[0] != "www";
        }

        public static bool IsSubdomainAdmin(string url)
        {
            Uri uri = new(url);
            string host = uri.Host;

            // Split the host into parts
            string[] parts = host.Split('.');

            // Check if it's or non-www
            return parts.Length > 2 && parts[0] == "edit";
        }

        public static string StripHtmlTagsCompiled(this string input)
        {
            return HtmlTags().Replace(input, string.Empty);
        }

        [GeneratedRegex("<.*?>", RegexOptions.Compiled)]
        private static partial Regex HtmlTags();
    }
}
