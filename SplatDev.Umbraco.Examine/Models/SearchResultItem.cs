using Examine;

namespace SplatDev.Umbraco.Examine.Models
{
    public class SearchResultItem
    {
        public ISearchResult? Result { get; set; }
        public SearchResultItemDto? Item { get; set; }
        public float Score { get; set; }
    }
}
