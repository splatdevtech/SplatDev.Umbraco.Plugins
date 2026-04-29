using SplatDev.Umbraco.Plugins.EmailNotifications.Models;

namespace SplatDev.Umbraco.Plugins.EmailNotifications.Services;

public record CampaignStats(
    int TotalRecipients,
    int SentCount,
    int DeliveredCount,
    int OpenedCount,
    int ClickedCount,
    int BouncedCount,
    int UnsubscribedCount);

public interface INewsletterService
{
    Task<List<Subscriber>> GetSubscribersAsync(string? listId = null, CancellationToken ct = default);
    Task<Subscriber?> SubscribeAsync(string email, string? listId = null, string? memberId = null,
        string? firstName = null, string? lastName = null, CancellationToken ct = default);
    Task<bool> UnsubscribeAsync(string email, string? listId = null, CancellationToken ct = default);

    Task<List<Campaign>> GetCampaignsAsync(CancellationToken ct = default);
    Task<Campaign?> GetCampaignAsync(int id, CancellationToken ct = default);
    Task<Campaign> CreateCampaignAsync(Campaign campaign, CancellationToken ct = default);
    Task<Campaign?> UpdateCampaignAsync(int id, Campaign campaign, CancellationToken ct = default);

    /// <summary>Sends a campaign immediately to all opted-in subscribers in the target list.</summary>
    Task<CampaignStats> ScheduleSendAsync(int campaignId, CancellationToken ct = default);

    Task<CampaignStats> GetStatsAsync(int campaignId, CancellationToken ct = default);
}
