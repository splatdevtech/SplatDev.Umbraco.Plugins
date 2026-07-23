using Microsoft.EntityFrameworkCore;

using SplatDev.Umbraco.Plugins.ShortUrls.Extensions;
using SplatDev.Umbraco.Plugins.ShortUrls.Models;

namespace SplatDev.Umbraco.Plugins.ShortUrls.Services
{
    public class ShortUrlService<T>(DbContext context) : IShortUrlService where T : class, IShortUrl
    {
        private readonly DbContext _context = context;

        public string GenerateShortUrl()
        {
            string random = ShortUrlExtensions.GenerateRandomUrl();
            while (Exists(random))
            {
                random = ShortUrlExtensions.GenerateRandomUrl();
            }
            return random;
        }

        public bool Exists(string random)
        {
            var dbSet = _context.Set<T>();
            var yes = from y in dbSet where y.ShortUrl == random select y;
            if (yes.Any()) return true;
            else return false;
        }

        public string Get(string shortUrl)
        {
            var dbSet = _context.Set<T>();
            var url = from u in dbSet where u.ShortUrl == shortUrl select u.Url;
            return url.First();
        }
    }
}
