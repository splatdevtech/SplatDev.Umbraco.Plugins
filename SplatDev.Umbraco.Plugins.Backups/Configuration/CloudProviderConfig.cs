namespace SplatDev.Umbraco.Plugins.Backups.Configuration;

public class CloudProviderConfig
{
    public string Id { get; set; } = string.Empty;
    public string ProviderType { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public Dictionary<string, string> Settings { get; set; } = new();
}
