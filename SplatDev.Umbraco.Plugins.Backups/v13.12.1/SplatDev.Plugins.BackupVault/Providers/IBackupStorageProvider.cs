namespace SplatDev.Plugins.BackupVault.Providers;

public interface IBackupStorageProvider
{
    string ProviderName { get; }
    Task<bool> UploadAsync(string localFilePath, string remoteFileName, CancellationToken ct = default);
    Task<IEnumerable<string>> ListAsync(string remoteFolder = "", CancellationToken ct = default);
    Task<bool> DeleteAsync(string remoteFileName, CancellationToken ct = default);
    Task<Stream> DownloadAsync(string remoteFileName, CancellationToken ct = default);
}
