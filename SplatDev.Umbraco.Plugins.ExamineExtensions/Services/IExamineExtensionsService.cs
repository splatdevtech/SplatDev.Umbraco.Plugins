using SplatDev.Umbraco.Plugins.ExamineExtensions.Models;

namespace SplatDev.Umbraco.Plugins.ExamineExtensions.Services;

public interface IExamineExtensionsService
{
    Task<SearchResult> SearchAsync(SearchRequest request);
    Task<IEnumerable<string>> GetAllIndexesAsync();
    Task RebuildIndexAsync(string indexName);
}
