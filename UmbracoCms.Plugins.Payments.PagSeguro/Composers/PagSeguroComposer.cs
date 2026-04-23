using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.Payments.PagSeguro.Models;
using UmbracoCms.Plugins.Payments.PagSeguro.Services;

namespace UmbracoCms.Plugins.Payments.PagSeguro.Composers;

public class PagSeguroComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddHttpClient();

        builder.Services.AddScoped<IPagSeguroService, PagSeguroService>();

        builder.Services.AddDbContext<PagSeguroDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));
    }
}
