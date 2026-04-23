using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

using SplatDev.Umbraco.Plugins.RdpManager.Models;
using SplatDev.Umbraco.Plugins.RdpManager.Services;

namespace SplatDev.Umbraco.Plugins.RdpManager.Composers
{
    public class RdpManagerComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddDbContext<RdpManagerDbContext>(options =>
                options.UseSqlServer(builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

            builder.Services.AddScoped<IRdpManagerService, RdpManagerService>();
        }
    }
}
