using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.DefaultValue.Models;

namespace SplatDev.Umbraco.Plugins.DefaultValue.Services;

public class DefaultValueService : IDefaultValueService
{
    private readonly DefaultValueDbContext _db;
    private readonly ILogger<DefaultValueService> _logger;

    public DefaultValueService(DefaultValueDbContext db, ILogger<DefaultValueService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<DefaultValueRule>> GetRulesAsync()
    {
        return await _db.DefaultValueRules
            .OrderBy(r => r.DocumentTypeAlias)
            .ThenBy(r => r.Priority)
            .ThenBy(r => r.PropertyAlias)
            .ToListAsync();
    }

    public async Task<IEnumerable<DefaultValueRule>> GetRulesForTypeAsync(string documentTypeAlias)
    {
        return await _db.DefaultValueRules
            .Where(r => r.DocumentTypeAlias == documentTypeAlias && r.IsEnabled)
            .OrderBy(r => r.Priority)
            .ThenBy(r => r.PropertyAlias)
            .ToListAsync();
    }

    public async Task<DefaultValueRule> SaveRuleAsync(DefaultValueRule rule)
    {
        if (rule.Id == 0)
        {
            _db.DefaultValueRules.Add(rule);
        }
        else
        {
            var existing = await _db.DefaultValueRules.FindAsync(rule.Id);
            if (existing is null)
                throw new KeyNotFoundException($"Rule with id {rule.Id} not found.");

            existing.DocumentTypeAlias = rule.DocumentTypeAlias;
            existing.PropertyAlias = rule.PropertyAlias;
            existing.DefaultValue = rule.DefaultValue;
            existing.IsEnabled = rule.IsEnabled;
            existing.Priority = rule.Priority;
            rule = existing;
        }

        await _db.SaveChangesAsync();
        return rule;
    }

    public async Task DeleteRuleAsync(int id)
    {
        var rule = await _db.DefaultValueRules.FindAsync(id);
        if (rule is not null)
        {
            _db.DefaultValueRules.Remove(rule);
            await _db.SaveChangesAsync();
        }
    }

    public async Task ApplyDefaultsAsync(string documentTypeAlias, IDictionary<string, object?> properties)
    {
        var rules = await GetRulesForTypeAsync(documentTypeAlias);

        foreach (var rule in rules)
        {
            if (!properties.ContainsKey(rule.PropertyAlias) || properties[rule.PropertyAlias] is null)
            {
                properties[rule.PropertyAlias] = rule.DefaultValue;
                _logger.LogDebug(
                    "DefaultValue: Applied default '{Value}' to property '{Property}' on doc type '{DocType}'.",
                    rule.DefaultValue, rule.PropertyAlias, documentTypeAlias);
            }
        }
    }
}
