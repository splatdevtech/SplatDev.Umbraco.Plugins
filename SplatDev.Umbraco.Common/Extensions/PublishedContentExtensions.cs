using Newtonsoft.Json.Linq;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Extensions;

namespace SplatDev.Umbraco.Common.Extensions
{
    public static class PublishedContentExtensions
    {
        public static bool HasGridValue(this IPublishedContent content, string alias)
        {
            var result = false;
            try
            {
                JObject? grid = content.Value(alias) as JObject;
                if (grid is not null && grid.SelectTokens("sections[*].rows[*].areas[*].controls").Any())
                {
                    result = true;
                }
            }
            catch { }
            return result;
        }
        /// <summary>
        /// Returns children of the <paramref name="source"/>, of the given type <see cref="{T}"/>, that satisfy provided <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="T">Type of the children to return.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate that child has to satisfy.</param>
        /// <returns>Children of given type <see><cref>{T}</cref></see>, that satisfy provided <paramref name="predicate"/>.</returns>
        public static IEnumerable<T>? Children<T>(this IPublishedContent source, Func<T, bool> predicate) where T : class, IPublishedContent
            => source?.Children<T>()?.Where(predicate) ?? [];

        /// <summary>
        /// Returns descendants of the <paramref name="source"/>, of the given <see cref="{T}"/>, that satisfy provided <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="T">Type of the descendants to return.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate that descendant has to satisfy.</param>
        /// <returns>Descendants of given type <see><cref>{T}</cref></see>, that satisfy provided <paramref name="predicate"/>.</returns>
        public static IEnumerable<T> Descendants<T>(this IPublishedContent source, Func<T, bool> predicate) where T : class, IPublishedContent
            => source?.Descendants<T>().Where(predicate) ?? [];

        /// <summary>
        /// Returns the <paramref name="source"/> and its descendants, of the given type <see cref="{T}"/>, that satisfy provided <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="T">Type of the descendants to return.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate that descendant has to satisfy.</param>
        /// <returns><paramref name="source"/> and its descendants of given type <see><cref>{T}</cref></see>, that satisfy provided <paramref name="predicate"/>.</returns>
        public static IEnumerable<T> DescendantsOrSelf<T>(this IPublishedContent source, Func<T, bool> predicate) where T : class, IPublishedContent
            => source?.DescendantsOrSelf<T>().Where(predicate) ?? [];

#if !NET10_0_OR_GREATER
        public static T? GetPublishedContentOfType<T>(this IPublishedContentCache cache, string name = "") where T : class, IPublishedContent
        {
            var nodes = cache.GetAtRoot().DescendantsOrSelf<T>();
            if (!string.IsNullOrEmpty(name)) nodes = nodes.Where(x => x.Name.Equals(name));
            return nodes.FirstOrDefault();
        }

        public static IPublishedContent? GetPublishedContentOfType(this IPublishedContentCache cache, string docTypeAlias, string name = "")
        {
            var nodes = cache.GetAtRoot().DescendantsOrSelfOfType(docTypeAlias);
            if (!string.IsNullOrEmpty(name)) nodes = nodes.Where(x => x.Name.Equals(name));
            return nodes.FirstOrDefault();
        }
#endif
    }
}
