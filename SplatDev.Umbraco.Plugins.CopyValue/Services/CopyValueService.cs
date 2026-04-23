using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.CopyValue.Models;

namespace SplatDev.Umbraco.Plugins.CopyValue.Services;

public class CopyValueService : ICopyValueService
{
    private readonly CopyValueDbContext _db;
    private readonly IContentService _contentService;
    private readonly ILogger<CopyValueService> _logger;

    public CopyValueService(
        CopyValueDbContext db,
        IContentService contentService,
        ILogger<CopyValueService> logger)
    {
        _db = db;
        _contentService = contentService;
        _logger = logger;
    }

    public async Task<IEnumerable<CopyMapping>> GetMappingsAsync()
    {
        return await _db.CopyMappings
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<CopyMapping?> GetMappingAsync(int id)
    {
        return await _db.CopyMappings.FindAsync(id);
    }

    public async Task<CopyMapping> SaveMappingAsync(CopyMapping mapping)
    {
        if (mapping.Id == 0)
        {
            mapping.CreatedAt = DateTime.UtcNow;
            _db.CopyMappings.Add(mapping);
        }
        else
        {
            var existing = await _db.CopyMappings.FindAsync(mapping.Id);
            if (existing is null)
                throw new KeyNotFoundException($"Mapping with id {mapping.Id} not found.");

            existing.Name = mapping.Name;
            existing.SourceDocTypeAlias = mapping.SourceDocTypeAlias;
            existing.TargetDocTypeAlias = mapping.TargetDocTypeAlias;
            existing.PropertyMappingsJson = mapping.PropertyMappingsJson;
            mapping = existing;
        }

        await _db.SaveChangesAsync();
        return mapping;
    }

    public async Task DeleteMappingAsync(int id)
    {
        var mapping = await _db.CopyMappings.FindAsync(id);
        if (mapping is not null)
        {
            _db.CopyMappings.Remove(mapping);
            await _db.SaveChangesAsync();
        }
    }

    public Task<bool> CopyPropertiesAsync(
        int sourceContentId,
        int targetContentId,
        IEnumerable<PropertyMapping> mappings,
        bool publish = false)
    {
        // 1. Load source content node
        var source = _contentService.GetById(sourceContentId);
        if (source is null)
        {
            _logger.LogWarning("CopyValue: Source content {Id} not found.", sourceContentId);
            return Task.FromResult(false);
        }

        // 2. Load target content node
        var target = _contentService.GetById(targetContentId);
        if (target is null)
        {
            _logger.LogWarning("CopyValue: Target content {Id} not found.", targetContentId);
            return Task.FromResult(false);
        }

        // 3. For each property mapping, get source value and set it on target
        foreach (var map in mappings)
        {
            var sourceValue = source.GetValue(map.Source);
            if (sourceValue is null)
            {
                _logger.LogDebug(
                    "CopyValue: Property '{Source}' on content {SourceId} has no value; skipping.",
                    map.Source, sourceContentId);
                continue;
            }

            target.SetValue(map.Target, sourceValue);
            _logger.LogDebug(
                "CopyValue: Copied '{Source}' → '{Target}' from content {SourceId} to {TargetId}.",
                map.Source, map.Target, sourceContentId, targetContentId);
        }

        // 4. Save and optionally publish the target
        if (publish)
        {
#if NET10_0_OR_GREATER
            _contentService.Save(target);
#else
            var result = _contentService.SaveAndPublish(target);
            if (!result.Success)
            {
                _logger.LogError(
                    "CopyValue: SaveAndPublish failed for content {TargetId}: {Status}",
                    targetContentId, result.Result);
                return Task.FromResult(false);
            }
#endif
        }
        else
        {
            _contentService.Save(target);
        }

        return Task.FromResult(true);
    }

    public async Task<int> BulkCopyAsync(
        int mappingId,
        IEnumerable<(int SourceId, int TargetId)> pairs,
        bool publish = false)
    {
        var mapping = await _db.CopyMappings.FindAsync(mappingId);
        if (mapping is null)
            throw new KeyNotFoundException($"Mapping with id {mappingId} not found.");

        var propertyMappings = JsonSerializer.Deserialize<List<PropertyMapping>>(
            mapping.PropertyMappingsJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new List<PropertyMapping>();

        int successCount = 0;
        foreach (var (sourceId, targetId) in pairs)
        {
            var ok = await CopyPropertiesAsync(sourceId, targetId, propertyMappings, publish);
            if (ok) successCount++;
        }

        _logger.LogInformation(
            "CopyValue: BulkCopy with mapping '{Name}' completed — {Count} nodes processed.",
            mapping.Name, successCount);

        return successCount;
    }
}
