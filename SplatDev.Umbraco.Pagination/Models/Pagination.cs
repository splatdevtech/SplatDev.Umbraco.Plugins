namespace SplatDev.Umbraco.Pagination.Models
{
    public class Pagination
    {
        public int PageSize { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public long TotalResults { get; set; }
        public string? SearchTerm { get; set; }
        public string? SearchDataType { get; set; }
    }
}
