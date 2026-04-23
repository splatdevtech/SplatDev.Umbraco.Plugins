using UmbracoCms.Plugins.Backups.Models;

namespace UmbracoCms.Plugins.Backups.Services;

public interface IBackupsService
{
    Task<IEnumerable<BackupInfo>> ListBackupsAsync();
    Task<BackupInfo> CreateBackupAsync(BackupRequest request);
    Task DeleteBackupAsync(string name);
}
