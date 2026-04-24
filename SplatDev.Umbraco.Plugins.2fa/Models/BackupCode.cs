namespace SplatDev.Umbraco.Plugins.TwoFactor.Models;

public class BackupCode
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsUsed { get; set; } = false;

    public int TwoFactorSetupId { get; set; }
    public TwoFactorSetup? Setup { get; set; }
}
