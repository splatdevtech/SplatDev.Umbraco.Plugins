namespace SplatDev.Umbraco.Tools.T4.Plugins.Templates;

public static class ComposerTemplate
{
    public static string Render(string name, string @namespace) => $$"""
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace {{@namespace}};

public class {{name}}Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<I{{name}}Service, {{name}}Service>();
    }
}
""";
}
