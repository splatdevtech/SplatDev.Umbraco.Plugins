namespace SplatDev.Umbraco.Plugins.Backups.Providers;

using SplatDev.Umbraco.Plugins.Backups.Models;

public interface ICloudStorageProvider
{
    string ProviderName { get; }
    string ProviderIcon { get; }
    bool RequiresOAuth { get; }
    bool RequiresApiKey { get; }
    Task UploadAsync(Stream data, string remotePath, string fileName, CancellationToken ct);
    Task<Stream> DownloadAsync(string remotePath, CancellationToken ct);
    Task DeleteAsync(string remotePath, CancellationToken ct);
    Task<bool> ValidateConnectionAsync(CancellationToken ct);
    Task<IEnumerable<StorageItem>> ListAsync(string remotePath, CancellationToken ct);
}
