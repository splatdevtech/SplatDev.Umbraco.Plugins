namespace SplatDev.Umbraco.Plugins.Backups.Scheduling;

public class BackupSchedule
{
    public string Id { get; set; } = string.Empty;
    public string CronExpression { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public Models.BackupOptions Options { get; set; } = new();
    public DateTime? LastRun { get; set; }
    public DateTime? NextRun { get; set; }
}
