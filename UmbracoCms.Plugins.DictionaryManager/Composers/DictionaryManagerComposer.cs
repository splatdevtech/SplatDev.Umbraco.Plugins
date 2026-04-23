using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.DictionaryManager.Services;

namespace UmbracoCms.Plugins.DictionaryManager.Composers;

public class DictionaryManagerComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IDictionaryManagerService, DictionaryManagerService>();
    }
}
