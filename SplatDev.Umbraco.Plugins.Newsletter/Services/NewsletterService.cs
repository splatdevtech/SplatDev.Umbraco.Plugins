using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Infrastructure.Scoping;
using SplatDev.Umbraco.Plugins.Newsletter.Models;
using SplatDev.Umbraco.Plugins.EmailTemplates.Services;

namespace SplatDev.Umbraco.Plugins.Newsletter.Services;

public class NewsletterService(
    IScopeProvider scopeProvider,
    IHttpClientFactory httpClientFactory,
    IEmailTemplateService templateService,
    IEmailStyleService styleService,
    IConfiguration config,
    ILogger<NewsletterService> logger) : INewsletterService
{
    // ── Subscriber lists ────────────────────────────────────────────────────────

    public IReadOnlyList<SubscriberList> GetAllLists()
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        return scope.Database.Fetch<SubscriberList>(
            new Sql($"SELECT * FROM [{SubscriberList.TableName}] ORDER BY [name]"));
    }

    public SubscriberList? GetListById(int id)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        return scope.Database.SingleOrDefault<SubscriberList>(
            new Sql($"SELECT * FROM [{SubscriberList.TableName}] WHERE [id]=@0", id));
    }

    public SubscriberList CreateList(string name)
    {
        var list = new SubscriberList { Name = name, CreatedAt = DateTime.UtcNow };
        using var scope = scopeProvider.CreateScope();
        var id = Convert.ToInt32(scope.Database.Insert(list));
        list.Id = id;
        scope.Complete();
        return list;
    }

    public bool DeleteList(int id)
    {
        using var scope = scopeProvider.CreateScope();
        var rows = scope.Database.Execute(
            new Sql($"DELETE FROM [{SubscriberList.TableName}] WHERE [id]=@0", id));
        scope.Complete();
        return rows > 0;
    }

    // ── Subscribers ─────────────────────────────────────────────────────────────

    public IReadOnlyList<Subscriber> GetSubscribers(int listId)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        return scope.Database.Fetch<Subscriber>(
            new Sql($"SELECT * FROM [{Subscriber.TableName}] WHERE [list_id]=@0 ORDER BY [email]", listId));
    }

    public Subscriber? Subscribe(int listId, string email, string? name = null, Guid? memberKey = null)
    {
        using var scope = scopeProvider.CreateScope();
        var existing = scope.Database.SingleOrDefault<Subscriber>(
            new Sql($"SELECT * FROM [{Subscriber.TableName}] WHERE [list_id]=@0 AND [email]=@1", listId, email.ToLowerInvariant()));

        if (existing is not null)
        {
            if (!existing.Active)
            {
                existing.Active = true;
                existing.UnsubscribedAt = null;
                scope.Database.Update(existing);
                scope.Complete();
            }
            return existing;
        }

        var subscriber = new Subscriber
        {
            ListId = listId,
            Email = email.ToLowerInvariant(),
            Name = name,
            MemberKey = memberKey,
            Active = true,
            SubscribedAt = DateTime.UtcNow,
        };

        var id = Convert.ToInt32(scope.Database.Insert(subscriber));
        subscriber.Id = id;
        scope.Complete();
        return subscriber;
    }

    public bool Unsubscribe(int listId, string email)
    {
        using var scope = scopeProvider.CreateScope();
        var subscriber = scope.Database.SingleOrDefault<Subscriber>(
            new Sql($"SELECT * FROM [{Subscriber.TableName}] WHERE [list_id]=@0 AND [email]=@1", listId, email.ToLowerInvariant()));

        if (subscriber is null || !subscriber.Active)
        {
            scope.Complete();
            return false;
        }

        subscriber.Active = false;
        subscriber.UnsubscribedAt = DateTime.UtcNow;
        scope.Database.Update(subscriber);
        scope.Complete();
        return true;
    }

    public bool DeleteSubscriber(int id)
    {
        using var scope = scopeProvider.CreateScope();
        var rows = scope.Database.Execute(
            new Sql($"DELETE FROM [{Subscriber.TableName}] WHERE [id]=@0", id));
        scope.Complete();
        return rows > 0;
    }

    // ── Campaigns ───────────────────────────────────────────────────────────────

    public IReadOnlyList<Campaign> GetAllCampaigns()
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        return scope.Database.Fetch<Campaign>(
            new Sql($"SELECT * FROM [{Campaign.TableName}] ORDER BY [created_at] DESC"));
    }

    public Campaign? GetCampaignById(int id)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        return scope.Database.SingleOrDefault<Campaign>(
            new Sql($"SELECT * FROM [{Campaign.TableName}] WHERE [id]=@0", id));
    }

    public Campaign Create(Campaign campaign)
    {
        campaign.Id = 0;
        campaign.Status = nameof(CampaignStatus.Draft);
        campaign.CreatedAt = DateTime.UtcNow;

        using var scope = scopeProvider.CreateScope();
        var id = Convert.ToInt32(scope.Database.Insert(campaign));
        campaign.Id = id;
        scope.Complete();
        return campaign;
    }

    public Campaign? Update(int id, Campaign campaign)
    {
        using var scope = scopeProvider.CreateScope();
        var existing = scope.Database.SingleOrDefault<Campaign>(
            new Sql($"SELECT * FROM [{Campaign.TableName}] WHERE [id]=@0", id));

        if (existing is null)
            return null;

        existing.Name = campaign.Name;
        existing.TemplateId = campaign.TemplateId;
        existing.ListId = campaign.ListId;
        existing.Subject = campaign.Subject;
        existing.Status = campaign.Status;
        existing.ScheduledAt = campaign.ScheduledAt;

        scope.Database.Update(existing);
        scope.Complete();
        return existing;
    }

    public bool DeleteCampaign(int id)
    {
        using var scope = scopeProvider.CreateScope();
        var rows = scope.Database.Execute(
            new Sql($"DELETE FROM [{Campaign.TableName}] WHERE [id]=@0", id));
        scope.Complete();
        return rows > 0;
    }

    // ── Send ────────────────────────────────────────────────────────────────────

    public async Task<int> SendCampaignAsync(int campaignId, CancellationToken ct = default)
    {
        var campaign = GetCampaignById(campaignId)
            ?? throw new InvalidOperationException($"Campaign {campaignId} not found.");

        if (campaign.Status == nameof(CampaignStatus.Sent))
            throw new InvalidOperationException("Campaign already sent.");

        var subscribers = GetSubscribers(campaign.ListId)
            .Where(s => s.Active)
            .ToList();

        if (subscribers.Count == 0)
        {
            logger.LogWarning("Campaign {CampaignId} has no active subscribers.", campaignId);
            return 0;
        }

        // Build body from template or use subject directly as plain text fallback
        string htmlBody;
        if (campaign.TemplateId.HasValue)
        {
            var template = templateService.GetById(campaign.TemplateId.Value);
            if (template is null)
                throw new InvalidOperationException($"Email template {campaign.TemplateId} not found.");
            var style = styleService.Get();
            htmlBody = templateService.RenderPreview(template, style);
        }
        else
        {
            htmlBody = $"<p>{campaign.Subject}</p>";
        }

        // Mark as sending
        UpdateStatus(campaignId, nameof(CampaignStatus.Sending));

        var http = httpClientFactory.CreateClient("NewsletterMailgun");
        var domain = config["Newsletter:Mailgun:Domain"] ?? string.Empty;
        var from = config["Newsletter:Mailgun:From"] ?? "newsletter@example.com";
        var subject = campaign.Subject;

        int sent = 0;

        // Mailgun supports up to 1000 recipients per batch; send in pages of 200
        const int batchSize = 200;
        for (int i = 0; i < subscribers.Count; i += batchSize)
        {
            ct.ThrowIfCancellationRequested();

            var batch = subscribers.Skip(i).Take(batchSize).ToList();
            var toList = string.Join(",", batch.Select(s =>
                string.IsNullOrWhiteSpace(s.Name) ? s.Email : $"{s.Name} <{s.Email}>"));

            var form = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("from", from),
                new KeyValuePair<string, string>("to", toList),
                new KeyValuePair<string, string>("subject", subject),
                new KeyValuePair<string, string>("html", htmlBody),
            ]);

            try
            {
                var response = await http.PostAsync($"/v3/{domain}/messages", form, ct);
                if (response.IsSuccessStatusCode)
                    sent += batch.Count;
                else
                {
                    var err = await response.Content.ReadAsStringAsync(ct);
                    logger.LogError("Mailgun batch {Batch} failed: {Error}", i / batchSize, err);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mailgun batch {Batch} threw exception.", i / batchSize);
            }
        }

        UpdateStatus(campaignId, nameof(CampaignStatus.Sent), DateTime.UtcNow);
        logger.LogInformation("Campaign {CampaignId} sent to {Count} subscribers.", campaignId, sent);
        return sent;
    }

    // ── Stats ───────────────────────────────────────────────────────────────────

    public CampaignStats? GetStats(int campaignId)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        return scope.Database.SingleOrDefault<CampaignStats>(
            new Sql($"SELECT * FROM [{CampaignStats.TableName}] WHERE [campaign_id]=@0", campaignId));
    }

    public async Task<CampaignStats?> FetchStatsFromMailgunAsync(int campaignId, CancellationToken ct = default)
    {
        var campaign = GetCampaignById(campaignId);
        if (campaign is null || campaign.SentAt is null)
            return null;

        var http = httpClientFactory.CreateClient("NewsletterMailgun");
        var domain = config["Newsletter:Mailgun:Domain"] ?? string.Empty;

        try
        {
            // Mailgun stats endpoint: GET /v3/{domain}/stats/total?event=delivered&event=opened&event=clicked&event=failed
            var url = $"/v3/{domain}/stats/total?event=delivered&event=opened&event=clicked&event=failed";
            var response = await http.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<MailgunStatsResponse>(cancellationToken: ct);
            if (result is null)
                return null;

            var stats = new CampaignStats
            {
                CampaignId = campaignId,
                Delivered = result.Stats?.FirstOrDefault()?.Delivered?.Total ?? 0,
                Opens = result.Stats?.FirstOrDefault()?.Opened?.Total ?? 0,
                Clicks = result.Stats?.FirstOrDefault()?.Clicked?.Total ?? 0,
                Bounced = result.Stats?.FirstOrDefault()?.Failed?.Permanent?.Total ?? 0,
                FetchedAt = DateTime.UtcNow,
            };

            UpsertStats(stats);
            return stats;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch Mailgun stats for campaign {CampaignId}.", campaignId);
            return null;
        }
    }

    // ── Private helpers ─────────────────────────────────────────────────────────

    private void UpdateStatus(int campaignId, string status, DateTime? sentAt = null)
    {
        using var scope = scopeProvider.CreateScope();
        if (sentAt.HasValue)
            scope.Database.Execute(new Sql(
                $"UPDATE [{Campaign.TableName}] SET [status]=@0, [sent_at]=@1 WHERE [id]=@2",
                status, sentAt, campaignId));
        else
            scope.Database.Execute(new Sql(
                $"UPDATE [{Campaign.TableName}] SET [status]=@0 WHERE [id]=@1", status, campaignId));
        scope.Complete();
    }

    private void UpsertStats(CampaignStats stats)
    {
        using var scope = scopeProvider.CreateScope();
        var existing = scope.Database.SingleOrDefault<CampaignStats>(
            new Sql($"SELECT * FROM [{CampaignStats.TableName}] WHERE [campaign_id]=@0", stats.CampaignId));

        if (existing is null)
        {
            scope.Database.Insert(stats);
        }
        else
        {
            existing.Opens = stats.Opens;
            existing.Clicks = stats.Clicks;
            existing.Delivered = stats.Delivered;
            existing.Bounced = stats.Bounced;
            existing.FetchedAt = stats.FetchedAt;
            scope.Database.Update(existing);
        }

        scope.Complete();
    }

    // ── Mailgun response DTOs ────────────────────────────────────────────────────

    private sealed class MailgunStatsResponse
    {
        [JsonPropertyName("stats")]
        public List<MailgunStatItem>? Stats { get; set; }
    }

    private sealed class MailgunStatItem
    {
        [JsonPropertyName("delivered")]
        public MailgunCount? Delivered { get; set; }

        [JsonPropertyName("opened")]
        public MailgunCount? Opened { get; set; }

        [JsonPropertyName("clicked")]
        public MailgunCount? Clicked { get; set; }

        [JsonPropertyName("failed")]
        public MailgunFailed? Failed { get; set; }
    }

    private sealed class MailgunCount
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    private sealed class MailgunFailed
    {
        [JsonPropertyName("permanent")]
        public MailgunCount? Permanent { get; set; }
    }
}
