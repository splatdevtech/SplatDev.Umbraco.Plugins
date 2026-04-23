using UmbracoCms.Plugins.SocialMedia.Share.Models;

namespace UmbracoCms.Plugins.SocialMedia.Share.Services
{
    public interface IShareService
    {
        IEnumerable<ShareLink> GetShareLinks(string pageUrl, string pageTitle);
    }
}
