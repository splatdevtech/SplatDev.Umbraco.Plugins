using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UmbracoCms.Plugins.OnOff.Models;

namespace UmbracoCms.Plugins.OnOff.Services;

public class OnOffService : IOnOffService
{
    private readonly OnOffDbContext _db;
    private readonly ILogger<OnOffService> _logger;

    public OnOffService(OnOffDbContext db, ILogger<OnOffService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<FeatureToggle>> GetAllFeaturesAsync()
    {
        return await _db.FeatureToggles
            .OrderBy(f => f.Name)
            .ToListAsync();
    }

    public async Task<FeatureToggle?> GetFeatureAsync(string alias)
    {
        return await _db.FeatureToggles
            .FirstOrDefaultAsync(f => f.Alias == alias);
    }

    public async Task<FeatureToggle> EnableFeatureAsync(string alias)
    {
        var feature = await _db.FeatureToggles
            .FirstOrDefaultAsync(f => f.Alias == alias)
            ?? throw new KeyNotFoundException($"Feature '{alias}' not found.");

        feature.IsEnabled = true;
        feature.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return feature;
    }

    public async Task<FeatureToggle> DisableFeatureAsync(string alias)
    {
        var feature = await _db.FeatureToggles
            .FirstOrDefaultAsync(f => f.Alias == alias)
            ?? throw new KeyNotFoundException($"Feature '{alias}' not found.");

        feature.IsEnabled = false;
        feature.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return feature;
    }

    public async Task<FeatureToggle> ScheduleFeatureAsync(string alias, DateTime? enableAt, DateTime? disableAt)
    {
        var feature = await _db.FeatureToggles
            .FirstOrDefaultAsync(f => f.Alias == alias)
            ?? throw new KeyNotFoundException($"Feature '{alias}' not found.");

        feature.ScheduledEnableAt = enableAt;
        feature.ScheduledDisableAt = disableAt;
        feature.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return feature;
    }

    public async Task<FeatureToggle> UpsertFeatureAsync(FeatureToggle feature)
    {
        var existing = await _db.FeatureToggles
            .FirstOrDefaultAsync(f => f.Alias == feature.Alias);

        if (existing is null)
        {
            feature.UpdatedAt = DateTime.UtcNow;
            _db.FeatureToggles.Add(feature);
        }
        else
        {
            existing.Name = feature.Name;
            existing.Description = feature.Description;
            existing.IsEnabled = feature.IsEnabled;
            existing.ScheduledEnableAt = feature.ScheduledEnableAt;
            existing.ScheduledDisableAt = feature.ScheduledDisableAt;
            existing.UpdatedAt = DateTime.UtcNow;
            feature = existing;
        }

        await _db.SaveChangesAsync();
        return feature;
    }

    public async Task DeleteFeatureAsync(int id)
    {
        var feature = await _db.FeatureToggles.FindAsync(id);
        if (feature is not null)
        {
            _db.FeatureToggles.Remove(feature);
            await _db.SaveChangesAsync();
        }
    }

    public async Task ApplyScheduledChangesAsync()
    {
        var now = DateTime.UtcNow;

        var toEnable = await _db.FeatureToggles
            .Where(f => !f.IsEnabled && f.ScheduledEnableAt.HasValue && f.ScheduledEnableAt.Value <= now)
            .ToListAsync();

        foreach (var f in toEnable)
        {
            f.IsEnabled = true;
            f.ScheduledEnableAt = null;
            f.UpdatedAt = now;
            _logger.LogInformation("OnOff: Scheduled enable applied for feature '{Alias}'.", f.Alias);
        }

        var toDisable = await _db.FeatureToggles
            .Where(f => f.IsEnabled && f.ScheduledDisableAt.HasValue && f.ScheduledDisableAt.Value <= now)
            .ToListAsync();

        foreach (var f in toDisable)
        {
            f.IsEnabled = false;
            f.ScheduledDisableAt = null;
            f.UpdatedAt = now;
            _logger.LogInformation("OnOff: Scheduled disable applied for feature '{Alias}'.", f.Alias);
        }

        if (toEnable.Count > 0 || toDisable.Count > 0)
            await _db.SaveChangesAsync();
    }
}
