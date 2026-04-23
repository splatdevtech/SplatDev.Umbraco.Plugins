using SplatDev.Umbraco.Plugins.StarRatings.Models;

namespace SplatDev.Umbraco.Plugins.StarRatings.Services;

public interface IStarRatingsService
{
    /// <summary>Returns the average rating (1–5) for <paramref name="contentKey"/>, or 0 if unrated.</summary>
    Task<double> GetAverageAsync(Guid contentKey);

    /// <summary>Returns the total number of votes cast for <paramref name="contentKey"/>.</summary>
    Task<int> GetVoteCountAsync(Guid contentKey);

    /// <summary>Records a rating. Throws <see cref="ArgumentOutOfRangeException"/> if rating is not 1–5.</summary>
    Task RateAsync(Guid contentKey, int rating, string voterIp);

    /// <summary>Returns the <paramref name="count"/> highest-rated content items.</summary>
    Task<ContentRatingSummary[]> GetTopRatedAsync(int count = 10);
}
