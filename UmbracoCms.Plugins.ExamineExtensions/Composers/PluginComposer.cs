using Umbraco.Cms.Core.Composing;
using UmbracoCms.Plugins.ExamineExtensions.Services;

namespace UmbracoCms.Plugins.ExamineExtensions.Composers;

public class PluginComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IExamineExtensionsService, ExamineExtensionsService>();
    }
}
