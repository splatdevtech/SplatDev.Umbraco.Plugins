# SplatDev.Messaging.Newsletter

Foundation abstractions for newsletter/mailing-list plugins. **No provider implementation** — this package defines the contracts that provider packages (`SplatDev.Messaging.Newsletter.Mailchimp`, etc.) and consumer plugins (`SplatDev.Umbraco.Plugins.Newsletter`) implement and reference.

Parallel to [`SplatDev.Messaging`](../SplatDev.Messaging) but for list management + campaigns, not one-shot sends.

## Install

```bash
dotnet add package SplatDev.Messaging.Newsletter
```

Targets: `net8.0`, `net10.0`. Published to nuget.org. Only dependency is `SplatDev.Messaging`.

## Models

### `NewsletterList`

```csharp
public class NewsletterList
{
    public string  Id                { get; set; }
    public string  Name              { get; set; }
    public string? Description       { get; set; }
    public string? ProviderExternalId { get; set; }  // set by provider after remote creation
    public DateTime CreatedAt        { get; set; }
    public DateTime UpdatedAt        { get; set; }
}
```

### `NewsletterSubscriber` + `SubscriberStatus`

```csharp
public enum SubscriberStatus { Subscribed, Unsubscribed, Pending, Bounced, Complained }

public class NewsletterSubscriber
{
    public string  Id                { get; set; }
    public string  ListId            { get; set; }
    public string  Email             { get; set; }
    public string? Name              { get; set; }
    public string? FirstName         { get; set; }
    public string? LastName          { get; set; }
    public SubscriberStatus Status   { get; set; }
    public Dictionary<string,string>? CustomFields { get; set; }
    public string? ProviderExternalId { get; set; }
    public DateTime SubscribedAt     { get; set; }
    public DateTime? UnsubscribedAt  { get; set; }
    public DateTime UpdatedAt        { get; set; }
}
```

### `NewsletterCampaign` + `CampaignStatus`

```csharp
public enum CampaignStatus { Draft, Scheduled, Sending, Sent }

public class NewsletterCampaign
{
    public string  Id              { get; set; }
    public string  ListId          { get; set; }
    public string? TemplateId      { get; set; }
    public string  Subject         { get; set; }
    public string  FromName        { get; set; }
    public string  FromAddress     { get; set; }
    public string? HtmlBody        { get; set; }
    public string? PlainTextBody   { get; set; }
    public CampaignStatus Status   { get; set; }
    public bool    TrackOpens      { get; set; }
    public bool    TrackClicks     { get; set; }
    public DateTime? ScheduledAt   { get; set; }
    public DateTime? SentAt        { get; set; }
    public DateTime CreatedAt      { get; set; }
    public DateTime UpdatedAt      { get; set; }
}
```

### `CampaignStats`

```csharp
public class CampaignStats
{
    public string   CampaignId   { get; set; }
    public int      Opens        { get; set; }
    public int      Clicks       { get; set; }
    public int      Delivered    { get; set; }
    public int      Bounced      { get; set; }
    public int      Complaints   { get; set; }
    public int      Unsubscribes { get; set; }
    public DateTime FetchedAt    { get; set; }
}
```

## Interfaces

### `INewsletterProvider`

The core contract. Provider packages implement this to connect a mail-sending service.

```csharp
public interface INewsletterProvider
{
    // Lists
    Task<NewsletterList> CreateListAsync(NewsletterList list);
    Task<NewsletterList?> GetListAsync(string listId);
    Task<IEnumerable<NewsletterList>> GetListsAsync();
    Task<NewsletterList> UpdateListAsync(NewsletterList list);
    Task<bool> DeleteListAsync(string listId);

    // Subscribers
    Task<NewsletterSubscriber> SubscribeAsync(string listId, NewsletterSubscriber subscriber);
    Task<NewsletterSubscriber?> GetSubscriberAsync(string listId, string subscriberId);
    Task<IEnumerable<NewsletterSubscriber>> GetSubscribersAsync(string listId);
    Task<NewsletterSubscriber> UpdateSubscriberAsync(string listId, NewsletterSubscriber subscriber);
    Task<bool> UnsubscribeAsync(string listId, string subscriberId);
    Task<bool> DeleteSubscriberAsync(string listId, string subscriberId);

    // Campaigns
    Task<NewsletterCampaign> CreateCampaignAsync(NewsletterCampaign campaign);
    Task<NewsletterCampaign?> GetCampaignAsync(string campaignId);
    Task<IEnumerable<NewsletterCampaign>> GetCampaignsAsync(string listId);
    Task<NewsletterCampaign> UpdateCampaignAsync(NewsletterCampaign campaign);
    Task<bool> DeleteCampaignAsync(string campaignId);
    Task<NewsletterCampaign> SendCampaignAsync(string campaignId);

    // Stats
    Task<CampaignStats?> GetCampaignStatsAsync(string campaignId);
}
```

### `INewsletterEventSource`

Optional interface for providers that fire webhook-driven events.

```csharp
public interface INewsletterEventSource
{
    event EventHandler<NewsletterEventArgs>? Subscribed;
    event EventHandler<NewsletterEventArgs>? Unsubscribed;
    event EventHandler<NewsletterEventArgs>? Bounced;
    event EventHandler<NewsletterEventArgs>? Complained;
    event EventHandler<NewsletterEventArgs>? Opened;
    event EventHandler<NewsletterEventArgs>? Clicked;
    event EventHandler<NewsletterEventArgs>? CampaignSent;
}
```

## Events

### `NewsletterEventArgs`

```csharp
public class NewsletterEventArgs : EventArgs
{
    public NewsletterList?       List         { get; set; }
    public NewsletterSubscriber? Subscriber   { get; set; }
    public NewsletterCampaign?   Campaign     { get; set; }
    public CampaignStats?        Stats        { get; set; }
    public string                EventType    { get; set; }
    public DateTime              Timestamp    { get; set; }
    public string?               ProviderName { get; set; }
    public Dictionary<string,object>? RawPayload { get; set; }
}
```

## Building a Provider

1. Reference `SplatDev.Messaging.Newsletter` in your provider project.
2. Implement `INewsletterProvider` using the service's API (REST, SDK, etc.).
3. Optionally implement `INewsletterEventSource` if the service supports webhooks.
4. Handle provider-specific webhooks and translate to `NewsletterEventArgs`.
5. Register via `IServiceCollection` extension method.

## Webhook Ingest

Webhook parsing is **provider-specific** — this foundation does NOT include a unified parser or router. Each provider translates its service's webhook format into `NewsletterEventArgs` and fires the corresponding event.

---

**SplatDev.Messaging.Newsletter** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
