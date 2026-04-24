using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using Umbraco.Cms.Core.Composing;
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
    private const string YamlResourceName = "SplatDev.Umbraco.Themes.Blog.Config.umbraco.yml";
    private const string DoneFileName = "blog-theme.done";

    private readonly ILogger<BlogThemeStartedHandler> _logger;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IServiceProvider _serviceProvider;

    public BlogThemeStartedHandler(
        ILogger<BlogThemeStartedHandler> logger,
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
            var themeConfigDir = Path.Combine(contentRoot, "config", "themes", "blog");
            var doneFile = Path.Combine(themeConfigDir, DoneFileName);

            if (File.Exists(doneFile))
            {
                _logger.LogDebug("[BlogTheme] Schema already installed. Skipping.");
                return;
            }

            _logger.LogInformation("[BlogTheme] Installing blog theme schema...");

            Directory.CreateDirectory(themeConfigDir);

            var yamlPath = Path.Combine(themeConfigDir, "umbraco.yml");
            await ExtractEmbeddedYamlAsync(yamlPath, cancellationToken);

            await InstallSchemaAsync(yamlPath, cancellationToken);

            await File.WriteAllTextAsync(doneFile, DateTime.UtcNow.ToString("O"), cancellationToken);
            _logger.LogInformation("[BlogTheme] Blog theme schema installed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BlogTheme] Failed to install blog theme schema.");
        }
    }

    private async Task ExtractEmbeddedYamlAsync(string targetPath, CancellationToken cancellationToken)
    {
        var assembly = typeof(BlogThemeComposer).Assembly;
        await using var stream = assembly.GetManifestResourceStream(YamlResourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{YamlResourceName}' not found in assembly '{assembly.FullName}'.");

        await using var fileStream = File.Create(targetPath);
        await stream.CopyToAsync(fileStream, cancellationToken);

        _logger.LogDebug("[BlogTheme] Extracted embedded YAML to: {Path}", targetPath);
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
            _logger.LogDebug("[BlogTheme] Creating {Count} data types...", schema.DataTypes.Count);
            foreach (var dataType in schema.DataTypes)
            {
                await dataTypeCreator.CreateOrUpdateAsync(dataType);
            }
        }

        if (schema.DocumentTypes?.Count > 0)
        {
            _logger.LogDebug("[BlogTheme] Creating {Count} document types...", schema.DocumentTypes.Count);
            foreach (var documentType in schema.DocumentTypes)
            {
                await documentTypeCreator.CreateOrUpdateAsync(documentType);
            }
        }

        if (schema.Templates?.Count > 0)
        {
            _logger.LogDebug("[BlogTheme] Creating {Count} templates...", schema.Templates.Count);
            foreach (var template in schema.Templates)
            {
                await templateCreator.CreateOrUpdateAsync(template);
            }
        }
    }
}
