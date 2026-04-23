namespace UmbracoCms.Plugins.Backups.Models;

public class BackupRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IncludeMedia { get; set; }
}
