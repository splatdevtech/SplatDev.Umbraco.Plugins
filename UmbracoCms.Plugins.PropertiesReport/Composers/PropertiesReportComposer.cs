using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.PropertiesReport.Services;

namespace UmbracoCms.Plugins.PropertiesReport.Composers;

public class PropertiesReportComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IPropertiesReportService, PropertiesReportService>();
    }
}
