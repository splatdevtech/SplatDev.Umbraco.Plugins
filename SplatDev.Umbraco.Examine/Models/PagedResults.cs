namespace SplatDev.Umbraco.Examine.Models
{
    public class PagedResults<TEntity> where TEntity : class?
    {
        public IList<TEntity>? Results { get; set; } = [];
        public Pagination Pagination { get; set; } = new();
        public bool FullQueryString { get; set; }
        public bool ShowSearchTerm { get; set; }

        public static explicit operator PagedResultsViewModel(PagedResults<TEntity> pagedResults)
        {
            return new PagedResultsViewModel
            {
                Results = [.. pagedResults.Results],
                Pagination = pagedResults.Pagination,
                FullQueryString = pagedResults.FullQueryString,
                ShowSearchTerm = pagedResults.ShowSearchTerm
            };
        }
    }

    public class PagedResultsViewModel
    {
        public List<object>? Results { get; set; } = [];
        public Pagination Pagination { get; set; } = new();
        public bool FullQueryString { get; set; }
        public bool ShowSearchTerm { get; set; }
    }
}
