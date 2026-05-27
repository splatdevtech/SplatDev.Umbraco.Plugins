using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Web.Common.Authorization;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using System.Text;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Controllers;

/// <summary>
/// API controller for Schema2Yaml dashboard operations.
/// </summary>
[ApiController]
[Route("umbraco/api/[controller]/[action]")]
[Authorize(Policy = AuthorizationPolicies.SectionAccessSettings)]
public class SchemaExportController : ControllerBase
{
    private readonly ISchemaExportService _exportService;
    private readonly ILogger<SchemaExportController> _logger;

    public SchemaExportController(
        ISchemaExportService exportService,
        ILogger<SchemaExportController> logger)
    {
        _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Exports Umbraco schema to YAML and returns it with statistics.
    /// GET: /umbraco/api/schemaexport/export
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Export()
    {
        try
        {
            _logger.LogInformation("Dashboard: Export YAML requested");

            var yaml = await _exportService.ExportToYamlAsync();
            var stats = _exportService.GetLastExportStatistics();

            return Ok(new
            {
                yaml,
                statistics = new
                {
                    exportDate = stats.ExportDate,
                    umbracoVersion = stats.UmbracoVersion,
                    languages = stats.LanguageCount,
                    dataTypes = stats.DataTypeCount,
                    documentTypes = stats.DocumentTypeCount,
                    mediaTypes = stats.MediaTypeCount,
                    templates = stats.TemplateCount,
                    media = stats.MediaCount,
                    content = stats.ContentCount,
                    dictionaryItems = stats.DictionaryItemCount,
                    members = stats.MemberCount,
                    users = stats.UserCount,
                    durationSeconds = stats.Duration.TotalSeconds
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export YAML");
            return StatusCode(500, new { error = "Export failed", message = ex.Message });
        }
    }

    /// <summary>
    /// Downloads the exported YAML as a file.
    /// GET: /umbraco/api/schemaexport/downloadyaml
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> DownloadYaml()
    {
        try
        {
            _logger.LogInformation("Dashboard: Download YAML requested");

            var yaml = await _exportService.ExportToYamlAsync();
            var bytes = Encoding.UTF8.GetBytes(yaml);

            return File(bytes, "application/x-yaml", $"umbraco-{DateTime.UtcNow:yyyyMMdd-HHmmss}.yml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download YAML");
            return StatusCode(500, new { error = "Download failed", message = ex.Message });
        }
    }

    /// <summary>
    /// Downloads the exported schema with media files as a ZIP archive.
    /// GET: /umbraco/api/schemaexport/downloadzip
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> DownloadZip()
    {
        try
        {
            _logger.LogInformation("Dashboard: Download ZIP requested");

            var zipBytes = await _exportService.ExportToZipAsync();

            return File(zipBytes, "application/zip", $"umbraco-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create ZIP");
            return StatusCode(500, new { error = "ZIP creation failed", message = ex.Message });
        }
    }

    // POST /umbraco/api/schemaexport/exportselected
    [HttpPost]
    public async Task<IActionResult> ExportSelected([FromBody] ExportSelection? selection)
    {
        try
        {
            _logger.LogInformation("Dashboard: Filtered export requested");
            selection ??= new ExportSelection();
            var yaml  = await _exportService.ExportToYamlAsync(selection);
            var stats = _exportService.GetLastExportStatistics();
            return Ok(new
            {
                yaml,
                statistics = new
                {
                    exportDate      = stats.ExportDate,
                    umbracoVersion  = stats.UmbracoVersion,
                    languages       = stats.LanguageCount,
                    dataTypes       = stats.DataTypeCount,
                    documentTypes   = stats.DocumentTypeCount,
                    mediaTypes      = stats.MediaTypeCount,
                    templates       = stats.TemplateCount,
                    media           = stats.MediaCount,
                    content         = stats.ContentCount,
                    dictionaryItems = stats.DictionaryItemCount,
                    members         = stats.MemberCount,
                    users           = stats.UserCount,
                    durationSeconds = stats.Duration.TotalSeconds
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Filtered export failed");
            return StatusCode(500, new { error = "Filtered export failed", message = ex.Message });
        }
    }

    // POST /umbraco/api/schemaexport/downloadyamlselected
    [HttpPost]
    public async Task<IActionResult> DownloadYamlSelected([FromBody] ExportSelection? selection)
    {
        try
        {
            selection ??= new ExportSelection();
            var yaml  = await _exportService.ExportToYamlAsync(selection);
            var bytes = Encoding.UTF8.GetBytes(yaml);
            return File(bytes, "application/x-yaml",
                $"umbraco-selected-{DateTime.UtcNow:yyyyMMdd-HHmmss}.yml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Filtered YAML download failed");
            return StatusCode(500, new { error = "Download failed", message = ex.Message });
        }
    }

    // POST /umbraco/api/schemaexport/downloadzipselected
    [HttpPost]
    public async Task<IActionResult> DownloadZipSelected([FromBody] ExportSelection? selection)
    {
        try
        {
            selection ??= new ExportSelection();
            var zip = await _exportService.ExportToZipAsync(selection);
            return File(zip, "application/zip",
                $"umbraco-selected-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Filtered ZIP download failed");
            return StatusCode(500, new { error = "ZIP creation failed", message = ex.Message });
        }
    }

    /// <summary>
    /// Gets export statistics without performing the export.
    /// GET: /umbraco/api/schemaexport/statistics
    /// </summary>
    [HttpGet]
    public IActionResult Statistics()
    {
        try
        {
            var stats = _exportService.GetLastExportStatistics();

            if (stats.DataTypeCount == 0)
            {
                return Ok(new { message = "No export performed yet" });
            }

            return Ok(new
            {
                exportDate = stats.ExportDate,
                umbracoVersion = stats.UmbracoVersion,
                languages = stats.LanguageCount,
                dataTypes = stats.DataTypeCount,
                documentTypes = stats.DocumentTypeCount,
                mediaTypes = stats.MediaTypeCount,
                templates = stats.TemplateCount,
                media = stats.MediaCount,
                content = stats.ContentCount,
                dictionaryItems = stats.DictionaryItemCount,
                members = stats.MemberCount,
                users = stats.UserCount,
                durationSeconds = stats.Duration.TotalSeconds
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get statistics");
            return StatusCode(500, new { error = "Failed to get statistics", message = ex.Message });
        }
    }
}
