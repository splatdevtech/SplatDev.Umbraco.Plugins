using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.TwoFactor.Models;
using SplatDev.Umbraco.Plugins.TwoFactor.Services;

namespace SplatDev.Umbraco.Plugins.TwoFactor.Composers;

public class TwoFactorComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddDbContext<TwoFactorDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

        builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();
    }
}
