using Microsoft.EntityFrameworkCore;
using SplatDev.Umbraco.Plugins.Newsletters.Models;

namespace SplatDev.Umbraco.Plugins.Newsletters.Services;

public interface INewslettersService
{
    Task<bool> SubscribeAsync(string email, string firstName, string lastName);
    Task<bool> UnsubscribeAsync(string email);
    Task<List<NewsletterSubscriber>> GetSubscribersAsync();
    Task SendCampaignAsync(int campaignId);
    Task<List<NewsletterCampaign>> GetCampaignsAsync();
}

public class NewslettersService : INewslettersService
{
    private readonly NewslettersDbContext _db;

    public NewslettersService(NewslettersDbContext db)
    {
        _db = db;
    }

    public async Task<bool> SubscribeAsync(string email, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var existing = await _db.Subscribers
            .FirstOrDefaultAsync(s => s.Email == email.ToLowerInvariant());

        if (existing != null)
        {
            if (existing.UnsubscribedAt.HasValue)
            {
                // Re-subscribe
                existing.UnsubscribedAt = null;
                existing.SubscribedAt = DateTime.UtcNow;
                existing.IsConfirmed = false;
                existing.FirstName = firstName;
                existing.LastName = lastName;
                await _db.SaveChangesAsync();
                return true;
            }

            // Already subscribed
            return false;
        }

        _db.Subscribers.Add(new NewsletterSubscriber
        {
            Email = email.ToLowerInvariant().Trim(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            SubscribedAt = DateTime.UtcNow,
            IsConfirmed = false,
        });

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnsubscribeAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var subscriber = await _db.Subscribers
            .FirstOrDefaultAsync(s => s.Email == email.ToLowerInvariant());

        if (subscriber == null || subscriber.UnsubscribedAt.HasValue)
            return false;

        subscriber.UnsubscribedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<NewsletterSubscriber>> GetSubscribersAsync()
    {
        return await _db.Subscribers
            .Where(s => s.UnsubscribedAt == null)
            .OrderByDescending(s => s.SubscribedAt)
            .ToListAsync();
    }

    public async Task SendCampaignAsync(int campaignId)
    {
        var campaign = await _db.Campaigns.FindAsync(campaignId)
            ?? throw new InvalidOperationException($"Campaign {campaignId} not found.");

        if (campaign.Status == CampaignStatus.Sent)
            throw new InvalidOperationException("Campaign has already been sent.");

        var activeSubscribers = await _db.Subscribers
            .Where(s => s.UnsubscribedAt == null && s.IsConfirmed)
            .ToListAsync();

        var now = DateTime.UtcNow;
        var sends = activeSubscribers.Select(sub => new NewsletterSend
        {
            CampaignId = campaignId,
            SubscriberId = sub.Id,
            SentAt = now,
            IsOpened = false,
        }).ToList();

        // In a real implementation this would invoke an email delivery service.
        // Here we record the send records and mark the campaign as sent.
        await _db.Sends.AddRangeAsync(sends);

        campaign.Status = CampaignStatus.Sent;
        campaign.SentAt = now;

        await _db.SaveChangesAsync();
    }

    public async Task<List<NewsletterCampaign>> GetCampaignsAsync()
    {
        return await _db.Campaigns
            .OrderByDescending(c => c.Id)
            .ToListAsync();
    }
}
