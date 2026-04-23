using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.ShopCart.Models;
using UmbracoCms.Plugins.ShopCart.Services;

namespace UmbracoCms.Plugins.ShopCart.Composers;

public class ShopCartComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IShopCartService, ShopCartService>();

        builder.Services.AddDbContext<ShopCartDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));
    }
}
