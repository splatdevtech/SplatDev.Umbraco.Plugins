using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Packaging;
using Microsoft.Extensions.DependencyInjection;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Migrations;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Configuration;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Composers;

/// <summary>
/// Registers all Schema2Yaml services in the DI container.
/// </summary>
public class Schema2YamlComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // Register configuration
        builder.Services.Configure<Schema2YamlOptions>(
            builder.Config.GetSection(Schema2YamlOptions.SectionName));

        // Register version detector
        builder.Services.AddSingleton<UmbracoVersionDetector>();

        // Register all exporters
        builder.Services.AddScoped<LanguageExporter>();
        builder.Services.AddScoped<DataTypeExporter>();
        builder.Services.AddScoped<DocumentTypeExporter>();
        builder.Services.AddScoped<MediaTypeExporter>();
        builder.Services.AddScoped<TemplateExporter>();
        builder.Services.AddScoped<ContentExporter>();
        builder.Services.AddScoped<MediaExporter>();
        builder.Services.AddScoped<DictionaryExporter>();
        builder.Services.AddScoped<MemberExporter>();
        builder.Services.AddScoped<UserExporter>();

        // Register orchestration service
        builder.Services.AddScoped<ISchemaExportService, SchemaExportService>();

        // Register export profile service
        builder.Services.AddScoped<IExportProfileService, ExportProfileService>();

        // Register DB migration plan so Umbraco runs it on startup
        builder.WithCollectionBuilder<PackageMigrationPlanCollectionBuilder>()
            .Add<Schema2YamlMigrationPlan>();

        // Dashboard is registered via App_Plugins/Schema2Yaml/umbraco-package.json (Umbraco 14+)
    }
}
