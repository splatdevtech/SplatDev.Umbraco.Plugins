using SplatDev.Umbraco.Markup.Models;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace SplatDev.Umbraco.Markup.Extensions
{
    public static class GridExtensions
    {
        private static string FirstCharToUpper(this string input) =>
            string.IsNullOrEmpty(input) ? input : char.ToUpper(input[0]) + input[1..];

        private readonly static string AUDIO_TAG = "<audio controls src=\"{0}\"></audio>";

        private readonly static string FILE_TAG = "<a href='{0}' target='_blank' download>{1}</a>";

        private readonly static string IMG_TAG = "<img src='{0}' alt='{1}'/>";

        private readonly static string VIDEO_TAG = "<video width=\"320\" height=\"240\" style=\"width:100%;height:100vh\" controls>\r\n  <source src=\"{0}\" type=\"video/mp4\">\r\n  Your browser does not support the video tag.\r\n</video>";

        public static string? GetMarkup(this BlockGridArea source)
        {
            if (source.Count == 0) return string.Empty;
            var alias = source[0].Content.Properties.ElementAt(0).Alias.FirstCharToUpper();
            var item = source[0];
            return item.GetMarkup(alias);
        }

        public static string? GetMarkup(this BlockGridItem source)
        {
            var alias = source.Content.Properties.ElementAt(0).Alias.FirstCharToUpper();
            var item = source;
            return item.GetMarkup(alias);
        }

        public static string? GetMarkup(this IPublishedElement source, string alias, TagType type, UrlMode mode = UrlMode.Absolute)
        {
            IPublishedContent? item = source.Value<IPublishedContent>(alias);
            if (item is null) return string.Empty;

            string? markup = string.Empty;
            switch (type)
            {
                case TagType.Audio:
                    markup = string.Format(AUDIO_TAG, item!.Url(null, mode));
                    break;
                case TagType.File:
                    markup = string.Format(FILE_TAG, item!.Url(null, mode), item.Name);
                    break;
                case TagType.Image:
                    var altText = item.HasProperty("altText") ? item.Value<string>("altText") : item.Name;
                    markup = string.Format(IMG_TAG, item!.Url(null, mode), altText);
                    if (item!.HasProperty("caption") && item.HasValue("caption"))
                        markup += $"<span class=\"caption\">{item!.Value<string>("caption")}</span>";
                    if (item!.HasProperty("source") && item.HasValue("source"))
                        markup += $"<p class=\"source\">Source: {item!.Value<string>("source")}</p>";
                    break;
                case TagType.Video:
                    markup = string.Format(VIDEO_TAG, item!.Url(null, mode));
                    break;
                case TagType.RichText:
                    markup = source.Value<string>(alias);
                    break;
                default:
                    break;
            }
            return markup;
        }

        private static string? GetMarkup(this BlockGridItem source, string alias, UrlMode mode = UrlMode.Absolute)
        {
            return alias switch
            {
                Constants.Conventions.MediaTypes.Image => source.Content.GetMarkup(alias, TagType.Image, mode),
                Constants.Conventions.MediaTypes.File => source.Content.GetMarkup(alias, TagType.File, mode),
                Constants.Conventions.MediaTypes.Video => source.Content.GetMarkup(alias, TagType.Video, mode),
                Constants.Conventions.MediaTypes.Audio => source.Content.GetMarkup(alias, TagType.Audio, mode),
                _ => source.Content.GetMarkup(alias, TagType.RichText, mode),
            };
        }
    }
}
