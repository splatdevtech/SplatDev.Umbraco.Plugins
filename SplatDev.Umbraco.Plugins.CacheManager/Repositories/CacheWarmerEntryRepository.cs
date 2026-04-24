using NPoco;

using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using SplatDev.Umbraco.Plugins.CacheManager.Models;

namespace SplatDev.Umbraco.Plugins.CacheManager.Repositories
{
    public class CacheWarmerEntryRepository(IScopeProvider scopeProvider)
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;

        public void AddCacheEntry(CacheWarmerEntry cacheEntry)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            scope.Database.Insert(cacheEntry);
        }

        public void DeleteCacheEntry(int id)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            scope.Database.Delete<CacheWarmerEntry>(id);
        }

        public void DeleteAll()
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            scope.Database.DeleteMany<CacheWarmerEntry>().Execute();
        }

        public CacheWarmerEntry? GetCacheEntry(int id)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            return scope.Database.Fetch<CacheWarmerEntry>()?.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<CacheWarmerEntry> GetCacheEntries()
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            int task = GetLatestTask();
            return scope.Database.Fetch<CacheWarmerEntry>().Where(x => x.Task == task).AsEnumerable();
        }

        public int GetLatestTask()
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var last = scope.Database.Query<CacheWarmerEntry>(new Sql($"SELECT * FROM [{CacheWarmerEntry.TABLE_NAME}] WHERE [Task] = (SELECT MAX([Task]) FROM [{CacheWarmerEntry.TABLE_NAME}]) ")).FirstOrDefault();
            return last is null ? 0 : last.Task;
        }
    }

}
