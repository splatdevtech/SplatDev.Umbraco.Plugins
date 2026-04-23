using SplatDev.Umbraco.Plugins.CacheManager.Models;
using SplatDev.Umbraco.Plugins.CacheManager.Repositories;

namespace SplatDev.Umbraco.Plugins.CacheManager.Services
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
