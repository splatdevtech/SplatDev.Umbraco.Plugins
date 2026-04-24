using Microsoft.EntityFrameworkCore;
using SplatDev.Umbraco.Plugins.MostViewed.Models;

namespace SplatDev.Umbraco.Plugins.MostViewed.Services;

public class MostViewedService : IMostViewedService
{
    private readonly MostViewedDbContext _db;

    public MostViewedService(MostViewedDbContext db) => _db = db;

    public async Task RecordViewAsync(Guid contentKey, string nodeName, string nodeUrl, string viewerIp)
    {
        _db.PageViews.Add(new PageView
        {
            ContentKey = contentKey,
            NodeName = nodeName,
            NodeUrl = nodeUrl,
            ViewerIp = viewerIp,
            ViewedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }

    public async Task<PageViewSummary[]> GetMostViewedAsync(int count = 10, int days = 30)
    {
        var cutoff = DateTime.UtcNow.AddDays(-days);

        return await _db.PageViews
            .Where(v => v.ViewedAt >= cutoff)
            .GroupBy(v => new { v.ContentKey, v.NodeName, v.NodeUrl })
            .Select(g => new PageViewSummary
            {
                ContentKey = g.Key.ContentKey,
                NodeName = g.Key.NodeName,
                NodeUrl = g.Key.NodeUrl,
                ViewCount = g.Count()
            })
            .OrderByDescending(s => s.ViewCount)
            .Take(count)
            .ToArrayAsync();
    }

    public async Task<int> GetViewCountAsync(Guid contentKey)
    {
        return await _db.PageViews.CountAsync(v => v.ContentKey == contentKey);
    }
}
