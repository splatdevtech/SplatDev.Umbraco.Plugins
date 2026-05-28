using Examine;
using SplatDev.Umbraco.Plugins.ExamineExtensions.Models;
using Sr = SplatDev.Umbraco.Plugins.ExamineExtensions.Models.SearchResult;

namespace SplatDev.Umbraco.Plugins.ExamineExtensions.Services;

public class ExamineExtensionsService : IExamineExtensionsService
{
    private readonly IExamineManager _examineManager;

    public ExamineExtensionsService(IExamineManager examineManager)
    {
        _examineManager = examineManager;
    }

    public Task<Sr> SearchAsync(SearchRequest request)
    {
        var result = new Sr();

        if (!_examineManager.TryGetIndex(request.IndexName, out var index))
            return Task.FromResult(result);

        var searcher = index.Searcher;
        var query = searcher.CreateQuery();

        ISearchResults searchResults;

#if NET10_0_OR_GREATER
        if (request.Fields?.Length > 0)
        {
            var booleanOp = query.GroupedOr(request.Fields, request.Query);
            searchResults = booleanOp.Execute(new Examine.Search.QueryOptions(
                (request.Page - 1) * request.PageSize, request.PageSize));
        }
        else
        {
            searchResults = query
                .ManagedQuery(request.Query)
                .Execute(new Examine.Search.QueryOptions(
                    (request.Page - 1) * request.PageSize, request.PageSize));
        }
#else
        if (request.Fields?.Length > 0)
        {
            var booleanOp = query.GroupedOr(request.Fields, request.Query);
            searchResults = booleanOp.Execute(QueryOptions.SkipTake(
                (request.Page - 1) * request.PageSize, request.PageSize));
        }
        else
        {
            searchResults = query
                .ManagedQuery(request.Query)
                .Execute(QueryOptions.SkipTake(
                    (request.Page - 1) * request.PageSize, request.PageSize));
        }
#endif

        result.TotalItems = searchResults.Count();
        result.Items = searchResults.Select(r => new SearchResultItem
        {
            Id = r.Id,
            Score = r.Score,
        }).ToList();

        return Task.FromResult(result);
    }

    public Task<IEnumerable<string>> GetAllIndexesAsync()
    {
        return Task.FromResult(
            _examineManager.Indexes.Select(i => i.Name).AsEnumerable());
    }

    public Task RebuildIndexAsync(string indexName)
    {
#if !NET10_0_OR_GREATER
        if (_examineManager.TryGetIndex(indexName, out var index))
            index.Rebuild();
#else
        if (_examineManager.TryGetIndex(indexName, out var index))
            ((global::Examine.IIndex)index).CreateIndex();
#endif
        return Task.CompletedTask;
    }
}
