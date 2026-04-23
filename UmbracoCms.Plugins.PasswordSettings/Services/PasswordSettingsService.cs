using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UmbracoCms.Plugins.PasswordSettings.Models;

namespace UmbracoCms.Plugins.PasswordSettings.Services;

public class PasswordSettingsService : IPasswordSettingsService
{
    private readonly PasswordSettingsDbContext _db;
    private readonly ILogger<PasswordSettingsService> _logger;

    public PasswordSettingsService(PasswordSettingsDbContext db, ILogger<PasswordSettingsService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PasswordPolicy?> GetPolicyAsync()
    {
        return await _db.PasswordPolicies.FirstOrDefaultAsync();
    }

    public async Task<PasswordPolicy> SavePolicyAsync(PasswordPolicy policy)
    {
        var existing = await _db.PasswordPolicies.FirstOrDefaultAsync();
        if (existing is null)
        {
            _db.PasswordPolicies.Add(policy);
        }
        else
        {
            existing.MinLength = policy.MinLength;
            existing.RequireUppercase = policy.RequireUppercase;
            existing.RequireDigit = policy.RequireDigit;
            existing.RequireSpecial = policy.RequireSpecial;
            existing.ExpirationDays = policy.ExpirationDays;
            existing.HistoryCount = policy.HistoryCount;
            policy = existing;
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Password policy saved.");
        return policy;
    }

    public async Task<(bool Valid, string[] Errors)> ValidatePasswordAsync(string password, PasswordPolicy policy)
    {
        var errors = new List<string>();
        if (password.Length < policy.MinLength) errors.Add($"Minimum {policy.MinLength} characters required.");
        if (policy.RequireUppercase && !password.Any(char.IsUpper)) errors.Add("At least one uppercase letter required.");
        if (policy.RequireDigit && !password.Any(char.IsDigit)) errors.Add("At least one digit required.");
        if (policy.RequireSpecial && !password.Any(c => "!@#$%^&*()_+-=[]{}|;':\",./<>?".Contains(c))) errors.Add("At least one special character required.");
        return await Task.FromResult((errors.Count == 0, errors.ToArray()));
    }

    public async Task RecordPasswordChangeAsync(int memberId, string passwordHash)
    {
        var history = new PasswordHistory
        {
            MemberId = memberId,
            PasswordHash = passwordHash,
            ChangedAt = DateTime.UtcNow
        };
        _db.PasswordHistories.Add(history);
        await _db.SaveChangesAsync();

        // Trim old entries beyond max history
        var policy = await GetPolicyAsync();
        if (policy is not null && policy.HistoryCount > 0)
        {
            var oldEntries = await _db.PasswordHistories
                .Where(h => h.MemberId == memberId)
                .OrderByDescending(h => h.ChangedAt)
                .Skip(policy.HistoryCount)
                .ToListAsync();

            if (oldEntries.Any())
            {
                _db.PasswordHistories.RemoveRange(oldEntries);
                await _db.SaveChangesAsync();
            }
        }
    }

    public async Task<bool> IsPasswordReusedAsync(int memberId, string passwordHash, int historyCount)
    {
        var recentHashes = await _db.PasswordHistories
            .Where(h => h.MemberId == memberId)
            .OrderByDescending(h => h.ChangedAt)
            .Take(historyCount)
            .Select(h => h.PasswordHash)
            .ToListAsync();

        return recentHashes.Contains(passwordHash);
    }
}
