# SplatDev.Umbraco.Plugins.Newsletter

Newsletter subscriber lists, campaigns, Mailgun bulk send, and stats tracking for Umbraco 17 (net10.0).

## Architecture

| Layer | Implementation |
|-------|---------------|
| Data | NPoco via `IScopeProvider`, 4 tables: `splatdev_newsletter_lists`, `splatdev_newsletter_subscribers`, `splatdev_newsletter_campaigns`, `splatdev_newsletter_stats` |
| Controller | `ManagementApiControllerBase`, route prefix `/umbraco/management/api/v1/newsletter` |
| Email Delivery | Mailgun via `HttpClientFactory` with Basic auth |
| Composer | `IComposer` + `ComponentComposer<T>` — DI registration + HTTP client setup |
| Backoffice | Lit 3 dashboards (Subscribers, Campaigns, Analytics) built with Vite |
| Dependencies | `SplatDev.Umbraco.Plugins.EmailTemplates` for template rendering |

## Quick Start

1. Reference the plugin in your Umbraco 17 host project.
2. Configure Mailgun in `appsettings.json`:

```json
{
  "Newsletter": {
    "Mailgun": {
      "BaseUrl": "https://api.mailgun.net",
      "ApiKey": "key-xxxxxxxxxxxxxxxx",
      "Domain": "mg.example.com",
      "From": "newsletter@example.com"
    }
  }
}
```

3. Build and run. The migration (`CreateNewsletterTables`) runs on startup.
4. Navigate to the Newsletter section in the Umbraco backoffice.
5. Build the Lit dashboards: `cd client && npm install && npm run build`

## REST API

All endpoints under `/umbraco/management/api/v1/newsletter`:

### Subscriber Lists

| Method | Path | Description |
|--------|------|-------------|
| GET | `/lists` | List all subscriber lists |
| POST | `/lists` | Create a list (body: `{ "name": "..." }`) |
| DELETE | `/lists/{id}` | Delete a list |

### Subscribers

| Method | Path | Description |
|--------|------|-------------|
| GET | `/lists/{listId}/subscribers` | List subscribers in a list |
| POST | `/lists/{listId}/subscribers` | Subscribe (body: `{ "email": "...", "name": "?" }`) |
| DELETE | `/lists/{listId}/subscribers/{email}` | Unsubscribe by email |
| DELETE | `/subscribers/{id}` | Permanently delete a subscriber |

### Campaigns

| Method | Path | Description |
|--------|------|-------------|
| GET | `/campaigns` | List all campaigns |
| GET | `/campaigns/{id}` | Get campaign by ID |
| POST | `/campaigns` | Create campaign |
| PUT | `/campaigns/{id}` | Update campaign |
| DELETE | `/campaigns/{id}` | Delete campaign |
| POST | `/campaigns/{id}/send` | Send campaign to all active subscribers |

### Stats

| Method | Path | Description |
|--------|------|-------------|
| GET | `/campaigns/{id}/stats` | Get cached campaign stats |
| POST | `/campaigns/{id}/stats/fetch` | Fetch stats from Mailgun API |

## Database Tables

| Table | Purpose |
|-------|---------|
| `splatdev_newsletter_lists` | Subscriber mailing lists |
| `splatdev_newsletter_subscribers` | Contacts with email, optional name and Umbraco member key |
| `splatdev_newsletter_campaigns` | Draft → Scheduled → Sending → Sent campaigns |
| `splatdev_newsletter_stats` | Per-campaign delivery/open/click/bounce counters |

## Backoffice Dashboards

### Subscribers Dashboard (`/umbraco#/newsletter/subscribers`)
- List selector (dropdown)
- Create/delete lists
- Add/remove subscribers via email
- Active/inactive status badges

### Campaigns Dashboard (`/umbraco#/newsletter/campaigns`)
- Create campaigns (name, subject, list)
- Send campaigns via Mailgun (200/batch)
- Delete campaigns
- View delivery/open/click/bounce stats inline

### Analytics Dashboard (`/umbraco#/newsletter/analytics`)
- Select a sent campaign
- View delivery, opens, clicks, bounces with percentages
- Fetch fresh stats from Mailgun API

## Extension Points

- `INewsletterService` — swap the implementation via DI (default: NPoco + Mailgun)
- Configure Mailgun `HttpClient` via `IHttpClientFactory`
- Email rendering delegated to `SplatDev.Umbraco.Plugins.EmailTemplates`

## Testing

```bash
# Build
dotnet build

# Unit tests
dotnet test tests/SplatDev.Umbraco.Plugins.Newsletter.Tests/

# Client dashboard build
cd client && npm install && npm run build
```

## License

MIT. Copyright SplatDev Ltda.
