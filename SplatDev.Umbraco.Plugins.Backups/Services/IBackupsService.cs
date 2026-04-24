using SplatDev.Umbraco.Plugins.Backups.Models;

namespace SplatDev.Umbraco.Plugins.Backups.Services;

public interface IBackupsService
{
    Task<IEnumerable<BackupInfo>> ListBackupsAsync();
    Task<BackupInfo> CreateBackupAsync(BackupRequest request);
    Task DeleteBackupAsync(string name);
}
