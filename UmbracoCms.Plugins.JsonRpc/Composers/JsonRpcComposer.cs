using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.JsonRpc.Models;
using UmbracoCms.Plugins.JsonRpc.Services;

namespace UmbracoCms.Plugins.JsonRpc.Composers;

public class JsonRpcComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
        builder.Services.AddScoped<IJsonRpcService, JsonRpcService>();
        builder.Services.AddDbContext<JsonRpcDbContext>(options =>
            options.UseSqlServer(builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));
    }
}
