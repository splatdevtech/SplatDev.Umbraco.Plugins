using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Configuration;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Services;

/// <summary>
/// Exports Umbraco Users to YAML format (passwords excluded for security).
/// </summary>
public class UserExporter
{
    private readonly IUserService _userService;
    private readonly Schema2YamlOptions _options;
    private readonly ILogger<UserExporter> _logger;

    public UserExporter(
        IUserService userService,
        IOptions<Schema2YamlOptions> options,
        ILogger<UserExporter> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Exports all Users from Umbraco.
    /// </summary>
    public async Task<List<ExportUser>> ExportAsync()
    {
        if (!_options.IncludeUsers)
        {
            _logger.LogInformation("User export is disabled in configuration (security consideration)");
            return [];
        }

        _logger.LogInformation("Starting User export");

        var users = _userService.GetAll(0, int.MaxValue, out _);
        var exported = new List<ExportUser>();

        foreach (var user in users)
        {
            try
            {
                var export = new ExportUser
                {
                    Name = user.Name ?? string.Empty,
                    Email = user.Email,
                    Username = user.Username,
                    UserGroups = user.Groups?
                        .Select(g => g.Alias)
                        .ToList() ?? []
                };

                exported.Add(export);
                _logger.LogDebug("Exported User: {Name} ({Email})", export.Name, export.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export User: {Name}", user.Name);
            }
        }

        _logger.LogInformation("Exported {Count} Users (passwords excluded)", exported.Count);
        _logger.LogWarning(
            "User export includes sensitive information. Ensure exported YAML is stored securely and not committed to public repositories.");

        return await Task.FromResult(exported);
    }

    /// <summary>
    /// Exports Users filtered by a CategorySelection.
    /// </summary>
    public virtual async Task<List<ExportUser>> ExportAsync(CategorySelection filter)
    {
        if (!filter.IncludeAll && filter.Aliases.Count == 0)
            return [];
        var all = await ExportAsync();
        if (filter.IncludeAll) return all;
        return all.Where(x => filter.Aliases.Contains(x.Email)).ToList();
    }
}
