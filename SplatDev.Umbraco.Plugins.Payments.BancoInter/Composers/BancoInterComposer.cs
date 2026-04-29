using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SplatDev.Umbraco.Plugins.Payments.BancoInter.Models;
using SplatDev.Umbraco.Plugins.Payments.BancoInter.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace SplatDev.Umbraco.Plugins.Payments.BancoInter.Composers;

public class BancoInterComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddMemoryCache();
        builder.Services.AddHttpClient("BancoInter");

        builder.Services.AddScoped<IBancoInterAuthService, BancoInterAuthService>();
        builder.Services.AddScoped<IBancoInterPixService, BancoInterPixService>();
        builder.Services.AddScoped<IBancoInterBoletoService, BancoInterBoletoService>();
        builder.Services.AddScoped<IBancoInterBankingService, BancoInterBankingService>();

        builder.Services.AddDbContext<BancoInterDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));
    }
}
