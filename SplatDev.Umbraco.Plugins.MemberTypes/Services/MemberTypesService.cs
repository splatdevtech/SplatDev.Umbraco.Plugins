using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.MemberTypes.Services;

public class MemberTypesService : IMemberTypesService
{
    private readonly IMemberTypeService _memberTypeService;
    private readonly ILogger<MemberTypesService> _logger;

    public MemberTypesService(IMemberTypeService memberTypeService, ILogger<MemberTypesService> logger)
    {
        _memberTypeService = memberTypeService;
        _logger = logger;
    }

    public Task<IEnumerable<IMemberType>> GetAllAsync()
    {
        var types = _memberTypeService.GetAll();
        return Task.FromResult(types);
    }

    public Task<IMemberType?> GetByAliasAsync(string alias)
    {
        var type = _memberTypeService.Get(alias);
        return Task.FromResult(type);
    }

    public Task<IMemberType> CreateAsync(string alias, string name, string description = "")
    {
        var memberType = new MemberType(null, -1)
        {
            Alias = alias,
            Name = name,
            Description = description
        };

        _memberTypeService.Save(memberType);
        _logger.LogInformation("Created member type {Alias}", alias);
        return Task.FromResult<IMemberType>(memberType);
    }

    public Task<IMemberType> UpdateAsync(string alias, string name, string description)
    {
        var memberType = _memberTypeService.Get(alias)
            ?? throw new InvalidOperationException($"Member type '{alias}' not found.");

        memberType.Name = name;
        memberType.Description = description;

        _memberTypeService.Save(memberType);
        _logger.LogInformation("Updated member type {Alias}", alias);
        return Task.FromResult(memberType);
    }

    public Task DeleteAsync(string alias)
    {
        var memberType = _memberTypeService.Get(alias);
        if (memberType is not null)
        {
            _memberTypeService.Delete(memberType);
            _logger.LogInformation("Deleted member type {Alias}", alias);
        }
        return Task.CompletedTask;
    }
}
