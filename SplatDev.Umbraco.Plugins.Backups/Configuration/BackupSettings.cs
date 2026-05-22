namespace SplatDev.Umbraco.Plugins.Backups.Configuration;

public class BackupSettings
{
    public int LocalRetentionDays { get; set; } = 30;
    public int MaxLocalBackups { get; set; } = 10;
    public string BackupPath { get; set; } = "App_Data/Backups";
    public bool AutoCleanup { get; set; } = true;
    public List<CloudProviderConfig> CloudProviders { get; set; } = new();
}
