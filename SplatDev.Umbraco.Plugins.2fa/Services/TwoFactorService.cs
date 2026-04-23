using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.TwoFactor.Models;

namespace SplatDev.Umbraco.Plugins.TwoFactor.Services;

public class TwoFactorService : ITwoFactorService
{
    private readonly TwoFactorDbContext _db;
    private readonly ILogger<TwoFactorService> _logger;

    public TwoFactorService(TwoFactorDbContext db, ILogger<TwoFactorService> logger)
    {
        _db = db;
        _logger = logger;
    }

    private static string GenerateTotpSecret() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(20));

    private static bool VerifyTotp(string secret, string code)
    {
        var key = Convert.FromBase64String(secret);
        var timeStep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
        for (long i = -1; i <= 1; i++)
        {
            var hmac = new HMACSHA1(key);
            var data = BitConverter.GetBytes(timeStep + i);
            if (BitConverter.IsLittleEndian) Array.Reverse(data);
            var hash = hmac.ComputeHash(data);
            var offset = hash[^1] & 0x0F;
            var otp = (((hash[offset] & 0x7F) << 24) | ((hash[offset + 1] & 0xFF) << 16) |
                       ((hash[offset + 2] & 0xFF) << 8) | (hash[offset + 3] & 0xFF)) % 1_000_000;
            if (otp.ToString("D6") == code) return true;
        }
        return false;
    }

    public async Task<TwoFactorSetup> SetupTotpAsync(int memberId)
    {
        var existing = await _db.TwoFactorSetups.FirstOrDefaultAsync(s => s.MemberId == memberId);
        if (existing is not null)
        {
            // Re-generate secret for fresh setup
            existing.SecretKey = GenerateTotpSecret();
            existing.IsEnabled = false;
            existing.CreatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return existing;
        }

        var setup = new TwoFactorSetup
        {
            MemberId = memberId,
            SecretKey = GenerateTotpSecret(),
            IsEnabled = false,
            CreatedAt = DateTime.UtcNow
        };
        _db.TwoFactorSetups.Add(setup);
        await _db.SaveChangesAsync();
        _logger.LogInformation("TOTP setup created for member {MemberId}", memberId);
        return setup;
    }

    public async Task<bool> VerifyTotpAsync(int memberId, string code)
    {
        var setup = await _db.TwoFactorSetups.FirstOrDefaultAsync(s => s.MemberId == memberId);
        if (setup is null) return false;

        var valid = VerifyTotp(setup.SecretKey, code);
        if (valid && !setup.IsEnabled)
        {
            setup.IsEnabled = true;
            await _db.SaveChangesAsync();
        }
        return valid;
    }

    public async Task<IEnumerable<string>> GenerateBackupCodesAsync(int memberId, int count = 8)
    {
        var setup = await _db.TwoFactorSetups
            .Include(s => s.BackupCodes)
            .FirstOrDefaultAsync(s => s.MemberId == memberId)
            ?? throw new InvalidOperationException($"No 2FA setup found for member {memberId}.");

        // Remove existing unused codes
        _db.BackupCodes.RemoveRange(setup.BackupCodes);

        var codes = new List<string>();
        for (int i = 0; i < count; i++)
        {
            var raw = Convert.ToHexString(RandomNumberGenerator.GetBytes(4)).ToLower();
            var code = $"{raw[..4]}-{raw[4..]}";
            codes.Add(code);
            setup.BackupCodes.Add(new BackupCode
            {
                MemberId = memberId,
                Code = code,
                IsUsed = false,
                TwoFactorSetupId = setup.Id
            });
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Generated {Count} backup codes for member {MemberId}", count, memberId);
        return codes;
    }

    public async Task<bool> UseBackupCodeAsync(int memberId, string code)
    {
        var backupCode = await _db.BackupCodes
            .FirstOrDefaultAsync(bc => bc.MemberId == memberId && bc.Code == code && !bc.IsUsed);

        if (backupCode is null) return false;

        backupCode.IsUsed = true;
        await _db.SaveChangesAsync();
        _logger.LogInformation("Backup code used for member {MemberId}", memberId);
        return true;
    }

    public async Task<bool> IsEnabledAsync(int memberId)
    {
        var setup = await _db.TwoFactorSetups.FirstOrDefaultAsync(s => s.MemberId == memberId);
        return setup?.IsEnabled ?? false;
    }

    public async Task DisableAsync(int memberId)
    {
        var setup = await _db.TwoFactorSetups
            .Include(s => s.BackupCodes)
            .FirstOrDefaultAsync(s => s.MemberId == memberId);

        if (setup is null) return;

        setup.IsEnabled = false;
        _db.BackupCodes.RemoveRange(setup.BackupCodes);
        await _db.SaveChangesAsync();
        _logger.LogInformation("2FA disabled for member {MemberId}", memberId);
    }
}
