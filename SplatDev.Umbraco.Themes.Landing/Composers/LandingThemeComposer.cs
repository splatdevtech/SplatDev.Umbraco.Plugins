using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace SplatDev.Umbraco.Themes.Landing.Composers;

public class LandingThemeComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, LandingThemeStartedHandler>();
    }
}

internal sealed class LandingThemeStartedHandler : INotificationAsyncHandler<UmbracoApplicationStartedNotification>
{
    private const string YamlResourceName = "SplatDev.Umbraco.Themes.Landing.Config.umbraco.yml";
    private const string DoneFileName = "landing-theme.done";

    private readonly ILogger<LandingThemeStartedHandler> _logger;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IServiceProvider _serviceProvider;

    public LandingThemeStartedHandler(
        ILogger<LandingThemeStartedHandler> logger,
        IHostEnvironment hostEnvironment,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync(UmbracoApplicationStartedNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            var contentRoot = _hostEnvironment.ContentRootPath;
            var themeConfigDir = Path.Combine(contentRoot, "config", "themes", "landing");
            var doneFile = Path.Combine(themeConfigDir, DoneFileName);

            if (File.Exists(doneFile))
            {
                _logger.LogDebug("[LandingTheme] Schema already installed. Skipping.");
                return;
            }

            _logger.LogInformation("[LandingTheme] Installing landing theme schema...");

            Directory.CreateDirectory(themeConfigDir);

            var yamlPath = Path.Combine(themeConfigDir, "umbraco.yml");
            await ExtractEmbeddedYamlAsync(yamlPath, cancellationToken);

            await InstallSchemaAsync(yamlPath, cancellationToken);

            await File.WriteAllTextAsync(doneFile, DateTime.UtcNow.ToString("O"), cancellationToken);
            _logger.LogInformation("[LandingTheme] Landing theme schema installed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LandingTheme] Failed to install landing theme schema.");
        }
    }

    private async Task ExtractEmbeddedYamlAsync(string targetPath, CancellationToken cancellationToken)
    {
        var assembly = typeof(LandingThemeComposer).Assembly;
        await using var stream = assembly.GetManifestResourceStream(YamlResourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{YamlResourceName}' not found in assembly '{assembly.FullName}'.");

        await using var fileStream = File.Create(targetPath);
        await stream.CopyToAsync(fileStream, cancellationToken);

        _logger.LogDebug("[LandingTheme] Extracted embedded YAML to: {Path}", targetPath);
    }

    private async Task InstallSchemaAsync(string yamlPath, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var yamlParser = scope.ServiceProvider.GetRequiredService<IYamlParser>();
        var dataTypeCreator = scope.ServiceProvider.GetRequiredService<IDataTypeCreator>();
        var documentTypeCreator = scope.ServiceProvider.GetRequiredService<IDocumentTypeCreator>();
        var templateCreator = scope.ServiceProvider.GetRequiredService<ITemplateCreator>();

        var yamlContent = await File.ReadAllTextAsync(yamlPath, cancellationToken);
        var schema = yamlParser.Parse(yamlContent);

        if (schema.DataTypes?.Count > 0)
        {
            _logger.LogDebug("[LandingTheme] Creating {Count} data types...", schema.DataTypes.Count);
            foreach (var dataType in schema.DataTypes)
            {
                await dataTypeCreator.CreateOrUpdateAsync(dataType);
            }
        }

        if (schema.DocumentTypes?.Count > 0)
        {
            _logger.LogDebug("[LandingTheme] Creating {Count} document types...", schema.DocumentTypes.Count);
            foreach (var documentType in schema.DocumentTypes)
            {
                await documentTypeCreator.CreateOrUpdateAsync(documentType);
            }
        }

        if (schema.Templates?.Count > 0)
        {
            _logger.LogDebug("[LandingTheme] Creating {Count} templates...", schema.Templates.Count);
            foreach (var template in schema.Templates)
            {
                await templateCreator.CreateOrUpdateAsync(template);
            }
        }
    }
}
