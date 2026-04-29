using NPoco;
using Umbraco.Cms.Infrastructure.Scoping;
using SplatDev.Umbraco.Plugins.MemberNotifications.Models;

namespace SplatDev.Umbraco.Plugins.MemberNotifications.Services;

public class NotificationService(IScopeProvider scopeProvider) : INotificationService
{
    private const string T = MemberNotification.TableName;

    public Task<MemberNotification> CreateAsync(
        Guid memberKey, string type, string title, string body,
        string? dataJson = null, CancellationToken ct = default)
    {
        var notification = new MemberNotification
        {
            MemberKey = memberKey,
            Type = type,
            Title = title,
            Body = body,
            DataJson = dataJson,
            CreatedAt = DateTime.UtcNow,
        };

        using var scope = scopeProvider.CreateScope();
        var id = Convert.ToInt32(scope.Database.Insert(notification));
        notification.Id = id;
        scope.Complete();

        return Task.FromResult(notification);
    }

    public (IReadOnlyList<MemberNotification> Items, int Total) GetPaged(
        Guid memberKey, int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        using var scope = scopeProvider.CreateScope(autoComplete: true);

        var total = scope.Database.ExecuteScalar<int>(
            new Sql($"SELECT COUNT(*) FROM [{T}] WHERE [member_key]=@0", memberKey));

        var offset = (page - 1) * pageSize;
        var items = scope.Database.Fetch<MemberNotification>(
            new Sql($"""
                SELECT * FROM [{T}]
                WHERE [member_key]=@0
                ORDER BY [created_at] DESC
                OFFSET @1 ROWS FETCH NEXT @2 ROWS ONLY
                """, memberKey, offset, pageSize));

        return (items, total);
    }

    public int GetUnreadCount(Guid memberKey)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        return scope.Database.ExecuteScalar<int>(
            new Sql($"SELECT COUNT(*) FROM [{T}] WHERE [member_key]=@0 AND [read_at] IS NULL", memberKey));
    }

    public int MarkRead(Guid memberKey, IReadOnlyList<int> notificationIds)
    {
        if (notificationIds.Count == 0)
            return 0;

        // IDs are typed as int — safe to inline in SQL
        var idList = string.Join(",", notificationIds);

        using var scope = scopeProvider.CreateScope();
        var rows = scope.Database.Execute(
            new Sql(
                $"UPDATE [{T}] SET [read_at]=GETUTCDATE() WHERE [member_key]=@0 AND [id] IN ({idList}) AND [read_at] IS NULL",
                memberKey));
        scope.Complete();
        return rows;
    }

    public int MarkAllRead(Guid memberKey)
    {
        using var scope = scopeProvider.CreateScope();
        var rows = scope.Database.Execute(
            new Sql($"UPDATE [{T}] SET [read_at]=GETUTCDATE() WHERE [member_key]=@0 AND [read_at] IS NULL", memberKey));
        scope.Complete();
        return rows;
    }
}
