using UmbracoCms.Plugins.ExamineExtensions.Models;

namespace UmbracoCms.Plugins.ExamineExtensions.Services;

public interface IExamineExtensionsService
{
    Task<SearchResult> SearchAsync(SearchRequest request);
    Task<IEnumerable<string>> GetAllIndexesAsync();
    Task RebuildIndexAsync(string indexName);
}
