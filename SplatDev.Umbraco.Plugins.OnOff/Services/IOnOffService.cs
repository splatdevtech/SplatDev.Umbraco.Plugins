using SplatDev.Umbraco.Plugins.OnOff.Models;

namespace SplatDev.Umbraco.Plugins.OnOff.Services;

public interface IOnOffService
{
    Task<IEnumerable<FeatureToggle>> GetAllFeaturesAsync();
    Task<FeatureToggle?> GetFeatureAsync(string alias);
    Task<FeatureToggle> EnableFeatureAsync(string alias);
    Task<FeatureToggle> DisableFeatureAsync(string alias);
    Task<FeatureToggle> ScheduleFeatureAsync(string alias, DateTime? enableAt, DateTime? disableAt);
    Task<FeatureToggle> UpsertFeatureAsync(FeatureToggle feature);
    Task DeleteFeatureAsync(int id);
    Task ApplyScheduledChangesAsync();
}
