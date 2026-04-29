using Microsoft.EntityFrameworkCore;
using SplatDev.Umbraco.Plugins.EmailNotifications.Models;

namespace SplatDev.Umbraco.Plugins.EmailNotifications.Services;

public class NotificationService(EmailNotificationsDbContext db) : INotificationService
{
    public async Task<Notification> CreateAsync(string memberId, NotificationType type, string message,
        CancellationToken ct = default)
    {
        var notification = new Notification
        {
            MemberId = memberId,
            Type = type,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
        };

        db.Notifications.Add(notification);
        await db.SaveChangesAsync(ct);
        return notification;
    }

    public Task<List<Notification>> GetAllAsync(string memberId, CancellationToken ct = default) =>
        db.Notifications
            .Where(n => n.MemberId == memberId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);

    public Task<List<Notification>> GetUnreadAsync(string memberId, CancellationToken ct = default) =>
        db.Notifications
            .Where(n => n.MemberId == memberId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);

    public Task<int> GetUnreadCountAsync(string memberId, CancellationToken ct = default) =>
        db.Notifications.CountAsync(n => n.MemberId == memberId && !n.IsRead, ct);

    public async Task<bool> MarkReadAsync(int notificationId, CancellationToken ct = default)
    {
        var notification = await db.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId, ct);
        if (notification is null || notification.IsRead)
            return false;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<int> MarkAllReadAsync(string memberId, CancellationToken ct = default)
    {
        var unread = await db.Notifications
            .Where(n => n.MemberId == memberId && !n.IsRead)
            .ToListAsync(ct);

        if (unread.Count == 0)
            return 0;

        var now = DateTime.UtcNow;
        foreach (var n in unread)
        {
            n.IsRead = true;
            n.ReadAt = now;
        }

        await db.SaveChangesAsync(ct);
        return unread.Count;
    }
}
