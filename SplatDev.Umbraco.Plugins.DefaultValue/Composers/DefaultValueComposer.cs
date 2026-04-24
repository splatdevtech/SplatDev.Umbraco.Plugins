using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.DefaultValue.Models;
using SplatDev.Umbraco.Plugins.DefaultValue.Services;

namespace SplatDev.Umbraco.Plugins.DefaultValue.Composers;

public class DefaultValueComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddDbContext<DefaultValueDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

        builder.Services.AddScoped<IDefaultValueService, DefaultValueService>();
    }
}
