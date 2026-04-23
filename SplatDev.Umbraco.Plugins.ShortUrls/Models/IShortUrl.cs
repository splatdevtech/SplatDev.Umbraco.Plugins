namespace SplatDev.Umbraco.Plugins.ShortUrls.Models
{
    public interface IShortUrl
    {
        string? ShortUrl { get; set; }
        string Url { get; set; }
    }
}
