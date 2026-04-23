using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.DefaultValue.Models;
using UmbracoCms.Plugins.DefaultValue.Services;

namespace UmbracoCms.Plugins.DefaultValue.Composers;

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
