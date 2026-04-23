using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

using UmbracoCms.Plugins.SocialMedia.Login.Services;

namespace UmbracoCms.Plugins.SocialMedia.Login.Composers
{
    public class SocialLoginComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddScoped<ISocialLoginService, SocialLoginService>();
        }
    }
}
