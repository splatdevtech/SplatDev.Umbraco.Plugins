using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace SplatDev.Umbraco.Themes.Portfolio.Composers;

public class PortfolioThemeComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddNotificationHandler<UmbracoApplicationStartedNotification, PortfolioThemeStartupHandler>();
    }
}

internal sealed class PortfolioThemeStartupHandler : INotificationHandler<UmbracoApplicationStartedNotification>
{
    private const string EmbeddedResourceName = "SplatDev.Umbraco.Themes.Portfolio.Config.umbraco.yml";
    private const string RelativeConfigPath   = "config/themes/portfolio/umbraco.yml";
    private const string DoneFileName         = "config/themes/portfolio/.import.done";

    private readonly ILogger<PortfolioThemeStartupHandler> _logger;
    private readonly IHostEnvironment                      _hostEnvironment;
    private readonly YamlParser                            _yamlParser;
    private readonly DataTypeCreator                       _dataTypeCreator;
    private readonly DocumentTypeCreator                   _documentTypeCreator;
    private readonly TemplateCreator                       _templateCreator;
    private readonly IRuntimeState                         _runtimeState;

    public PortfolioThemeStartupHandler(
        ILogger<PortfolioThemeStartupHandler> logger,
        IHostEnvironment                      hostEnvironment,
        YamlParser                            yamlParser,
        DataTypeCreator                       dataTypeCreator,
        DocumentTypeCreator                   documentTypeCreator,
        TemplateCreator                       templateCreator,
        IRuntimeState                         runtimeState)
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
            _logger.LogDebug("[PortfolioTheme] Runtime level is {Level} – skipping schema import.", _runtimeState.Level);
            return;
        }

        var contentRoot = _hostEnvironment.ContentRootPath;
        var doneFile    = Path.Combine(contentRoot, DoneFileName);

        if (File.Exists(doneFile))
        {
            _logger.LogDebug("[PortfolioTheme] Schema already imported (found {DoneFile}).", doneFile);
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
            _logger.LogError(ex, "[PortfolioTheme] Schema import failed.");
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

        _logger.LogInformation("[PortfolioTheme] Extracted umbraco.yml to {Path}.", targetPath);
    }

    private void RunImport(string yamlPath)
    {
        _logger.LogInformation("[PortfolioTheme] Starting schema import from {Path}.", yamlPath);

        var schema = _yamlParser.ParseFile(yamlPath);

        foreach (var dataType in schema.DataTypes ?? [])
            _dataTypeCreator.CreateOrUpdate(dataType);

        foreach (var documentType in schema.DocumentTypes ?? [])
            _documentTypeCreator.CreateOrUpdate(documentType);

        foreach (var template in schema.Templates ?? [])
            _templateCreator.CreateOrUpdate(template);

        _logger.LogInformation("[PortfolioTheme] Schema import completed successfully.");
    }

    private void MarkDone(string doneFile)
    {
        var directory = Path.GetDirectoryName(doneFile)!;
        Directory.CreateDirectory(directory);
        File.WriteAllText(doneFile, DateTimeOffset.UtcNow.ToString("o"));
        _logger.LogDebug("[PortfolioTheme] Created done file at {DoneFile}.", doneFile);
    }
}
