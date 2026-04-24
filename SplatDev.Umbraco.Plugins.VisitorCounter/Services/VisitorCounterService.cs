using Microsoft.EntityFrameworkCore;
using SplatDev.Umbraco.Plugins.VisitorCounter.Models;

namespace SplatDev.Umbraco.Plugins.VisitorCounter.Services;

public class VisitorCounterService : IVisitorCounterService
{
    private readonly VisitorCounterDbContext _db;

    public VisitorCounterService(VisitorCounterDbContext db) => _db = db;

    public async Task RecordVisitAsync(string sessionId, string requestPath)
    {
        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);

        var session = await _db.VisitorSessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        bool isNew = session is null;

        if (session is null)
        {
            session = new VisitorSession
            {
                SessionId = sessionId,
                FirstSeenAt = now,
                LastSeenAt = now,
                PageCount = 1,
                IsUnique = true
            };
            _db.VisitorSessions.Add(session);
        }
        else
        {
            session.LastSeenAt = now;
            session.PageCount++;
        }

        // Update or insert daily aggregate
        var daily = await _db.DailyVisitorCounts
            .FirstOrDefaultAsync(d => d.Date == today);

        if (daily is null)
        {
            daily = new DailyVisitorCount
            {
                Date = today,
                TotalVisits = 1,
                UniqueVisits = isNew ? 1 : 0
            };
            _db.DailyVisitorCounts.Add(daily);
        }
        else
        {
            daily.TotalVisits++;
            if (isNew) daily.UniqueVisits++;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<long> GetTotalVisitsAsync()
    {
        return await _db.VisitorSessions.SumAsync(s => (long)s.PageCount);
    }

    public async Task<long> GetUniqueVisitsAsync(int days = 30)
    {
        var cutoff = DateTime.UtcNow.AddDays(-days);
        return await _db.VisitorSessions
            .CountAsync(s => s.FirstSeenAt >= cutoff);
    }

    public async Task<DailyVisitorCount[]> GetDailyCountsAsync(int days = 30)
    {
        var cutoff = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-days));
        return await _db.DailyVisitorCounts
            .Where(d => d.Date >= cutoff)
            .OrderBy(d => d.Date)
            .ToArrayAsync();
    }
}
