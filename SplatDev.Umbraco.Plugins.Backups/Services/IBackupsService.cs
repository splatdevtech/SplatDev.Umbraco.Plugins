using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Models;

namespace SplatDev.Umbraco.Plugins.Backups.Services;

public interface IBackupsService
{
    Task<IEnumerable<BackupInfo>> ListBackupsAsync();
    Task<BackupInfo> CreateBackupAsync(BackupRequest request);
    Task<BackupResult> CreateBackupAsync(BackupOptions options, CancellationToken ct = default);
    Task<RestoreResult> RestoreBackupAsync(string backupPath, RestoreOptions options, CancellationToken ct = default);
    Task DeleteBackupAsync(string name);
    Task<IEnumerable<CloudProviderConfig>> GetCloudProvidersAsync();
    Task<bool> TestCloudProviderAsync(string providerId, CancellationToken ct = default);
}
