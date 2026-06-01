using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

using SplatDev.Umbraco.Plugins.SocialMedia.Channels.Models;
using SplatDev.Umbraco.Plugins.SocialMedia.Channels.Services;

namespace SplatDev.Umbraco.Plugins.SocialMedia.Channels.Composers
{
    public class SocialChannelsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddDbContext<SocialChannelsDbContext>(options =>
                options.UseSqlServer(builder.Config.GetSection("ConnectionStrings")["umbracoDbDSN"] ?? string.Empty));

            builder.Services.AddScoped<ISocialChannelsService, SocialChannelsService>();
        }
    }
}
