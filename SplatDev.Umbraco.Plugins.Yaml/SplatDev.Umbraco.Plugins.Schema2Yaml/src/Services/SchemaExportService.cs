using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.IO.Compression;
using System.Text;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Configuration;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Services;

/// <summary>
/// Main orchestration service for exporting Umbraco schema to YAML.
/// </summary>
public interface ISchemaExportService
{
    Task<string> ExportToYamlAsync();
    Task<string> ExportToYamlAsync(ExportSelection selection);
    Task ExportToFileAsync(string filePath);
    Task<byte[]> ExportToZipAsync();
    Task<byte[]> ExportToZipAsync(ExportSelection selection);
    ExportStatistics GetLastExportStatistics();
}

/// <summary>
/// Main orchestration service for exporting Umbraco schema to YAML.
/// </summary>
public class SchemaExportService : ISchemaExportService
{
    private readonly LanguageExporter _languageExporter;
    private readonly DataTypeExporter _dataTypeExporter;
    private readonly DocumentTypeExporter _documentTypeExporter;
    private readonly MediaTypeExporter _mediaTypeExporter;
    private readonly TemplateExporter _templateExporter;
    private readonly MediaExporter _mediaExporter;
    private readonly ContentExporter _contentExporter;
    private readonly DictionaryExporter _dictionaryExporter;
    private readonly MemberExporter _memberExporter;
    private readonly UserExporter _userExporter;
    private readonly UmbracoVersionDetector _versionDetector;
    private readonly Schema2YamlOptions _options;
    private readonly ILogger<SchemaExportService> _logger;

    private ExportStatistics? _lastStatistics;

