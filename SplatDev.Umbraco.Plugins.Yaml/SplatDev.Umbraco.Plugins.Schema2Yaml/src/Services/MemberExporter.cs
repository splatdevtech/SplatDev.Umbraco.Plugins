using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Configuration;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Services;

/// <summary>
/// Exports Umbraco Members to YAML format (passwords excluded for security).
/// </summary>
public class MemberExporter
{
    private readonly IMemberService _memberService;
    private readonly Schema2YamlOptions _options;
    private readonly ILogger<MemberExporter> _logger;

    public MemberExporter(
        IMemberService memberService,
        IOptions<Schema2YamlOptions> options,
        ILogger<MemberExporter> logger)
    {
        _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Exports all Members from Umbraco.
    /// </summary>
    public async Task<List<ExportMember>> ExportAsync()
    {
        if (!_options.IncludeMembers)
        {
            _logger.LogInformation("Member export is disabled in configuration");
            return [];
        }

        _logger.LogInformation("Starting Member export");

        var members = _memberService.GetAll(0, int.MaxValue, out _);
        var exported = new List<ExportMember>();

        foreach (var member in members)
        {
            try
            {
                var export = new ExportMember
                {
                    Name = member.Name ?? string.Empty,
                    Email = member.Email,
                    Username = member.Username,
                    MemberType = member.ContentType.Alias,
                    IsApproved = member.IsApproved,
                    Properties = ExportProperties(member)
                };

                exported.Add(export);
                _logger.LogDebug("Exported Member: {Name} ({Email})", export.Name, export.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export Member: {Name}", member.Name);
            }
        }

        _logger.LogInformation("Exported {Count} Members (passwords excluded)", exported.Count);
        return await Task.FromResult(exported);
    }

    /// <summary>
    /// Exports Members filtered by a CategorySelection.
    /// </summary>
    public virtual async Task<List<ExportMember>> ExportAsync(CategorySelection filter)
    {
        if (!filter.IncludeAll && filter.Aliases.Count == 0)
            return [];
        var all = await ExportAsync();
        if (filter.IncludeAll) return all;
        return all.Where(x => filter.Aliases.Contains(x.Email)).ToList();
    }

    /// <summary>
    /// Exports all property values from a member.
    /// </summary>
    private Dictionary<string, object> ExportProperties(IMember member)
    {
        var properties = new Dictionary<string, object>();

        foreach (var property in member.Properties)
        {
            try
            {
                var value = property.GetValue();
                if (value != null)
                {
                    properties[property.Alias] = ConvertPropertyValue(value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to export property: {Alias} on {MemberName}", 
                    property.Alias, member.Name);
            }
        }

        return properties;
    }

    /// <summary>
    /// Converts property values to YAML-serializable format.
    /// </summary>
    private object ConvertPropertyValue(object value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        return value switch
        {
            string str => str,
            int or long or decimal or double or float => value,
            bool b => b,
            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
            _ => value.ToString() ?? string.Empty
        };
    }
}
