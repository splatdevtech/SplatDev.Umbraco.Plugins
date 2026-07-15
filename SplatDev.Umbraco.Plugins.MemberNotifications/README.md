# SplatDev.Umbraco.Plugins.MemberNotifications

In-app notification system for Umbraco members (Umbraco 17+). Stores per-member notifications in a database table with read/unread tracking, exposes a member-authorised API for listing, unread counts, and marking as read, and auto-creates the database table via Umbraco migrations on startup.

## Install

```bash
dotnet add package SplatDev.Umbraco.Plugins.MemberNotifications
```

Targets `net10.0` (Umbraco 17). Published to nuget.org.

## What's implemented

### Database table

Automatically created on startup via an Umbraco migration (`CreateNotificationsTable`):

| Column | Type | Description |
|--------|------|-------------|
| `id` | `int` (PK, auto-increment) | Primary key |
| `member_key` | `Guid` | Umbraco member key |
| `type` | `nvarchar(50)` | Category: `system`, `contract`, `payment`, `newsletter` |
| `title` | `nvarchar(300)` | Notification title |
| `body` | `nvarchar(max)` | Notification body |
| `read_at` | `datetime` (nullable) | `null` = unread |
| `created_at` | `datetime` | Defaults to UTC now |
| `data_json` | `nvarchar(max)` (nullable) | Optional JSON payload |

Table name: `splatdev_notifications`. Migration is idempotent — skips if the table already exists.

### `INotificationService`

| Method | Description |
|--------|-------------|
| `CreateAsync(memberKey, type, title, body, dataJson)` | Create and persist a notification |
| `GetPaged(memberKey, page, pageSize)` | Paginated, newest-first list + total count |
| `GetUnreadCount(memberKey)` | Count of unread notifications |
| `MarkRead(memberKey, notificationIds)` | Mark specific notifications as read |
| `MarkAllRead(memberKey)` | Mark all for the member as read |

### Member API (`MemberNotificationsController`)

Authenticated via `[UmbracoMemberAuthorize]`. All endpoints require a logged-in member.

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `api/member/notifications?page=1&pageSize=20` | Paginated notification list |
| `GET` | `api/member/notifications/unread-count` | Unread badge count |
| `POST` | `api/member/notifications/mark-read` | Mark read (body: `{ ids: [1, 2] }` or `null` to mark all) |

Response shape:

```json
// GET api/member/notifications
{
  "items": [
    {
      "id": 1,
      "type": "payment",
      "title": "Payment received",
      "body": "Your payment of $49.99 was processed.",
      "createdAt": "2026-07-09T12:00:00Z",
      "readAt": null
    }
  ],
  "total": 1,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1
}

// POST api/member/notifications/mark-read  { "ids": null }
{ "marked": 5 }
```

### Creating notifications from application code

```csharp
var svc = serviceProvider.GetRequiredService<INotificationService>();
await svc.CreateAsync(
    memberKey: member.Key,
    type: "payment",
    title: "Invoice paid",
    body: "Invoice #1042 has been paid.",
    dataJson: "{\"invoiceId\":1042}");
```

## DI Registration

Automatic via `MemberNotificationsComposer : IComposer`. Registers:

| Service | Lifetime |
|---------|----------|
| `INotificationService` → `NotificationService` | Scoped |

The startup component (`MemberNotificationsComponent`) runs the migration on first boot. No manual setup required.

## Dependencies

| Package | Purpose |
|---------|---------|
| `Umbraco.Cms.Core` (17.3.4) | `IScopeProvider`, `IMemberManager`, composers, migrations |
| `Umbraco.Cms.Infrastructure` (17.3.4) | NPoco database access, migration infrastructure |
| `Umbraco.Cms.Web.Common` (17.3.4) | Web hosting and API controllers |

---

**SplatDev.Umbraco.Plugins.MemberNotifications** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
