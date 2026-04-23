using SplatDev.Umbraco.Plugins.CopyValue.Models;

namespace SplatDev.Umbraco.Plugins.CopyValue.Services;

public interface ICopyValueService
{
    Task<IEnumerable<CopyMapping>> GetMappingsAsync();
    Task<CopyMapping?> GetMappingAsync(int id);
    Task<CopyMapping> SaveMappingAsync(CopyMapping mapping);
    Task DeleteMappingAsync(int id);

    /// <summary>
    /// Copy properties from a source content node to a target content node using an explicit list of mappings.
    /// </summary>
    Task<bool> CopyPropertiesAsync(
        int sourceContentId,
        int targetContentId,
        IEnumerable<PropertyMapping> mappings,
        bool publish = false);

    /// <summary>
    /// Apply a saved mapping template to bulk-copy properties from multiple source nodes to their corresponding targets.
    /// </summary>
    Task<int> BulkCopyAsync(
        int mappingId,
        IEnumerable<(int SourceId, int TargetId)> pairs,
        bool publish = false);
}
