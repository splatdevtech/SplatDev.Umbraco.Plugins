using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace SplatDev.Umbraco.Themes.Forum.Composers;

public class ForumThemeComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddNotificationHandler<UmbracoApplicationStartedNotification, ForumThemeStartupHandler>();
    }
}

internal sealed class ForumThemeStartupHandler : INotificationHandler<UmbracoApplicationStartedNotification>
{
    private const string EmbeddedResourceName = "SplatDev.Umbraco.Themes.Forum.Config.umbraco.yml";
    private const string RelativeConfigPath   = "config/themes/forum/umbraco.yml";
    private const string DoneFileName         = "config/themes/forum/.import.done";

    private readonly ILogger<ForumThemeStartupHandler> _logger;
    private readonly IHostEnvironment                  _hostEnvironment;
    private readonly YamlParser                        _yamlParser;
    private readonly DataTypeCreator                   _dataTypeCreator;
    private readonly DocumentTypeCreator               _documentTypeCreator;
    private readonly TemplateCreator                   _templateCreator;
    private readonly IRuntimeState                     _runtimeState;

    public ForumThemeStartupHandler(
        ILogger<ForumThemeStartupHandler> logger,
        IHostEnvironment                  hostEnvironment,
        YamlParser                        yamlParser,
        DataTypeCreator                   dataTypeCreator,
        DocumentTypeCreator               documentTypeCreator,
        TemplateCreator                   templateCreator,
        IRuntimeState                     runtimeState)
    {
        _logger              = logger;
        _hostEnvironment     = hostEnvironment;
        _yamlParser          = yamlParser;
        _dataTypeCreator     = dataTypeCreator;
        _documentTypeCreator = documentTypeCreator;
        _templateCreator     = templateCreator;
        _runtimeState        = runtimeState;
    }

    public void Handle(UmbracoApplicationStartedNotification notification)
    {
        if (_runtimeState.Level != RuntimeLevel.Run)
        {
            _logger.LogDebug("[ForumTheme] Runtime level is {Level} – skipping schema import.", _runtimeState.Level);
            return;
        }

        var contentRoot = _hostEnvironment.ContentRootPath;
        var doneFile    = Path.Combine(contentRoot, DoneFileName);

        if (File.Exists(doneFile))
        {
            _logger.LogDebug("[ForumTheme] Schema already imported (found {DoneFile}).", doneFile);
            return;
        }

        var yamlPath = Path.Combine(contentRoot, RelativeConfigPath);

        try
        {
            ExtractEmbeddedYaml(yamlPath);
            RunImport(yamlPath);
            MarkDone(doneFile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ForumTheme] Schema import failed.");
        }
    }

    private void ExtractEmbeddedYaml(string targetPath)
    {
        var directory = Path.GetDirectoryName(targetPath)!;
        Directory.CreateDirectory(directory);

        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(EmbeddedResourceName)
            ?? throw new InvalidOperationException(
                $"Embedded resource '{EmbeddedResourceName}' not found.");

        using var fileStream = File.Create(targetPath);
        stream.CopyTo(fileStream);

        _logger.LogInformation("[ForumTheme] Extracted umbraco.yml to {Path}.", targetPath);
    }

    private void RunImport(string yamlPath)
    {
        _logger.LogInformation("[ForumTheme] Starting schema import from {Path}.", yamlPath);

        var yamlRoot = _yamlParser.ParseYaml(yamlPath);
        var schema = yamlRoot.Umbraco;

        if (schema.DataTypes is { Count: > 0 })
            _dataTypeCreator.CreateDataTypes(schema.DataTypes);

        if (schema.DocumentTypes is { Count: > 0 })
            _documentTypeCreator.CreateDocumentTypes(schema.DocumentTypes, schema.DataTypes);

        if (schema.Templates is { Count: > 0 })
            _templateCreator.CreateTemplates(schema.Templates);

        _logger.LogInformation("[ForumTheme] Schema import completed successfully.");
    }

    private void MarkDone(string doneFile)
    {
        var directory = Path.GetDirectoryName(doneFile)!;
        Directory.CreateDirectory(directory);
        File.WriteAllText(doneFile, DateTimeOffset.UtcNow.ToString("o"));
        _logger.LogDebug("[ForumTheme] Created done file at {DoneFile}.", doneFile);
    }
}
