using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Backups.Models;

namespace SplatDev.Umbraco.Plugins.Backups.Services;

public class BackupsService : IBackupsService
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IContentService _contentService;
    private readonly IMediaService _mediaService;

    private string BackupDirectory =>
        Path.Combine(_hostEnvironment.ContentRootPath, "App_Data", "Backups");

    public BackupsService(
        IHostEnvironment hostEnvironment,
        IContentService contentService,
        IMediaService mediaService)
    {
        _hostEnvironment = hostEnvironment;
        _contentService = contentService;
        _mediaService = mediaService;
    }

    public Task<IEnumerable<BackupInfo>> ListBackupsAsync()
    {
        EnsureDirectory();

        var files = Directory.GetFiles(BackupDirectory, "*.json")
            .Select(f =>
            {
                var fi = new FileInfo(f);
                return new BackupInfo
                {
                    Name = Path.GetFileNameWithoutExtension(fi.Name),
                    CreatedAt = fi.CreationTimeUtc,
                    SizeBytes = fi.Length
                };
            })
            .OrderByDescending(b => b.CreatedAt);

        return Task.FromResult<IEnumerable<BackupInfo>>(files);
    }

    public async Task<BackupInfo> CreateBackupAsync(BackupRequest request)
    {
        EnsureDirectory();

        var safeName = string.IsNullOrWhiteSpace(request.Name)
            ? $"backup-{DateTime.UtcNow:yyyyMMdd-HHmmss}"
            : request.Name.Replace(" ", "-");

        var payload = new
        {
            createdAt = DateTime.UtcNow,
            includeMedia = request.IncludeMedia,
            content = ExportContent(),
            media = request.IncludeMedia ? ExportMedia() : Array.Empty<object>()
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        var filePath = Path.Combine(BackupDirectory, $"{safeName}.json");
        await File.WriteAllTextAsync(filePath, json);

        var fi = new FileInfo(filePath);
        return new BackupInfo
        {
            Name = safeName,
            CreatedAt = fi.CreationTimeUtc,
            SizeBytes = fi.Length
        };
    }

    public Task DeleteBackupAsync(string name)
    {
        EnsureDirectory();

        var filePath = Path.Combine(BackupDirectory, $"{name}.json");
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Backup '{name}' not found.");

        File.Delete(filePath);
        return Task.CompletedTask;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void EnsureDirectory()
    {
        if (!Directory.Exists(BackupDirectory))
            Directory.CreateDirectory(BackupDirectory);
    }

    private IEnumerable<object> ExportContent()
    {
        return _contentService.GetRootContent()
            .Select(c => new
            {
                id = c.Id,
                key = c.Key,
                name = c.Name,
                contentType = c.ContentType.Alias,
                published = c.Published
            })
            .Cast<object>();
    }

    private IEnumerable<object> ExportMedia()
    {
        return _mediaService.GetRootMedia()
            .Select(m => new
            {
                id = m.Id,
                key = m.Key,
                name = m.Name,
                mediaType = m.ContentType.Alias
            })
            .Cast<object>();
    }
}