    public SchemaExportService(
        LanguageExporter languageExporter,
        DataTypeExporter dataTypeExporter,
        DocumentTypeExporter documentTypeExporter,
        MediaTypeExporter mediaTypeExporter,
        TemplateExporter templateExporter,
        MediaExporter mediaExporter,
        ContentExporter contentExporter,
        DictionaryExporter dictionaryExporter,
        MemberExporter memberExporter,
        UserExporter userExporter,
        UmbracoVersionDetector versionDetector,
        IOptions<Schema2YamlOptions> options,
        ILogger<SchemaExportService> logger)
    {
        _languageExporter = languageExporter ?? throw new ArgumentNullException(nameof(languageExporter));
        _dataTypeExporter = dataTypeExporter ?? throw new ArgumentNullException(nameof(dataTypeExporter));
        _documentTypeExporter = documentTypeExporter ?? throw new ArgumentNullException(nameof(documentTypeExporter));
        _mediaTypeExporter = mediaTypeExporter ?? throw new ArgumentNullException(nameof(mediaTypeExporter));
        _templateExporter = templateExporter ?? throw new ArgumentNullException(nameof(templateExporter));
        _mediaExporter = mediaExporter ?? throw new ArgumentNullException(nameof(mediaExporter));
        _contentExporter = contentExporter ?? throw new ArgumentNullException(nameof(contentExporter));
        _dictionaryExporter = dictionaryExporter ?? throw new ArgumentNullException(nameof(dictionaryExporter));
        _memberExporter = memberExporter ?? throw new ArgumentNullException(nameof(memberExporter));
        _userExporter = userExporter ?? throw new ArgumentNullException(nameof(userExporter));
        _versionDetector = versionDetector ?? throw new ArgumentNullException(nameof(versionDetector));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Exports the entire Umbraco schema to YAML string.
    /// </summary>
    public async Task<string> ExportToYamlAsync()
    {
        _logger.LogInformation("Starting full schema export (Umbraco {Version})", 
            _versionDetector.GetVersionString());

        var startTime = DateTime.UtcNow;

        try
        {
            // Execute all exporters in proper order
            var languages = await _languageExporter.ExportAsync();
            var dataTypes = await _dataTypeExporter.ExportAsync();
            var documentTypes = await _documentTypeExporter.ExportAsync();
            var mediaTypes = await _mediaTypeExporter.ExportAsync();
            var templates = await _templateExporter.ExportAsync();
            var (media, _) = await _mediaExporter.ExportAsync();
            var content = await _contentExporter.ExportAsync();
            var dictionary = await _dictionaryExporter.ExportAsync();
            var members = await _memberExporter.ExportAsync();
            var users = await _userExporter.ExportAsync();

            // Build export root
            var root = new ExportRoot
            {
                Umbraco = new UmbracoExport
                {
                    Languages = languages,
                    DataTypes = dataTypes,
                    DocumentTypes = documentTypes,
                    MediaTypes = mediaTypes,
                    Templates = templates,
                    Media = media,
                    Content = content,
                    DictionaryItems = dictionary,
                    Members = members,
                    Users = users
                }
            };

            // Update statistics
            _lastStatistics = new ExportStatistics
            {
                ExportDate = DateTime.UtcNow,
                UmbracoVersion = _versionDetector.GetVersionString(),
                LanguageCount = languages.Count,
                DataTypeCount = dataTypes.Count,
                DocumentTypeCount = documentTypes.Count,
                MediaTypeCount = mediaTypes.Count,
                TemplateCount = templates.Count,
                MediaCount = media.Count,
                ContentCount = content.Count,
                DictionaryItemCount = dictionary.Count,
                MemberCount = members.Count,
                UserCount = users.Count,
                Duration = DateTime.UtcNow - startTime
            };

            // Serialize to YAML
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var yaml = serializer.Serialize(root);

            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation(
                "Schema export completed in {Duration:N2}s: {DataTypes} data types, {DocumentTypes} document types, {Content} content nodes, {Media} media items",
                duration.TotalSeconds,
                dataTypes.Count,
                documentTypes.Count,
                content.Count,
                media.Count);

            return yaml;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Schema export failed");
            throw;
        }
    }

    /// <summary>
    /// Exports the Umbraco schema to YAML string, filtered by the provided selection.
    /// </summary>
    public async Task<string> ExportToYamlAsync(ExportSelection selection)
    {
        var (mediaItems, _) = await _mediaExporter.ExportAsync(selection.Media);
        return await BuildFilteredYamlAsync(selection, mediaItems);
    }

    private async Task<string> BuildFilteredYamlAsync(ExportSelection selection, List<ExportMedia> mediaItems)
    {
        _logger.LogInformation("Starting filtered schema export");
        var startTime = DateTime.UtcNow;

        var languages     = await _languageExporter.ExportAsync(selection.Languages);
        var dataTypes     = await _dataTypeExporter.ExportAsync(selection.DataTypes);
        var documentTypes = await _documentTypeExporter.ExportAsync(selection.DocumentTypes);
        var mediaTypes    = await _mediaTypeExporter.ExportAsync(selection.MediaTypes);
        var templates     = await _templateExporter.ExportAsync(selection.Templates);
        var media         = mediaItems;
        var content       = await _contentExporter.ExportAsync(selection.Content);
        var dictionary    = await _dictionaryExporter.ExportAsync(selection.DictionaryItems);
        var members       = await _memberExporter.ExportAsync(selection.Members);
        var users         = await _userExporter.ExportAsync(selection.Users);

        var root = new ExportRoot
        {
            Umbraco = new UmbracoExport
            {
                Languages = languages,
                DataTypes = dataTypes,
                DocumentTypes = documentTypes,
                MediaTypes = mediaTypes,
                Templates = templates,
                Media = media,
                Content = content,
                DictionaryItems = dictionary,
                Members = members,
                Users = users
            }
        };

        _lastStatistics = new ExportStatistics
        {
            ExportDate = DateTime.UtcNow,
            UmbracoVersion = _versionDetector.GetVersionString(),
            LanguageCount = languages.Count,
            DataTypeCount = dataTypes.Count,
            DocumentTypeCount = documentTypes.Count,
            MediaTypeCount = mediaTypes.Count,
            TemplateCount = templates.Count,
            MediaCount = media.Count,
            ContentCount = content.Count,
            DictionaryItemCount = dictionary.Count,
            MemberCount = members.Count,
            UserCount = users.Count,
            Duration = DateTime.UtcNow - startTime
        };

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return serializer.Serialize(root);
    }

    /// <summary>
    /// Exports schema and media files to a ZIP archive, filtered by the provided selection.
    /// </summary>
    public async Task<byte[]> ExportToZipAsync(ExportSelection selection)
    {
        // Fetch media once — items go into YAML, files go into the ZIP entries.
        var (mediaItems, mediaFiles) = await _mediaExporter.ExportAsync(selection.Media);
        var yaml = await BuildFilteredYamlAsync(selection, mediaItems);

        using var memStream = new MemoryStream();
        using (var archive = new ZipArchive(memStream, ZipArchiveMode.Create, true))
        {
            var yamlEntry = archive.CreateEntry("umbraco.yml", CompressionLevel.Optimal);
            using (var entryStream = yamlEntry.Open())
            using (var writer = new StreamWriter(entryStream, Encoding.UTF8))
            {
                await writer.WriteAsync(yaml);
            }

            foreach (var (path, bytes) in mediaFiles)
            {
                var mediaEntry = archive.CreateEntry($"media/{path}", CompressionLevel.Optimal);
                using var entryStream = mediaEntry.Open();
                await entryStream.WriteAsync(bytes);
            }
        }

        memStream.Position = 0;
        return memStream.ToArray();
    }

    /// <summary>
    /// Exports schema to a YAML file.
    /// </summary>
    public async Task ExportToFileAsync(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        _logger.LogInformation("Exporting schema to file: {FilePath}", filePath);

        var yaml = await ExportToYamlAsync();

        // Ensure directory exists
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(filePath, yaml, Encoding.UTF8);

        _logger.LogInformation("Schema exported to: {FilePath}", filePath);
    }

    /// <summary>
    /// Exports schema and media files to a ZIP archive.
    /// </summary>
    public async Task<byte[]> ExportToZipAsync()
    {
        _logger.LogInformation("Creating ZIP archive with schema and media files");

        var yaml = await ExportToYamlAsync();
        var (_, mediaFiles) = await _mediaExporter.ExportAsync();

        using var memStream = new MemoryStream();
        using (var archive = new ZipArchive(memStream, ZipArchiveMode.Create, true))
        {
            // Add YAML file
            var yamlEntry = archive.CreateEntry("umbraco.yml", CompressionLevel.Optimal);
            using (var entryStream = yamlEntry.Open())
            using (var writer = new StreamWriter(entryStream, Encoding.UTF8))
            {
                await writer.WriteAsync(yaml);
            }

            // Add media files
            foreach (var (path, bytes) in mediaFiles)
            {
                var mediaEntry = archive.CreateEntry($"media/{path}", CompressionLevel.Optimal);
                using var entryStream = mediaEntry.Open();
                await entryStream.WriteAsync(bytes);
            }

            _logger.LogInformation(
                "ZIP archive created: 1 YAML file + {FileCount} media files ({Size:N0} bytes total)",
                mediaFiles.Count,
                memStream.Length);
        }

        memStream.Position = 0;
        return memStream.ToArray();
    }

    /// <summary>
    /// Gets statistics from the last export operation.
    /// </summary>
    public ExportStatistics GetLastExportStatistics()
    {
        return _lastStatistics ?? new ExportStatistics();
    }
}

/// <summary>
/// Statistics about an export operation.
/// </summary>
public class ExportStatistics
{
    public DateTime ExportDate { get; set; }
    public string UmbracoVersion { get; set; } = string.Empty;
    public int LanguageCount { get; set; }
    public int DataTypeCount { get; set; }
    public int DocumentTypeCount { get; set; }
    public int MediaTypeCount { get; set; }
    public int TemplateCount { get; set; }
    public int MediaCount { get; set; }
    public int ContentCount { get; set; }
    public int DictionaryItemCount { get; set; }
    public int MemberCount { get; set; }
    public int UserCount { get; set; }
    public TimeSpan Duration { get; set; }
}
