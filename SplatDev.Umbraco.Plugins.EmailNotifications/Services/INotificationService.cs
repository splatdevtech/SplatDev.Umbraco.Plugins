using SplatDev.Umbraco.Plugins.EmailNotifications.Models;

namespace SplatDev.Umbraco.Plugins.EmailNotifications.Services;

public interface INotificationService
{
    Task<Notification> CreateAsync(string memberId, NotificationType type, string message,
        CancellationToken ct = default);

    Task<List<Notification>> GetAllAsync(string memberId, CancellationToken ct = default);

    Task<List<Notification>> GetUnreadAsync(string memberId, CancellationToken ct = default);

    Task<int> GetUnreadCountAsync(string memberId, CancellationToken ct = default);

    Task<bool> MarkReadAsync(int notificationId, CancellationToken ct = default);

    Task<int> MarkAllReadAsync(string memberId, CancellationToken ct = default);
}
