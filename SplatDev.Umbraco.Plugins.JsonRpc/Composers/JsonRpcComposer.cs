using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.JsonRpc.Models;
using SplatDev.Umbraco.Plugins.JsonRpc.Services;

namespace SplatDev.Umbraco.Plugins.JsonRpc.Composers;

public class JsonRpcComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
        builder.Services.AddScoped<IJsonRpcService, JsonRpcService>();
        builder.Services.AddDbContext<JsonRpcDbContext>(options =>
            options.UseSqlServer(builder.Config.GetSection("ConnectionStrings")["umbracoDbDSN"] ?? string.Empty));
    }
}
