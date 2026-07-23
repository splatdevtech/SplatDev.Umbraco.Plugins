using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Microsoft.Extensions.DependencyInjection;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Handlers;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Composers
{
    public class YamlStartupComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Register YamlParser
            builder.Services.AddScoped<YamlParser>();

            // Register all Creators
            builder.Services.AddScoped<DataTypeCreator>();
            builder.Services.AddScoped<DocumentTypeCreator>();
            builder.Services.AddScoped<MediaTypeCreator>();
            builder.Services.AddScoped<TemplateCreator>();
            builder.Services.AddScoped<ContentCreator>();
            builder.Services.AddScoped<MediaCreator>();
            builder.Services.AddScoped<StaticAssetCreator>();
            builder.Services.AddScoped<LanguageCreator>();
            builder.Services.AddScoped<DictionaryCreator>();
            builder.Services.AddScoped<MemberCreator>();
            builder.Services.AddScoped<MemberTypeCreator>();
            builder.Services.AddScoped<MemberGroupCreator>();
            builder.Services.AddScoped<UserCreator>();
            builder.Services.AddScoped<PackageInstaller>();
            builder.Services.AddScoped<PackageValidator>();
            builder.Services.AddScoped<PropertyEditorCreator>();
            builder.Services.AddScoped<ModelsBuilderConfigurator>();
            builder.Services.AddScoped<PublishedModelsGenerator>();

            // IHttpClientFactory is needed by MediaCreator for URL-based file downloads
            builder.Services.AddHttpClient();

            // Register the initialization handler for startup notification
            builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, YamlInitializationHandler>();
        }
    }
}
