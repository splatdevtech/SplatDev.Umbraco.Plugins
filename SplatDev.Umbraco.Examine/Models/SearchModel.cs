using SplatDev.Umbraco.Pagination.Models;

using Umbraco.Cms.Core.Models.PublishedContent;

namespace SplatDev.Umbraco.Examine.Models
{
    public partial class SearchModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : PublishedContentWrapped(content, publishedValueFallback)
    {
        public string? Query { get; set; }
        public long TotalResults { get; set; }
        public PersistenceDataType? SearchDataType { get; set; }
        public PagedResults<SearchResultItem>? SearchResults { get; set; }

    }
}
