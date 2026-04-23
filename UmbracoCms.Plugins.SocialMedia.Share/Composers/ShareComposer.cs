using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

using UmbracoCms.Plugins.SocialMedia.Share.Models;
using UmbracoCms.Plugins.SocialMedia.Share.Services;

namespace UmbracoCms.Plugins.SocialMedia.Share.Composers
{
    public class ShareComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.Configure<ShareConfig>(
                builder.Config.GetSection("SocialMedia:Share"));

            builder.Services.AddScoped<IShareService, ShareService>();
        }
    }
}
