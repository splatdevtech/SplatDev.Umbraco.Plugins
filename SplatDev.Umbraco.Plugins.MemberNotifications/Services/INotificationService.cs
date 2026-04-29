using SplatDev.Umbraco.Plugins.MemberNotifications.Models;

namespace SplatDev.Umbraco.Plugins.MemberNotifications.Services;

public interface INotificationService
{
    Task<MemberNotification> CreateAsync(
        Guid memberKey, string type, string title, string body,
        string? dataJson = null, CancellationToken ct = default);

    (IReadOnlyList<MemberNotification> Items, int Total) GetPaged(
        Guid memberKey, int page, int pageSize);

    int GetUnreadCount(Guid memberKey);

    int MarkRead(Guid memberKey, IReadOnlyList<int> notificationIds);

    int MarkAllRead(Guid memberKey);
}
