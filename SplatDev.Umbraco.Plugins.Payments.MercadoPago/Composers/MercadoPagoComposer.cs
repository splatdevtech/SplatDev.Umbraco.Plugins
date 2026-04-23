using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.Payments.MercadoPago.Models;
using SplatDev.Umbraco.Plugins.Payments.MercadoPago.Services;

namespace SplatDev.Umbraco.Plugins.Payments.MercadoPago.Composers;

public class MercadoPagoComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddHttpClient();

        builder.Services.AddScoped<IMercadoPagoService, MercadoPagoService>();

        builder.Services.AddDbContext<MercadoPagoDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));
    }
}
