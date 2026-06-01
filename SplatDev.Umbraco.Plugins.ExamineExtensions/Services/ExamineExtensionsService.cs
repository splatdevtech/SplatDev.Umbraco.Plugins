using Examine;
using SplatDev.Umbraco.Plugins.ExamineExtensions.Models;

namespace SplatDev.Umbraco.Plugins.ExamineExtensions.Services;

public class ExamineExtensionsService : IExamineExtensionsService
{
    private readonly IExamineManager _examineManager;

    public ExamineExtensionsService(IExamineManager examineManager)
    {
        _examineManager = examineManager;
    }

    public Task<Models.SearchResult> SearchAsync(SearchRequest request)
    {
        var result = new Models.SearchResult();

        if (!_examineManager.TryGetIndex(request.IndexName, out var index))
            return Task.FromResult<Models.SearchResult>(result);

        var searcher = index.Searcher;
        var query = searcher.CreateQuery();

        ISearchResults searchResults;

        var skip = (request.Page - 1) * request.PageSize;
        var take = request.PageSize;

        if (request.Fields?.Length > 0)
        {
            var booleanOp = query.GroupedOr(request.Fields, request.Query);
            searchResults = booleanOp.Execute(Examine.Search.QueryOptions.SkipTake(skip, take));
        }
        else
        {
            searchResults = query
                .ManagedQuery(request.Query)
                .Execute(Examine.Search.QueryOptions.SkipTake(skip, take));
        }

        result.TotalItems = searchResults.TotalItemCount;
        result.Items = searchResults.Select(r => new SearchResultItem
        {
            Id = r.Id,
            Score = r.Score,
            Fields = r.AllValues.ToDictionary(kvp => kvp.Key, kvp => string.Join(", ", kvp.Value))
        }).ToList();

        return Task.FromResult<Models.SearchResult>(result);
    }

    public Task<IEnumerable<string>> GetAllIndexesAsync()
    {
        var names = _examineManager.Indexes.Select(i => i.Name);
        return Task.FromResult(names);
    }

    public Task RebuildIndexAsync(string indexName)
    {
        if (_examineManager.TryGetIndex(indexName, out var index))
        {
            index.CreateIndex();
        }
        return Task.CompletedTask;
    }
}
