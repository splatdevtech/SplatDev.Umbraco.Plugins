using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

using UmbracoCms.Plugins.SocialMedia.Channels.Models;
using UmbracoCms.Plugins.SocialMedia.Channels.Services;

namespace UmbracoCms.Plugins.SocialMedia.Channels.Composers
{
    public class SocialChannelsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddDbContext<SocialChannelsDbContext>(options =>
                options.UseSqlServer(builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

            builder.Services.AddScoped<ISocialChannelsService, SocialChannelsService>();
        }
    }
}
