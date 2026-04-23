using UmbracoCms.Plugins.SocialMedia.Login.Models;

namespace UmbracoCms.Plugins.SocialMedia.Login.Services
{
    public interface ISocialLoginService
    {
        List<SocialLoginConfig> GetEnabledProviders();
        string GetLoginUrl(SocialProvider provider, string redirectUri);
    }
}
