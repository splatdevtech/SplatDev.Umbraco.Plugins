using Umbraco.Cms.Infrastructure.Scoping;
using SplatDev.Umbraco.Plugins.CacheManager.Models;

namespace SplatDev.Umbraco.Plugins.CacheManager.Repositories
{
    public class UrlNotFoundRepository(IScopeProvider scopeProvider)
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;

        public void AddUrlNotFound(UrlNotFound url)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            scope.Database.Insert(url);
        }

        public void DeleteUrlNotFound(int id)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            scope.Database.Delete<UrlNotFound>(id);
        }

        public void DeleteAll()
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            scope.Database.DeleteMany<UrlNotFound>().Execute();
        }

        public UrlNotFound? GetUrlNotFound(int id)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            return scope.Database.Fetch<UrlNotFound>()?.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<UrlNotFound?> GetAllUrlNotFound()
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            return scope.Database.Fetch<UrlNotFound>();
        }
    }
}
