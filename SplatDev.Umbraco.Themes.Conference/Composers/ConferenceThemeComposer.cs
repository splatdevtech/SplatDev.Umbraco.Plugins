using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace SplatDev.Umbraco.Themes.Conference.Composers;

public class ConferenceThemeComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, ConferenceThemeStartupHandler>();
    }
}

public class ConferenceThemeStartupHandler : INotificationAsyncHandler<UmbracoApplicationStartedNotification>
{
    private const string ThemeName = "conference";
    private const string EmbeddedYamlPath = "SplatDev.Umbraco.Themes.Conference.Config.umbraco.yml";

    private readonly ILogger<ConferenceThemeStartupHandler> _logger;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IRuntimeState _runtimeState;
    private readonly YamlParser _yamlParser;
    private readonly DataTypeCreator _dataTypeCreator;
    private readonly DocumentTypeCreator _documentTypeCreator;
    private readonly TemplateCreator _templateCreator;

    public ConferenceThemeStartupHandler(
        ILogger<ConferenceThemeStartupHandler> logger,
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
            _logger.LogDebug("Conference theme setup skipped – runtime level is {Level}", _runtimeState.Level);
            return;
        }

        var contentRoot = _hostEnvironment.ContentRootPath;
        var themeDir = Path.Combine(contentRoot, "config", "themes", ThemeName);
        var yamlDest = Path.Combine(themeDir, "umbraco.yml");
        var doneFile = Path.Combine(themeDir, "umbraco.done");

        if (File.Exists(doneFile))
        {
            _logger.LogDebug("Conference theme already installed (found {DoneFile})", doneFile);
            return;
        }

        _logger.LogInformation("Installing Conference theme schema...");

        try
        {
            Directory.CreateDirectory(themeDir);

            // Extract embedded YAML to disk
            var assembly = typeof(ConferenceThemeComposer).Assembly;
            await using (var stream = assembly.GetManifestResourceStream(EmbeddedYamlPath)
                ?? throw new InvalidOperationException($"Embedded resource '{EmbeddedYamlPath}' not found."))
            await using (var fileStream = File.Create(yamlDest))
            {
                await stream.CopyToAsync(fileStream, cancellationToken);
            }

            _logger.LogInformation("Conference theme YAML extracted to {Path}", yamlDest);

            // Parse YAML
            var schema = await _yamlParser.ParseAsync(yamlDest, cancellationToken);

            // Install data types
            if (schema.DataTypes is { Count: > 0 })
            {
                _logger.LogInformation("Creating {Count} Conference data type(s)...", schema.DataTypes.Count);
                await _dataTypeCreator.CreateOrUpdateAsync(schema.DataTypes, cancellationToken);
            }

            // Install document / element types
            if (schema.DocumentTypes is { Count: > 0 })
            {
                _logger.LogInformation("Creating {Count} Conference document type(s)...", schema.DocumentTypes.Count);
                await _documentTypeCreator.CreateOrUpdateAsync(schema.DocumentTypes, cancellationToken);
            }

            // Install templates
            if (schema.Templates is { Count: > 0 })
            {
                _logger.LogInformation("Creating {Count} Conference template(s)...", schema.Templates.Count);
                await _templateCreator.CreateOrUpdateAsync(schema.Templates, cancellationToken);
            }

            // Mark as done
            await File.WriteAllTextAsync(doneFile, DateTime.UtcNow.ToString("O"), cancellationToken);
            _logger.LogInformation("Conference theme installed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Conference theme installation failed.");
        }
    }
}
