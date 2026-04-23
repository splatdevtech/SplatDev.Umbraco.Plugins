using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using UmbracoCms.Plugins.VisitorCounter.Middleware;
using UmbracoCms.Plugins.VisitorCounter.Models;
using UmbracoCms.Plugins.VisitorCounter.Services;

namespace UmbracoCms.Plugins.VisitorCounter.Composers;

public class VisitorCounterComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddDbContext<VisitorCounterDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

        builder.Services.AddScoped<IVisitorCounterService, VisitorCounterService>();
        builder.Services.AddTransient<VisitorCounterMiddleware>();

        builder.Services.Configure<UmbracoPipelineOptions>(options =>
        {
            options.AddFilter(new UmbracoPipelineFilter("VisitorCounterMiddleware")
            {
                PostPipeline = app => app.UseMiddleware<VisitorCounterMiddleware>()
            });
        });
    }
}
