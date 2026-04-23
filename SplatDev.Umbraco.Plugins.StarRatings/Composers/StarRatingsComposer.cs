using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.StarRatings.Models;
using SplatDev.Umbraco.Plugins.StarRatings.Services;

namespace SplatDev.Umbraco.Plugins.StarRatings.Composers;

public class StarRatingsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddDbContext<StarRatingsDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

        builder.Services.AddScoped<IStarRatingsService, StarRatingsService>();
    }
}
