using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.Faqs.Models;
using SplatDev.Umbraco.Plugins.Faqs.Services;

namespace SplatDev.Umbraco.Plugins.Faqs.Composers;

public class FaqsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddDbContext<FaqsDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

        builder.Services.AddScoped<IFaqsService, FaqsService>();
    }
}
