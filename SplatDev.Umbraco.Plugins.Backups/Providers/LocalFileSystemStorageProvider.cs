namespace SplatDev.Umbraco.Plugins.Backups.Providers;

using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Models;

public class LocalFileSystemStorageProvider : ICloudStorageProvider
{
    private readonly BackupSettings _settings;

    public string ProviderName => "LocalFileSystem";
    public string ProviderIcon => "folder";
    public bool RequiresOAuth => false;
    public bool RequiresApiKey => false;

    public LocalFileSystemStorageProvider(BackupSettings settings)
    {
        _settings = settings;
    }

    public Task UploadAsync(Stream data, string remotePath, string fileName, CancellationToken ct)
    {
        var dir = Path.Combine(_settings.BackupPath, remotePath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var filePath = Path.Combine(dir, fileName);
        using var fileStream = File.Create(filePath);
        return data.CopyToAsync(fileStream, ct);
    }

    public Task<Stream> DownloadAsync(string remotePath, CancellationToken ct)
    {
        if (!File.Exists(remotePath))
            throw new FileNotFoundException($"File not found: {remotePath}");

        var stream = File.OpenRead(remotePath);
        return Task.FromResult<Stream>(stream);
    }

    public Task DeleteAsync(string remotePath, CancellationToken ct)
    {
        if (File.Exists(remotePath))
            File.Delete(remotePath);

        return Task.CompletedTask;
    }

    public Task<bool> ValidateConnectionAsync(CancellationToken ct)
    {
        var dir = _settings.BackupPath;
        if (!Directory.Exists(dir))
        {
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        var testFile = Path.Combine(dir, ".connection_test");
        try
        {
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<IEnumerable<StorageItem>> ListAsync(string remotePath, CancellationToken ct)
    {
        var dir = Path.Combine(_settings.BackupPath, remotePath);
        if (!Directory.Exists(dir))
            return Task.FromResult(Enumerable.Empty<StorageItem>());

        var items = new DirectoryInfo(dir).GetFileSystemInfos()
            .Select(fsi => new StorageItem
            {
                Name = fsi.Name,
                Path = fsi.FullName,
                SizeBytes = fsi is FileInfo fi ? fi.Length : 0,
                LastModified = fsi.LastWriteTimeUtc,
                IsDirectory = fsi is DirectoryInfo
            })
            .OrderByDescending(i => i.LastModified);

        return Task.FromResult<IEnumerable<StorageItem>>(items);
    }
}
