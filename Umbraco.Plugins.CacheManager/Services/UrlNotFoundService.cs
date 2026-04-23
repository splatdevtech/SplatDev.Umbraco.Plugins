using Umbraco.Plugins.CacheManager.Models;
using Umbraco.Plugins.CacheManager.Repositories;

namespace Umbraco.Plugins.CacheManager.Services
{
    public class UrlNotFoundService(UrlNotFoundRepository urlNotFoundRepository)
    {
        private readonly UrlNotFoundRepository _urlNotFoundRepository = urlNotFoundRepository;

        public void ClearLog()
        {
            _urlNotFoundRepository.DeleteAll();
        }

        public void LogUrlNotFound(string url)
        {
            _urlNotFoundRepository.AddUrlNotFound(new UrlNotFound { Url = url });
        }
    }
}
