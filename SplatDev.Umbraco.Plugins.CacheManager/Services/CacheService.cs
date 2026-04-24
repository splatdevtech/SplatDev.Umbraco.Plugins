using Microsoft.Extensions.Caching.Memory;

namespace SplatDev.Umbraco.Plugins.CacheManager.Services
{
    public class CacheService(IMemoryCache memoryCache) : ICacheService
    {
        private readonly IMemoryCache _memoryCache = memoryCache;

        public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null)
        {
            if (!_memoryCache.TryGetValue(key, out T? cachedResult))
            {
                cachedResult = await factory();

                var cacheEntryOptions = new MemoryCacheEntryOptions();

                if (slidingExpiration.HasValue)
                {
                    cacheEntryOptions.SetSlidingExpiration(slidingExpiration.Value);
                }

                if (absoluteExpiration.HasValue)
                {
                    cacheEntryOptions.SetAbsoluteExpiration(absoluteExpiration.Value);
                }

                _memoryCache.Set(key, cachedResult, cacheEntryOptions);
            }

            return cachedResult;
        }

        public T? GetOrCreate<T>(string key, Func<T> factory, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null)
        {
            if (!_memoryCache.TryGetValue(key, out T? cachedResult))
            {
                cachedResult = factory();

                var cacheEntryOptions = new MemoryCacheEntryOptions();

                if (slidingExpiration.HasValue)
                {
                    cacheEntryOptions.SetSlidingExpiration(slidingExpiration.Value);
                }

                if (absoluteExpiration.HasValue)
                {
                    cacheEntryOptions.SetAbsoluteExpiration(absoluteExpiration.Value);
                }

                _memoryCache.Set(key, cachedResult, cacheEntryOptions);
            }

            return cachedResult;
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}