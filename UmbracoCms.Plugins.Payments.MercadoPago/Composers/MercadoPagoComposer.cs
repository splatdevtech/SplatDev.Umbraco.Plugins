using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.Payments.MercadoPago.Models;
using UmbracoCms.Plugins.Payments.MercadoPago.Services;

namespace UmbracoCms.Plugins.Payments.MercadoPago.Composers;

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
