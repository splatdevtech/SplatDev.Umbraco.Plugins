using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.CopyValue.Models;
using SplatDev.Umbraco.Plugins.CopyValue.Services;

namespace SplatDev.Umbraco.Plugins.CopyValue.Composers;

public class CopyValueComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddDbContext<CopyValueDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

        builder.Services.AddScoped<ICopyValueService, CopyValueService>();
    }
}
