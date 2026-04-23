using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.CopyValue.Models;
using UmbracoCms.Plugins.CopyValue.Services;

namespace UmbracoCms.Plugins.CopyValue.Composers;

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
