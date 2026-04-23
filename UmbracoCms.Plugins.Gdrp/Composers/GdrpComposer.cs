using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.Gdrp.Models;
using UmbracoCms.Plugins.Gdrp.Services;

namespace UmbracoCms.Plugins.Gdrp.Composers;

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
