namespace SplatDev.Search;

public interface ISearchProvider<TDoc>
    where TDoc : class
{
    Task<SearchResult<TDoc>> SearchAsync(SearchQuery query, CancellationToken cancellationToken = default);

    Task<AutocompleteResult> AutocompleteAsync(string prefix, AutocompleteOptions? options = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Facet>> FacetsAsync(SearchQuery query, IEnumerable<string> fields, CancellationToken cancellationToken = default);
}
