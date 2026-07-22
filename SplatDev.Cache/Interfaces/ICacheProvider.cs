using System.Diagnostics.CodeAnalysis;

namespace SplatDev.Cache;

public interface ICacheProvider
{
    [return: MaybeNull]
    T Get<T>(string key);

    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    void Set<T>(string key, T value, CacheEntryOptions? options = null);

    Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null, CancellationToken cancellationToken = default);

    void Remove(string key);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    Task<bool> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    Task<bool> RemoveByTagAsync(string tag, CancellationToken cancellationToken = default);

    [return: MaybeNull]
    T GetOrCreate<T>(string key, Func<T> factory, CacheEntryOptions? options = null);

    Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        CacheEntryOptions? options = null,
        CancellationToken cancellationToken = default);
}
