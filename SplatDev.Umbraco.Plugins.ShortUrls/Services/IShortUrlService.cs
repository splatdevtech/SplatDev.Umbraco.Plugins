namespace SplatDev.Umbraco.Plugins.ShortUrls.Services
{
    public interface IShortUrlService
    {
        public string GenerateShortUrl();
        string Get(string shortUrl);
    }
}
