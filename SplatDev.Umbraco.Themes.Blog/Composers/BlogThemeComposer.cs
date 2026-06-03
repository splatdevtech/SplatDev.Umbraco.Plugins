using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace SplatDev.Umbraco.Themes.Blog.Composers;

public class BlogThemeComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, BlogThemeStartedHandler>();
    }
}

internal sealed class BlogThemeStartedHandler : INotificationAsyncHandler<UmbracoApplicationStartedNotification>
{
    private const string ThemeName = "blog";
    private const string EmbeddedYamlPath = "SplatDev.Umbraco.Themes.Blog.Config.umbraco.yml";

    private readonly ILogger<BlogThemeStartedHandler> _logger;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IRuntimeState _runtimeState;
    private readonly YamlParser _yamlParser;
    private readonly DataTypeCreator _dataTypeCreator;
    private readonly DocumentTypeCreator _documentTypeCreator;
    private readonly TemplateCreator _templateCreator;

    public BlogThemeStartedHandler(
        ILogger<BlogThemeStartedHandler> logger,
        IHostEnvironment hostEnvironment,
        IRuntimeState runtimeState,
        YamlParser yamlParser,
        DataTypeCreator dataTypeCreator,
        DocumentTypeCreator documentTypeCreator,
        TemplateCreator templateCreator)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _runtimeState = runtimeState;
        _yamlParser = yamlParser;
        _dataTypeCreator = dataTypeCreator;
        _documentTypeCreator = documentTypeCreator;
        _templateCreator = templateCreator;
    }

    public async Task HandleAsync(UmbracoApplicationStartedNotification notification, CancellationToken cancellationToken)
    {
        if (_runtimeState.Level != RuntimeLevel.Run)
        {
            _logger.LogDebug("Blog theme setup skipped – runtime level is {Level}", _runtimeState.Level);
            return;
        }

        var contentRoot = _hostEnvironment.ContentRootPath;
        var themeDir = Path.Combine(contentRoot, "config", "themes", ThemeName);
        var yamlDest = Path.Combine(themeDir, "umbraco.yml");
        var doneFile = Path.Combine(themeDir, "umbraco.done");

        if (File.Exists(doneFile))
        {
            _logger.LogDebug("Blog theme already installed (found {DoneFile})", doneFile);
            return;
        }

        _logger.LogInformation("Installing Blog theme schema...");

        try
        {
            Directory.CreateDirectory(themeDir);

            var assembly = typeof(BlogThemeComposer).Assembly;
            await using (var stream = assembly.GetManifestResourceStream(EmbeddedYamlPath)
                ?? throw new InvalidOperationException($"Embedded resource '{EmbeddedYamlPath}' not found."))
            await using (var fileStream = File.Create(yamlDest))
            {
                await stream.CopyToAsync(fileStream, cancellationToken);
            }

            var yamlRoot = _yamlParser.ParseYaml(yamlDest);
            var schema = yamlRoot.Umbraco;

            if (schema.DataTypes is { Count: > 0 })
            {
                _logger.LogInformation("Creating {Count} Blog data type(s)...", schema.DataTypes.Count);
                _dataTypeCreator.CreateDataTypes(schema.DataTypes);
            }

            if (schema.DocumentTypes is { Count: > 0 })
            {
                _logger.LogInformation("Creating {Count} Blog document type(s)...", schema.DocumentTypes.Count);
                _documentTypeCreator.CreateDocumentTypes(schema.DocumentTypes, schema.DataTypes);
            }

            if (schema.Templates is { Count: > 0 })
            {
                _logger.LogInformation("Creating {Count} Blog template(s)...", schema.Templates.Count);
                _templateCreator.CreateTemplates(schema.Templates);
            }

            await File.WriteAllTextAsync(doneFile, DateTime.UtcNow.ToString("O"), cancellationToken);
            _logger.LogInformation("Blog theme installed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blog theme installation failed.");
        }
    }
}
