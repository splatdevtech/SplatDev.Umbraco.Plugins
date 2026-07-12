namespace SplatDev.Messaging.Newsletter.Interfaces
{
    using SplatDev.Messaging.Newsletter.Models;

    public interface INewsletterProvider
    {
        Task<NewsletterList> CreateListAsync(NewsletterList list);

        Task<NewsletterList?> GetListAsync(string listId);

        Task<IEnumerable<NewsletterList>> GetListsAsync();

        Task<NewsletterList> UpdateListAsync(NewsletterList list);

        Task<bool> DeleteListAsync(string listId);

        Task<NewsletterSubscriber> SubscribeAsync(string listId, NewsletterSubscriber subscriber);

        Task<NewsletterSubscriber?> GetSubscriberAsync(string listId, string subscriberId);

        Task<IEnumerable<NewsletterSubscriber>> GetSubscribersAsync(string listId);

        Task<NewsletterSubscriber> UpdateSubscriberAsync(string listId, NewsletterSubscriber subscriber);

        Task<bool> UnsubscribeAsync(string listId, string subscriberId);

        Task<bool> DeleteSubscriberAsync(string listId, string subscriberId);

        Task<NewsletterCampaign> CreateCampaignAsync(NewsletterCampaign campaign);

        Task<NewsletterCampaign?> GetCampaignAsync(string campaignId);

        Task<IEnumerable<NewsletterCampaign>> GetCampaignsAsync(string listId);

        Task<NewsletterCampaign> UpdateCampaignAsync(NewsletterCampaign campaign);

        Task<bool> DeleteCampaignAsync(string campaignId);

        Task<NewsletterCampaign> SendCampaignAsync(string campaignId);

        Task<CampaignStats?> GetCampaignStatsAsync(string campaignId);
    }
}
