using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UmbracoCms.Plugins.Tweets.Models;

namespace UmbracoCms.Plugins.Tweets.Services;

public interface ITweetsService
{
    Task<List<CachedTweet>> GetCachedTweetsAsync();
    Task RefreshFromApiAsync();
}

public class TweetsService : ITweetsService
{
    private readonly TweetsDbContext _db;
    private readonly TweetSettings _settings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TweetsService> _logger;

    public TweetsService(
        TweetsDbContext db,
        IOptions<TweetSettings> settings,
        IHttpClientFactory httpClientFactory,
        ILogger<TweetsService> logger)
    {
        _db = db;
        _settings = settings.Value;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<List<CachedTweet>> GetCachedTweetsAsync()
    {
        return await _db.CachedTweets
            .OrderByDescending(t => t.PublishedAt)
            .Take(_settings.MaxTweets)
            .ToListAsync();
    }

    public async Task RefreshFromApiAsync()
    {
        if (string.IsNullOrWhiteSpace(_settings.BearerToken))
        {
            _logger.LogWarning("TweetsPlugin: BearerToken is not configured. Skipping API refresh.");
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.TwitterHandle))
        {
            _logger.LogWarning("TweetsPlugin: TwitterHandle is not configured. Skipping API refresh.");
            return;
        }

        try
        {
            var client = _httpClientFactory.CreateClient("TwitterV2");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.BearerToken);

            // Step 1: Look up the user ID by handle.
            var handle = _settings.TwitterHandle.TrimStart('@');
            var userUrl = $"https://api.twitter.com/2/users/by/username/{handle}?user.fields=profile_image_url,name,username";

            var userResponse = await client.GetAsync(userUrl);
            if (!userResponse.IsSuccessStatusCode)
            {
                _logger.LogError("TweetsPlugin: Failed to look up user {Handle}. Status: {Status}",
                    handle, userResponse.StatusCode);
                return;
            }

            var userJson = await userResponse.Content.ReadAsStringAsync();
            var userDoc = JsonDocument.Parse(userJson);
            var userId = userDoc.RootElement.GetProperty("data").GetProperty("id").GetString();
            var authorName = userDoc.RootElement.GetProperty("data").GetProperty("name").GetString() ?? handle;
            var avatarUrl = userDoc.RootElement.GetProperty("data")
                .TryGetProperty("profile_image_url", out var avatarProp)
                ? avatarProp.GetString() ?? string.Empty
                : string.Empty;

            // Step 2: Fetch recent tweets.
            var tweetUrl = $"https://api.twitter.com/2/users/{userId}/tweets"
                + $"?max_results={Math.Min(_settings.MaxTweets, 100)}"
                + $"&tweet.fields=created_at,public_metrics"
                + $"&expansions=author_id";

            var tweetResponse = await client.GetAsync(tweetUrl);
            if (!tweetResponse.IsSuccessStatusCode)
            {
                _logger.LogError("TweetsPlugin: Failed to fetch tweets. Status: {Status}",
                    tweetResponse.StatusCode);
                return;
            }

            var tweetJson = await tweetResponse.Content.ReadAsStringAsync();
            var tweetDoc = JsonDocument.Parse(tweetJson);

            if (!tweetDoc.RootElement.TryGetProperty("data", out var tweetsArray))
            {
                _logger.LogInformation("TweetsPlugin: No tweets returned for {Handle}.", handle);
                return;
            }

            var now = DateTime.UtcNow;

            foreach (var tweet in tweetsArray.EnumerateArray())
            {
                var tweetId = tweet.GetProperty("id").GetString() ?? string.Empty;
                var text = tweet.GetProperty("text").GetString() ?? string.Empty;
                var createdAtStr = tweet.TryGetProperty("created_at", out var createdProp)
                    ? createdProp.GetString()
                    : null;
                var publishedAt = createdAtStr != null
                    ? DateTime.Parse(createdAtStr, null, System.Globalization.DateTimeStyles.RoundtripKind)
                    : now;

                var metrics = tweet.TryGetProperty("public_metrics", out var metricsProp) ? metricsProp : default;
                var likeCount = metrics.ValueKind != JsonValueKind.Undefined
                    ? metrics.GetProperty("like_count").GetInt32()
                    : 0;
                var retweetCount = metrics.ValueKind != JsonValueKind.Undefined
                    ? metrics.GetProperty("retweet_count").GetInt32()
                    : 0;

                var tweetUrlStr = $"https://twitter.com/{handle}/status/{tweetId}";

                var existing = await _db.CachedTweets.FirstOrDefaultAsync(t => t.TweetId == tweetId);
                if (existing == null)
                {
                    _db.CachedTweets.Add(new CachedTweet
                    {
                        TweetId = tweetId,
                        AuthorHandle = handle,
                        AuthorName = authorName,
                        AuthorAvatarUrl = avatarUrl,
                        Content = text,
                        TweetUrl = tweetUrlStr,
                        PublishedAt = publishedAt,
                        LikeCount = likeCount,
                        RetweetCount = retweetCount,
                        CachedAt = now,
                    });
                }
                else
                {
                    existing.LikeCount = likeCount;
                    existing.RetweetCount = retweetCount;
                    existing.CachedAt = now;
                }
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("TweetsPlugin: Cache refreshed for @{Handle}.", handle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TweetsPlugin: Unexpected error during API refresh.");
        }
    }
}
