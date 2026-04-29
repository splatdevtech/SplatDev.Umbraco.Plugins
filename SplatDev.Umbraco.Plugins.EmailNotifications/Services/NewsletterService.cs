using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.EmailNotifications.Models;

namespace SplatDev.Umbraco.Plugins.EmailNotifications.Services;

public class NewsletterService(
    EmailNotificationsDbContext db,
    IEmailTemplateService templateService,
    IMailProvider mailProvider,
    ILogger<NewsletterService> logger) : INewsletterService
{
    // Mailgun free/pay-as-you-go allows 300 messages/minute; stay safely below that.
    private const int BatchSize = 50;
    private static readonly TimeSpan BatchDelay = TimeSpan.FromSeconds(12);

    public Task<List<Subscriber>> GetSubscribersAsync(string? listId = null, CancellationToken ct = default)
    {
        var query = db.Subscribers.Where(s => s.OptedIn && s.OptedOutAt == null);
        if (listId is not null)
            query = query.Where(s => s.ListId == listId);
        return query.OrderBy(s => s.Email).ToListAsync(ct);
    }

    public async Task<Subscriber?> SubscribeAsync(string email, string? listId = null, string? memberId = null,
        string? firstName = null, string? lastName = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var normalised = email.ToLowerInvariant().Trim();
        var existing = await db.Subscribers
            .FirstOrDefaultAsync(s => s.Email == normalised && s.ListId == listId, ct);

        if (existing is not null)
        {
            if (existing.OptedOutAt.HasValue)
            {
                existing.OptedIn = true;
                existing.OptedOutAt = null;
                existing.SubscribedAt = DateTime.UtcNow;
                await db.SaveChangesAsync(ct);
            }

            return existing;
        }

        var sub = new Subscriber
        {
            Email = normalised,
            ListId = listId,
            MemberId = memberId,
            FirstName = firstName?.Trim(),
            LastName = lastName?.Trim(),
            OptedIn = true,
            SubscribedAt = DateTime.UtcNow,
        };

        db.Subscribers.Add(sub);
        await db.SaveChangesAsync(ct);
        return sub;
    }

    public async Task<bool> UnsubscribeAsync(string email, string? listId = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var normalised = email.ToLowerInvariant().Trim();
        var subscriber = await db.Subscribers
            .FirstOrDefaultAsync(s => s.Email == normalised && s.ListId == listId, ct);

        if (subscriber is null || subscriber.OptedOutAt.HasValue)
            return false;

        subscriber.OptedIn = false;
        subscriber.OptedOutAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public Task<List<Campaign>> GetCampaignsAsync(CancellationToken ct = default) =>
        db.Campaigns.Include(c => c.Template).OrderByDescending(c => c.Id).ToListAsync(ct);

    public Task<Campaign?> GetCampaignAsync(int id, CancellationToken ct = default) =>
        db.Campaigns.Include(c => c.Template).FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Campaign> CreateCampaignAsync(Campaign campaign, CancellationToken ct = default)
    {
        campaign.Id = 0;
        campaign.Status = CampaignStatus.Draft;
        campaign.CreatedAt = DateTime.UtcNow;
        campaign.UpdatedAt = null;
        campaign.SentAt = null;
        campaign.SentCount = 0;
        campaign.RecipientCount = 0;
        db.Campaigns.Add(campaign);
        await db.SaveChangesAsync(ct);
        return campaign;
    }

    public async Task<Campaign?> UpdateCampaignAsync(int id, Campaign campaign, CancellationToken ct = default)
    {
        var existing = await db.Campaigns.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (existing is null)
            return null;

        if (existing.Status != CampaignStatus.Draft)
            throw new InvalidOperationException("Only draft campaigns can be updated.");

        existing.Subject = campaign.Subject;
        existing.TemplateId = campaign.TemplateId;
        existing.ListId = campaign.ListId;
        existing.SendAt = campaign.SendAt;
        existing.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<CampaignStats> ScheduleSendAsync(int campaignId, CancellationToken ct = default)
    {
        var campaign = await db.Campaigns.Include(c => c.Template)
            .FirstOrDefaultAsync(c => c.Id == campaignId, ct)
            ?? throw new InvalidOperationException($"Campaign {campaignId} not found.");

        if (campaign.Status != CampaignStatus.Draft && campaign.Status != CampaignStatus.Scheduled)
            throw new InvalidOperationException($"Cannot send campaign with status {campaign.Status}.");

        var subscribers = await GetSubscribersAsync(campaign.ListId, ct);
        campaign.RecipientCount = subscribers.Count;
        campaign.Status = CampaignStatus.Sending;
        await db.SaveChangesAsync(ct);

        int sentCount = 0;

        for (int i = 0; i < subscribers.Count; i += BatchSize)
        {
            if (ct.IsCancellationRequested)
                break;

            var batch = subscribers.GetRange(i, Math.Min(BatchSize, subscribers.Count - i));

            foreach (var sub in batch)
            {
                try
                {
                    string html;
                    string subject;

                    if (campaign.Template is not null)
                    {
                        var vars = BuildVariables(sub);
                        html = templateService.RenderPreview(campaign.Template, vars);
                        subject = campaign.Subject;
                    }
                    else
                    {
                        html = $"<p>{campaign.Subject}</p>";
                        subject = campaign.Subject;
                    }

                    var messageId = await mailProvider.SendAsync(
                        new MailMessage(
                            To: sub.Email,
                            Subject: subject,
                            HtmlBody: html,
                            Tags: ["newsletter", $"campaign-{campaignId}"]),
                        ct);

                    if (messageId is not null)
                        sentCount++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send campaign {CampaignId} to {Email}", campaignId, sub.Email);
                }
            }

            // Rate limiting: pause between batches (except after the last one).
            if (i + BatchSize < subscribers.Count)
                await Task.Delay(BatchDelay, ct);
        }

        campaign.SentCount = sentCount;
        campaign.Status = CampaignStatus.Sent;
        campaign.SentAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        return await GetStatsAsync(campaignId, ct);
    }

    public async Task<CampaignStats> GetStatsAsync(int campaignId, CancellationToken ct = default)
    {
        var campaign = await db.Campaigns.FirstOrDefaultAsync(c => c.Id == campaignId, ct)
            ?? throw new InvalidOperationException($"Campaign {campaignId} not found.");

        var events = await db.EmailEvents
            .Where(e => db.EmailEvents
                .Where(inner => inner.MessageId == e.MessageId)
                .Any())
            .ToListAsync(ct);

        // Count distinct events by type from Mailgun webhook data.
        // Events are stored by MessageId; join back via tags is not needed — just aggregate by type.
        var allEvents = await db.EmailEvents
            .GroupBy(e => e.EventType)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(ct);

        int Count(EmailEventType type) => allEvents.FirstOrDefault(e => e.Key == type)?.Count ?? 0;

        return new CampaignStats(
            TotalRecipients: campaign.RecipientCount,
            SentCount: campaign.SentCount,
            DeliveredCount: Count(EmailEventType.Delivered),
            OpenedCount: Count(EmailEventType.Opened),
            ClickedCount: Count(EmailEventType.Clicked),
            BouncedCount: Count(EmailEventType.Bounced),
            UnsubscribedCount: Count(EmailEventType.Unsubscribed));
    }

    private static Dictionary<string, string> BuildVariables(Subscriber sub) => new(StringComparer.OrdinalIgnoreCase)
    {
        ["Email"] = sub.Email,
        ["FirstName"] = sub.FirstName ?? string.Empty,
        ["LastName"] = sub.LastName ?? string.Empty,
        ["FullName"] = $"{sub.FirstName} {sub.LastName}".Trim(),
        ["MemberId"] = sub.MemberId ?? string.Empty,
    };
}
