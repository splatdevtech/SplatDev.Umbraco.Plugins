namespace UmbracoCms.Plugins.TwoFactor.Models;

public class TwoFactorSetup
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public string SecretKey { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<BackupCode> BackupCodes { get; set; } = new List<BackupCode>();
}
