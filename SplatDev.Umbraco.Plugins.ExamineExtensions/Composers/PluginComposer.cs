using Umbraco.Cms.Core.Composing;
using SplatDev.Umbraco.Plugins.ExamineExtensions.Services;

namespace SplatDev.Umbraco.Plugins.ExamineExtensions.Composers;

public class PluginComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IExamineExtensionsService, ExamineExtensionsService>();
    }
}
