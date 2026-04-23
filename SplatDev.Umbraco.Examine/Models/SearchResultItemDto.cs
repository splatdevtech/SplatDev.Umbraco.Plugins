using SplatDev.Umbraco.Pagination.Models;

namespace SplatDev.Umbraco.Examine.Models
{
    public class SearchResultItemDto : IEntity, IDescription, IText
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Url { get; set; }
    }
}
