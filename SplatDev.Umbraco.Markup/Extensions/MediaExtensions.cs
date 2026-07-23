using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace SplatDev.Umbraco.Markup.Extensions
{
    public static class MediaExtensions
    {
        public static string AltText(this IPublishedContent content)
        {
            return content.HasProperty("altText") && content.HasValue("altText") ? content.Value<string>("altText")! : content.Name;
        }
    }
}
