using Umbraco.Cms.Core.Models;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IDynamicRootContentLocator
    {
        Task<IContent?> GetContent(string rootNodeSettingValue, int currentPageId);

        Task<IContent?> CreateContent(
          string rootNodeSettingValue,
          string nodeName,
          string contentTypeAlias,
          int currentPageId);
    }
}