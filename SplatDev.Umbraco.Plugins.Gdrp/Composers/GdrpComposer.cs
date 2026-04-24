using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.Gdrp.Models;
using SplatDev.Umbraco.Plugins.Gdrp.Services;

namespace SplatDev.Umbraco.Plugins.Gdrp.Composers;

public class GdrpComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IGdrpService, GdrpService>();

        builder.Services.AddDbContext<GdrpDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));
    }
}
