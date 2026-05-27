using Microsoft.Extensions.Logging;
using System.Text.Json;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Migrations;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using Umbraco.Cms.Infrastructure.Scoping;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Services;

public interface IExportProfileService
{
    Task<List<ExportProfileSummary>> GetAllAsync();
    Task<ExportProfile?> GetActiveAsync();
    Task<ExportProfile> GetByIdAsync(int id);
    Task<ExportProfile> CreateAsync(string name, ExportSelection selection);
    Task<ExportProfile> UpdateAsync(int id, string name, ExportSelection selection);
    Task DeleteAsync(int id);
    Task ActivateAsync(int id);
    Task DeactivateAsync();
}

public class ExportProfileService : IExportProfileService
{
    private readonly IScopeProvider _scopeProvider;
    private readonly ILogger<ExportProfileService> _logger;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExportProfileService(IScopeProvider scopeProvider, ILogger<ExportProfileService> logger)
    {
        _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        _logger        = logger        ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<ExportProfileSummary>> GetAllAsync()
    {
        using var scope = _scopeProvider.CreateScope();
        var dtos = await scope.Database.FetchAsync<ExportProfileDto>(
            "SELECT id, name, isActive FROM schema2yamlExportProfiles ORDER BY name");
        scope.Complete();
        return dtos.Select(d => new ExportProfileSummary
            { Id = d.Id, Name = d.Name, IsActive = d.IsActive }).ToList();
    }

    public async Task<ExportProfile?> GetActiveAsync()
    {
        using var scope = _scopeProvider.CreateScope();
        var dtos = await scope.Database.FetchAsync<ExportProfileDto>(
            "SELECT * FROM schema2yamlExportProfiles WHERE isActive = 1");
        scope.Complete();
        return dtos.Count == 0 ? null : Map(dtos[0]);
    }

    public async Task<ExportProfile> GetByIdAsync(int id)
    {
        using var scope = _scopeProvider.CreateScope();
        var dto = await scope.Database.SingleOrDefaultAsync<ExportProfileDto>(
            "SELECT * FROM schema2yamlExportProfiles WHERE id = @0",
            new object[] { id },
            default);
        scope.Complete();
        if (dto is null) throw new KeyNotFoundException($"Export profile {id} not found");
        return Map(dto);
    }

    public async Task<ExportProfile> CreateAsync(string name, ExportSelection selection)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var now = DateTime.UtcNow;
        var dto = new ExportProfileDto
        {
            Name          = name,
            IsActive      = false,
            SelectionJson = JsonSerializer.Serialize(selection, _json),
            CreatedDate   = now,
            ModifiedDate  = now
        };
        using var scope = _scopeProvider.CreateScope();
        await scope.Database.InsertAsync(dto);
        scope.Complete();
        _logger.LogInformation("Created export profile: {Name} (id={Id})", name, dto.Id);
        return Map(dto);
    }

    public async Task<ExportProfile> UpdateAsync(int id, string name, ExportSelection selection)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        using var scope = _scopeProvider.CreateScope();
        var dto = await scope.Database.SingleOrDefaultAsync<ExportProfileDto>(
            "SELECT * FROM schema2yamlExportProfiles WHERE id = @0",
            new object[] { id },
            default)
            ?? throw new KeyNotFoundException($"Export profile {id} not found");
        dto.Name         = name;
        dto.SelectionJson = JsonSerializer.Serialize(selection, _json);
        dto.ModifiedDate = DateTime.UtcNow;
        await scope.Database.UpdateAsync(dto);
        scope.Complete();
        return Map(dto);
    }

    public async Task DeleteAsync(int id)
    {
        using var scope = _scopeProvider.CreateScope();
        var affected = await scope.Database.ExecuteAsync(
            "DELETE FROM schema2yamlExportProfiles WHERE id = @0",
            new object[] { id },
            default);
        if (affected == 0) throw new KeyNotFoundException($"Export profile {id} not found");
        scope.Complete();
        _logger.LogInformation("Deleted export profile id={Id}", id);
    }

    public async Task ActivateAsync(int id)
    {
        using var scope = _scopeProvider.CreateScope();
        // Single atomic statement — sets isActive=1 for the target row and isActive=0 for all others.
        // Avoids the race window of three separate UPDATEs.
        var affected = await scope.Database.ExecuteAsync(
            "UPDATE schema2yamlExportProfiles SET isActive = CASE WHEN id = @0 THEN 1 ELSE 0 END, modifiedDate = CASE WHEN id = @0 THEN @1 ELSE modifiedDate END",
            new object[] { id, DateTime.UtcNow },
            default);
        if (affected == 0) throw new KeyNotFoundException($"Export profile {id} not found");
        scope.Complete();
        _logger.LogInformation("Activated export profile id={Id}", id);
    }

    public async Task DeactivateAsync()
    {
        using var scope = _scopeProvider.CreateScope();
        await scope.Database.ExecuteAsync(
            "UPDATE schema2yamlExportProfiles SET isActive = 0");
        scope.Complete();
    }

    private static ExportProfile Map(ExportProfileDto dto) => new()
    {
        Id           = dto.Id,
        Name         = dto.Name,
        IsActive     = dto.IsActive,
        Selection    = JsonSerializer.Deserialize<ExportSelection>(dto.SelectionJson, _json) ?? new(),
        CreatedDate  = dto.CreatedDate,
        ModifiedDate = dto.ModifiedDate
    };
}
