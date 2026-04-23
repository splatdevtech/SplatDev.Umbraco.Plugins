using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace SplatDev.Umbraco.Common.Extensions
{
    public static partial class FallbackExtensions
    {
        public static string? ValueFallbackNodeName(this IPublishedContent content, string alias)
        {
            return content.HasValue(alias) ? content.Value<string>(alias) : content.Name;
        }

        public static string? ValueWithFallback(this IPublishedContent content, string alias)
        {
            return content.Value<string>(alias, fallback: Fallback.ToAncestors);
        }

        public static string? ValueWithFallback(this IPublishedContent content, string alias, string defaultText = "")
        {
            return content.HasValue(alias) ? content.Value<string>(alias) : defaultText;
        }
    }
}
