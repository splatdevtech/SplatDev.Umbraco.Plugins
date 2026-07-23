using Umbraco.Cms.Infrastructure.Scoping;
using SplatDev.Umbraco.Plugins.RedirectManager.Models;

namespace SplatDev.Umbraco.Plugins.RedirectManager.Repositories
{
    public class RedirectUrlsRepository(IScopeProvider scopeProvider)
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;

        public void AddRedirectionUrl(RedirectUrl url)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            url.CreatedOn = DateTime.Now;
            scope.Database.Insert(url);
        }

        public void EditRedirectionUrl(RedirectUrl url)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            scope.Database.Update(url);
        }

        public void DeleteRedirectionUrl(int id)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            scope.Database.Delete<RedirectUrl>(id);
        }

        public void DeleteAll()
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            scope.Database.DeleteMany<RedirectUrl>().Execute();
        }

        public RedirectUrl? GetRedirectionUrl(int id)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            return scope.Database.Fetch<RedirectUrl>()?.FirstOrDefault(x => x.Id == id);
        }

        public RedirectUrl? GetRedirectionUrl(string url)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            return scope.Database.Fetch<RedirectUrl>()?.FirstOrDefault(x => x.Url == url);
        }

        public IEnumerable<RedirectUrl>? GetAllRedirectionUrls()
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            return scope.Database.Fetch<RedirectUrl>();
        }
    }
}
