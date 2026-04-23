using Microsoft.EntityFrameworkCore;
using SplatDev.Umbraco.Plugins.StarRatings.Models;

namespace SplatDev.Umbraco.Plugins.StarRatings.Services;

public class StarRatingsService : IStarRatingsService
{
    private readonly StarRatingsDbContext _db;

    public StarRatingsService(StarRatingsDbContext db) => _db = db;

    public async Task<double> GetAverageAsync(Guid contentKey)
    {
        var ratings = await _db.ContentRatings
            .Where(r => r.ContentKey == contentKey)
            .Select(r => r.Rating)
            .ToListAsync();

        return ratings.Count == 0 ? 0d : ratings.Average();
    }

    public async Task<int> GetVoteCountAsync(Guid contentKey)
    {
        return await _db.ContentRatings
            .CountAsync(r => r.ContentKey == contentKey);
    }

    public async Task RateAsync(Guid contentKey, int rating, string voterIp)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

        // Update existing vote from this IP for this content, or add new
        var existing = await _db.ContentRatings
            .FirstOrDefaultAsync(r => r.ContentKey == contentKey && r.VoterIp == voterIp);

        if (existing is not null)
        {
            existing.Rating = rating;
            existing.RatedAt = DateTime.UtcNow;
        }
        else
        {
            _db.ContentRatings.Add(new ContentRating
            {
                ContentKey = contentKey,
                Rating = rating,
                VoterIp = voterIp,
                RatedAt = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();
    }

    public async Task<ContentRatingSummary[]> GetTopRatedAsync(int count = 10)
    {
        return await _db.ContentRatings
            .GroupBy(r => r.ContentKey)
            .Select(g => new ContentRatingSummary
            {
                ContentKey = g.Key,
                AverageRating = g.Average(r => r.Rating),
                TotalVotes = g.Count()
            })
            .OrderByDescending(s => s.AverageRating)
            .ThenByDescending(s => s.TotalVotes)
            .Take(count)
            .ToArrayAsync();
    }
}
