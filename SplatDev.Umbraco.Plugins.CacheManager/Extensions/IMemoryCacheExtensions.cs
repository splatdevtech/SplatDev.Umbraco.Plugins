using Microsoft.Extensions.Caching.Memory;

using System.Collections;
using System.Reflection;
using System.Runtime.Versioning;

namespace SplatDev.Umbraco.Plugins.CacheManager.Extensions
{
    public static class IMemoryCacheExtensions
    {
        [RequiresPreviewFeatures]
        public static List<object> GetKeys(this IMemoryCache memoryCache)
        {
            var cache = memoryCache as MemoryCache;
            var coherentState = cache?.GetType().GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance);
            var coherentStateValue = coherentState?.GetValue(cache);
            var entriesCollection = coherentStateValue?.GetType().GetProperty("StringEntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var entriesCollectionValue = entriesCollection?.GetValue(coherentStateValue) as IDictionary;

            return entriesCollectionValue?.Keys.Cast<object>().ToList() ?? [];
        }
    }
}
