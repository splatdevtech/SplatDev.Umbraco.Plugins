using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.StarRatings.Models;
using UmbracoCms.Plugins.StarRatings.Services;

namespace UmbracoCms.Plugins.StarRatings.Composers;

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
