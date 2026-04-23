using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.Tweets.Models;
using SplatDev.Umbraco.Plugins.Tweets.Services;

namespace SplatDev.Umbraco.Plugins.Tweets.Composers;

public class TweetsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.Configure<TweetSettings>(
            builder.Config.GetSection(TweetSettings.SectionKey));

        builder.Services.AddDbContext<TweetsDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

        builder.Services.AddHttpClient("TwitterV2");

        builder.Services.AddScoped<ITweetsService, TweetsService>();
    }
}
