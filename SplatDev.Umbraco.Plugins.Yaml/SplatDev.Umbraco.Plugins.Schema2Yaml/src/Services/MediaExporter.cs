using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Configuration;
using SystemFile = System.IO.File;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Services;

/// <summary>
/// Exports Umbraco Media items to YAML format and downloads media files.
/// </summary>
public class MediaExporter
{
    private readonly IMediaService _mediaService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly Schema2YamlOptions _options;
    private readonly ILogger<MediaExporter> _logger;

    public MediaExporter(
        IMediaService mediaService,
        IWebHostEnvironment webHostEnvironment,
        IOptions<Schema2YamlOptions> options,
        ILogger<MediaExporter> logger)
    {
        _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
        _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Exports all Media items and their physical files.
    /// Returns a tuple of (export models, file dictionary).
    /// </summary>
    public async Task<(List<ExportMedia> Media, Dictionary<string, byte[]> Files)> ExportAsync()
    {
        if (!_options.IncludeMedia)
        {
            _logger.LogInformation("Media export is disabled in configuration");
            return ([], new Dictionary<string, byte[]>());
        }

        _logger.LogInformation("Starting Media export");

        var roots = _mediaService.GetRootMedia();
        var exported = new List<ExportMedia>();
        var files = new Dictionary<string, byte[]>();

        foreach (var root in roots)
        {
            await ExportMediaItemAsync(root, exported, files, string.Empty, 0);
        }

        _logger.LogInformation("Exported {Count} Media items with {FileCount} files", 
            exported.Count, files.Count);

        return (exported, files);
    }

    /// <summary>
    /// Exports media items that match the provided NodeIds filter.
    /// When IncludeAll is false and NodeIds is empty, returns empty collections.
    /// When a node's ID is in the list, it and all its descendants are exported fully.
    /// </summary>
    public virtual async Task<(List<ExportMedia> Media, Dictionary<string, byte[]> Files)> ExportAsync(CategorySelection filter)
    {
        if (!filter.IncludeAll && filter.NodeIds.Count == 0)
            return ([], new Dictionary<string, byte[]>());

        if (filter.IncludeAll)
            return await ExportAsync();

        _logger.LogInformation("Filtered media export: {Count} node IDs", filter.NodeIds.Count);

        var exported = new List<ExportMedia>();
        var files    = new Dictionary<string, byte[]>();
        foreach (var root in _mediaService.GetRootMedia())
            await CollectFilteredMediaAsync(root, filter.NodeIds, exported, files, string.Empty);

        return (exported, files);
    }

    private async Task CollectFilteredMediaAsync(
        IMedia node,
        List<int> nodeIds,
        List<ExportMedia> exported,
        Dictionary<string, byte[]> files,
        string folder)
    {
        if (nodeIds.Contains(node.Id))
        {
            await ExportMediaItemAsync(node, exported, files, folder, 0);
            return;
        }

        // Not selected — traverse children to find selected descendants
        foreach (var child in _mediaService.GetPagedChildren(node.Id, 0, int.MaxValue, out _))
            await CollectFilteredMediaAsync(child, nodeIds, exported, files, folder);
    }

    /// <summary>
    /// Recursively exports a media item and its children.
    /// </summary>
    private async Task ExportMediaItemAsync(
        IMedia media, 
        List<ExportMedia> exported, 
        Dictionary<string, byte[]> files, 
        string folder, 
        int depth)
    {
        if (depth >= _options.MaxHierarchyDepth)
        {
            _logger.LogWarning("Max hierarchy depth reached at media: {Name}", media.Name);
            return;
        }

        try
        {
            var export = new ExportMedia
            {
                Name = media.Name ?? string.Empty,
                MediaType = media.ContentType.Alias,
                Folder = folder,
                Properties = ExportProperties(media),
                Children = []
            };

            // Download file if exists
            if (media.HasProperty("umbracoFile"))
            {
                var fileValue = media.GetValue<string>("umbracoFile");
                if (!string.IsNullOrEmpty(fileValue))
                {
                    await DownloadMediaFileAsync(media, fileValue, folder, export, files);
                }
            }

            exported.Add(export);
            _logger.LogDebug("Exported Media: {Name} ({MediaType})", export.Name, export.MediaType);

            // Export children recursively
            var children = _mediaService.GetPagedChildren(media.Id, 0, int.MaxValue, out _);
            var childFolder = string.IsNullOrEmpty(folder) ? (media.Name ?? string.Empty) : $"{folder}/{media.Name ?? string.Empty}";

            foreach (var child in children.OrderBy(c => c.SortOrder))
            {
                await ExportMediaItemAsync(child, export.Children, files, childFolder, depth + 1);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export Media item: {Name}", media.Name);
        }
    }

    /// <summary>
    /// Downloads a media file and adds it to the files dictionary.
    /// </summary>
    private async Task DownloadMediaFileAsync(
        IMedia media, 
        string fileValue, 
        string folder, 
        ExportMedia export, 
        Dictionary<string, byte[]> files)
    {
        try
        {
            // Extract actual file path from JSON if needed (umbracoFile can be JSON for Image Cropper)
            var filePath = ExtractFilePath(fileValue);

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (SystemFile.Exists(physicalPath))
            {
                var fileBytes = await SystemFile.ReadAllBytesAsync(physicalPath);
                var fileName = Path.GetFileName(filePath);
                var fileKey = string.IsNullOrEmpty(folder) 
                    ? fileName 
                    : $"{folder}/{fileName}";

                files[fileKey] = fileBytes;
                export.Url = filePath;

                _logger.LogDebug("Downloaded media file: {FilePath} ({Size} bytes)", 
                    fileKey, fileBytes.Length);
            }
            else
            {
                _logger.LogWarning("Media file not found: {PhysicalPath}", physicalPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to download media file for: {Name}", media.Name);
        }
    }

    /// <summary>
    /// Extracts file path from umbracoFile value (handles both plain string and JSON).
    /// </summary>
    private string? ExtractFilePath(string fileValue)
    {
        if (string.IsNullOrWhiteSpace(fileValue))
        {
            return null;
        }

        // Check if it's JSON (Image Cropper format)
        if (fileValue.TrimStart().StartsWith("{"))
        {
            try
            {
                var json = System.Text.Json.JsonDocument.Parse(fileValue);
                if (json.RootElement.TryGetProperty("src", out var srcElement))
                {
                    return srcElement.GetString();
                }
            }
            catch
            {
                // Not valid JSON, treat as plain path
            }
        }

        // Plain file path
        return fileValue;
    }

    /// <summary>
    /// Exports all property values from a media item.
    /// </summary>
    private Dictionary<string, object> ExportProperties(IMedia media)
    {
        var properties = new Dictionary<string, object>();

        foreach (var property in media.Properties)
        {
            try
            {
                var value = property.GetValue();
                if (value != null)
                {
                    properties[property.Alias] = ConvertPropertyValue(value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to export property: {Alias} on {MediaName}", 
                    property.Alias, media.Name);
            }
        }

        return properties;
    }

    /// <summary>
    /// Converts property values to YAML-serializable format.
    /// </summary>
    private object ConvertPropertyValue(object value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        return value switch
        {
            string str => str,
            int or long or decimal or double or float => value,
            bool b => b,
            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
            _ => value.ToString() ?? string.Empty
        };
    }
}
