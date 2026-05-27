using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Services;

/// <summary>
/// Exports Umbraco Languages to YAML format.
/// </summary>
public class LanguageExporter
{
    private readonly ILanguageService _languageService;
    private readonly ILogger<LanguageExporter> _logger;

    public LanguageExporter(
        ILanguageService languageService,
        ILogger<LanguageExporter> logger)
    {
        _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Exports all Languages from Umbraco.
    /// </summary>
    public async Task<List<ExportLanguage>> ExportAsync()
    {
        _logger.LogInformation("Starting Language export");

        var languages = await _languageService.GetAllAsync();
        var exported = new List<ExportLanguage>();

        foreach (var language in languages)
        {
            try
            {
                var export = new ExportLanguage
                {
                    IsoCode = language.IsoCode,
                    CultureName = language.CultureName,
                    IsDefault = language.IsDefault,
                    IsMandatory = language.IsMandatory
                };

                exported.Add(export);
                _logger.LogDebug("Exported Language: {CultureName} ({IsoCode})", export.CultureName, export.IsoCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export Language: {IsoCode}", language.IsoCode);
            }
        }

        _logger.LogInformation("Exported {Count} Languages", exported.Count);
        return exported;
    }

    /// <summary>
    /// Exports Languages filtered by a CategorySelection.
    /// </summary>
    public virtual async Task<List<ExportLanguage>> ExportAsync(CategorySelection filter)
    {
        if (!filter.IncludeAll && filter.Aliases.Count == 0)
            return [];
        var all = await ExportAsync();
        if (filter.IncludeAll) return all;
        return all.Where(x => filter.Aliases.Contains(x.IsoCode)).ToList();
    }
}
