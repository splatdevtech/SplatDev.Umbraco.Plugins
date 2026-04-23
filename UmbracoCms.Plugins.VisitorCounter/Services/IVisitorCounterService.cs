using UmbracoCms.Plugins.VisitorCounter.Models;

namespace UmbracoCms.Plugins.VisitorCounter.Services;

public interface IVisitorCounterService
{
    /// <summary>Records or updates a visit for the given session.</summary>
    Task RecordVisitAsync(string sessionId, string requestPath);

    /// <summary>Returns the all-time total visit count (all sessions × pages).</summary>
    Task<long> GetTotalVisitsAsync();

    /// <summary>Returns the number of unique sessions within the last <paramref name="days"/> days.</summary>
    Task<long> GetUniqueVisitsAsync(int days = 30);

    /// <summary>Returns per-day visitor counts for the last <paramref name="days"/> days.</summary>
    Task<DailyVisitorCount[]> GetDailyCountsAsync(int days = 30);
}
