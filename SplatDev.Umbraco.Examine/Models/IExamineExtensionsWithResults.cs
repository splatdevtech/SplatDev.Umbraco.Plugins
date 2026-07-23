using Umbraco.Cms.Core.Models.PublishedContent;

namespace SplatDev.Umbraco.Examine.Models
{
    public interface IExamineExtensionsWithResults
    {
        string Keywords { get; set; }
        int CurrentPage { get; set; }
        int ItemsPerPage { get; set; }
        int TotalPages { get; set; }
        int[] PageRange { get; set; }
        IList<IPublishedContent> Results { get; set; }
        int TotalItems { get; set; }
    }
}
