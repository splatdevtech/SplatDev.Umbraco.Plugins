using UmbracoCms.Plugins.DefaultValue.Models;

namespace UmbracoCms.Plugins.DefaultValue.Services;

public interface IDefaultValueService
{
    Task<IEnumerable<DefaultValueRule>> GetRulesAsync();
    Task<IEnumerable<DefaultValueRule>> GetRulesForTypeAsync(string documentTypeAlias);
    Task<DefaultValueRule> SaveRuleAsync(DefaultValueRule rule);
    Task DeleteRuleAsync(int id);
    Task ApplyDefaultsAsync(string documentTypeAlias, IDictionary<string, object?> properties);
}
