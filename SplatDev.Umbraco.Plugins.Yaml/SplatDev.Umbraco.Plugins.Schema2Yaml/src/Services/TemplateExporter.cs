using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Services;

/// <summary>
/// Exports Umbraco Templates to YAML format.
/// </summary>
public class TemplateExporter
{
    private readonly ITemplateService _templateService;
    private readonly ILogger<TemplateExporter> _logger;

    public TemplateExporter(
        ITemplateService templateService,
        ILogger<TemplateExporter> logger)
    {
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Exports all Templates from Umbraco.
    /// </summary>
    public async Task<List<ExportTemplate>> ExportAsync()
    {
        _logger.LogInformation("Starting Template export");

        var templates = await _templateService.GetAllAsync(Array.Empty<string>());
        var exported = new List<ExportTemplate>();

        foreach (var template in templates)
        {
            try
            {
                var export = new ExportTemplate
                {
                    Alias = template.Alias,
                    Name = template.Name ?? string.Empty,
                    MasterTemplate = template.MasterTemplateAlias,
                    Content = template.Content
                };

                exported.Add(export);
                _logger.LogDebug("Exported Template: {Name} ({Alias})", export.Name, export.Alias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export Template: {Name}", template.Name);
            }
        }

        _logger.LogInformation("Exported {Count} Templates", exported.Count);
        return exported;
    }

    /// <summary>
    /// Exports Templates filtered by a CategorySelection.
    /// </summary>
    public virtual async Task<List<ExportTemplate>> ExportAsync(CategorySelection filter)
    {
        if (!filter.IncludeAll && filter.Aliases.Count == 0)
            return [];
        var all = await ExportAsync();
        if (filter.IncludeAll) return all;
        return all.Where(x => filter.Aliases.Contains(x.Alias)).ToList();
    }
}
