using SplatDev.Umbraco.Plugins.Newsletter.Models;

namespace SplatDev.Umbraco.Plugins.Newsletter.Services;

public interface INewsletterService
{
    // Subscriber lists
    IReadOnlyList<SubscriberList> GetAllLists();
    SubscriberList? GetListById(int id);
    SubscriberList CreateList(string name);
    bool DeleteList(int id);

    // Subscribers
    IReadOnlyList<Subscriber> GetSubscribers(int listId);
    Subscriber? Subscribe(int listId, string email, string? name = null, Guid? memberKey = null);
    bool Unsubscribe(int listId, string email);
    bool DeleteSubscriber(int id);

    // Campaigns
    IReadOnlyList<Campaign> GetAllCampaigns();
    Campaign? GetCampaignById(int id);
    Campaign Create(Campaign campaign);
    Campaign? Update(int id, Campaign campaign);
    bool DeleteCampaign(int id);

    // Send
    Task<int> SendCampaignAsync(int campaignId, CancellationToken ct = default);

    // Stats
    CampaignStats? GetStats(int campaignId);
    Task<CampaignStats?> FetchStatsFromMailgunAsync(int campaignId, CancellationToken ct = default);
}
