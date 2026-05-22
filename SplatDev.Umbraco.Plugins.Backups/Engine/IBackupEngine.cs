namespace SplatDev.Umbraco.Plugins.Backups.Engine;

using SplatDev.Umbraco.Plugins.Backups.Models;

public interface IBackupEngine
{
    Task<BackupResult> CreateFullBackupAsync(BackupOptions options, CancellationToken ct = default);
    Task<BackupResult> CreateContentBackupAsync(BackupOptions options, CancellationToken ct = default);
    Task<RestoreResult> RestoreAsync(string backupPath, RestoreOptions options, CancellationToken ct = default);
}
