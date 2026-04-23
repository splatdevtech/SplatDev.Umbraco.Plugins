using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.NewsTicker.Models;
using UmbracoCms.Plugins.NewsTicker.Services;

namespace UmbracoCms.Plugins.NewsTicker.Composers;

public class NewsTickerComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.Configure<NewsTickerSettings>(
            builder.Config.GetSection(NewsTickerSettings.SectionKey));

        builder.Services.AddDbContext<NewsTickerDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

        builder.Services.AddScoped<INewsTickerService, NewsTickerService>();
    }
}
