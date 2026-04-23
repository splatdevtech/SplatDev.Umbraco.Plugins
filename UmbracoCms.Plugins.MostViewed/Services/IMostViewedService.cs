using UmbracoCms.Plugins.MostViewed.Models;

namespace UmbracoCms.Plugins.MostViewed.Services;

public interface IMostViewedService
{
    /// <summary>Records a page view for the given content node.</summary>
    Task RecordViewAsync(Guid contentKey, string nodeName, string nodeUrl, string viewerIp);

    /// <summary>Returns the most-viewed content nodes within the last <paramref name="days"/> days.</summary>
    Task<PageViewSummary[]> GetMostViewedAsync(int count = 10, int days = 30);

    /// <summary>Returns the total view count for a single content node.</summary>
    Task<int> GetViewCountAsync(Guid contentKey);
}
