using UmbracoCms.Plugins.ToastNotifications.Models;

namespace UmbracoCms.Plugins.ToastNotifications.Services;

public interface IToastNotificationsService
{
    Task<IEnumerable<ToastMessage>> GetActiveToastsAsync();
    Task<ToastMessage?> GetByIdAsync(int id);
    Task<ToastMessage> CreateToastAsync(ToastMessage toast);
    Task<ToastMessage?> UpdateToastAsync(int id, ToastMessage toast);
    Task<bool> DeleteToastAsync(int id);
}
