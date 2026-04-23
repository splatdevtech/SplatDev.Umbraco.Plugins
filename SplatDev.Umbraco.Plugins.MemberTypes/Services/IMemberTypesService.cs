using Umbraco.Cms.Core.Models;

namespace SplatDev.Umbraco.Plugins.MemberTypes.Services;

public interface IMemberTypesService
{
    Task<IEnumerable<IMemberType>> GetAllAsync();
    Task<IMemberType?> GetByAliasAsync(string alias);
    Task<IMemberType> CreateAsync(string alias, string name, string description = "");
    Task<IMemberType> UpdateAsync(string alias, string name, string description);
    Task DeleteAsync(string alias);
}
