using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace SplatDev.Umbraco.Themes.Commerce.Composers;

public class CommerceThemeComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, CommerceThemeStartupHandler>();
    }
}

public class CommerceThemeStartupHandler : INotificationAsyncHandler<UmbracoApplicationStartedNotification>
{
    private const string ThemeName = "commerce";
    private const string EmbeddedYamlPath = "SplatDev.Umbraco.Themes.Commerce.Config.umbraco.yml";

    private readonly ILogger<CommerceThemeStartupHandler> _logger;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IRuntimeState _runtimeState;
    private readonly YamlParser _yamlParser;
    private readonly DataTypeCreator _dataTypeCreator;
    private readonly DocumentTypeCreator _documentTypeCreator;
    private readonly TemplateCreator _templateCreator;

    public CommerceThemeStartupHandler(
        ILogger<CommerceThemeStartupHandler> logger,
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
            _logger.LogDebug("Commerce theme setup skipped – runtime level is {Level}", _runtimeState.Level);
            return;
        }

        var contentRoot = _hostEnvironment.ContentRootPath;
        var themeDir = Path.Combine(contentRoot, "config", "themes", ThemeName);
        var yamlDest = Path.Combine(themeDir, "umbraco.yml");
        var doneFile = Path.Combine(themeDir, "umbraco.done");

        if (File.Exists(doneFile))
        {
            _logger.LogDebug("Commerce theme already installed (found {DoneFile})", doneFile);
            return;
        }

        _logger.LogInformation("Installing Commerce theme schema…");

        try
        {
            Directory.CreateDirectory(themeDir);

            // Extract embedded YAML to disk
            var assembly = typeof(CommerceThemeComposer).Assembly;
            await using (var stream = assembly.GetManifestResourceStream(EmbeddedYamlPath)
                ?? throw new InvalidOperationException($"Embedded resource '{EmbeddedYamlPath}' not found."))
            await using (var fileStream = File.Create(yamlDest))
            {
                await stream.CopyToAsync(fileStream, cancellationToken);
            }

            _logger.LogInformation("Commerce theme YAML extracted to {Path}", yamlDest);

            // Parse YAML
            var yamlRoot = _yamlParser.ParseYaml(yamlDest);
            var schema = yamlRoot.Umbraco;

            // Install data types
            if (schema.DataTypes is { Count: > 0 })
            {
                _logger.LogInformation("Creating {Count} Commerce data type(s)…", schema.DataTypes.Count);
                _dataTypeCreator.CreateDataTypes(schema.DataTypes);
            }

            // Install document / element types
            if (schema.DocumentTypes is { Count: > 0 })
            {
                _logger.LogInformation("Creating {Count} Commerce document type(s)…", schema.DocumentTypes.Count);
                _documentTypeCreator.CreateDocumentTypes(schema.DocumentTypes, schema.DataTypes);
            }

            // Install templates
            if (schema.Templates is { Count: > 0 })
            {
                _logger.LogInformation("Creating {Count} Commerce template(s)…", schema.Templates.Count);
                _templateCreator.CreateTemplates(schema.Templates);
            }

            // Mark as done
            await File.WriteAllTextAsync(doneFile, DateTime.UtcNow.ToString("O"), cancellationToken);
            _logger.LogInformation("Commerce theme installed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Commerce theme installation failed.");
        }
    }
}
