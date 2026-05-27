namespace SplatDev.Umbraco.Plugins.Backups.Providers;

using Dropbox.Api;
using Dropbox.Api.Files;
using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Models;

public class DropboxStorageProvider : ICloudStorageProvider
{
    private readonly BackupSettings _settings;

    public string ProviderName => "Dropbox";
    public string ProviderIcon => "dropbox";
    public bool RequiresOAuth => true;
    public bool RequiresApiKey => false;

    public DropboxStorageProvider(BackupSettings settings)
    {
        _settings = settings;
    }

    private (string AccessToken, string RootPath) GetConfig()
    {
        var cfg = _settings.CloudProviders
            .FirstOrDefault(c => c.ProviderType == ProviderName && c.Enabled);
        if (cfg is null)
            throw new InvalidOperationException("Dropbox provider not configured.");

        var token = cfg.Settings.GetValueOrDefault("AccessToken") ?? string.Empty;
        var root = cfg.Settings.GetValueOrDefault("RootPath") ?? "/Backups";
        if (!root.StartsWith('/'))
            root = "/" + root;
        return (token, root);
    }

    public async Task UploadAsync(Stream data, string remotePath, string fileName, CancellationToken ct)
    {
        var (token, root) = GetConfig();
        using var client = new DropboxClient(token);

        var fullPath = $"{root}/{remotePath}/{fileName}";
        var commitInfo = new CommitInfo(fullPath, WriteMode.Overwrite.Instance, false, null, false, null, false);
        await client.Files.UploadAsync(commitInfo, data);
    }

    public async Task<Stream> DownloadAsync(string remotePath, CancellationToken ct)
    {
        var (token, _) = GetConfig();
        using var client = new DropboxClient(token);

        using var response = await client.Files.DownloadAsync(remotePath, null);
        var ms = new MemoryStream();
        using var content = await response.GetContentAsStreamAsync();
        await content.CopyToAsync(ms, ct);
        ms.Position = 0;
        return ms;
    }

    public async Task DeleteAsync(string remotePath, CancellationToken ct)
    {
        var (token, _) = GetConfig();
        using var client = new DropboxClient(token);
        await client.Files.DeleteV2Async(remotePath, null);
    }

    public async Task<bool> ValidateConnectionAsync(CancellationToken ct)
    {
        try
        {
            var (token, _) = GetConfig();
            using var client = new DropboxClient(token);
            await client.Users.GetCurrentAccountAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<StorageItem>> ListAsync(string remotePath, CancellationToken ct)
    {
        var (token, root) = GetConfig();
        try
        {
            using var client = new DropboxClient(token);
            var fullPath = $"{root}/{remotePath}";

            var result = await client.Files.ListFolderAsync(
                fullPath, false, false, false, false, false, null, null, null, false);

            return result.Entries
                .OfType<FileMetadata>()
                .Select(f => new StorageItem
                {
                    Name = f.Name,
                    Path = f.PathLower ?? f.PathDisplay ?? f.Name,
                    SizeBytes = (long)f.Size,
                    LastModified = f.ClientModified,
                    IsDirectory = false
                }).ToList();
        }
        catch
        {
            return [];
        }
    }
}
