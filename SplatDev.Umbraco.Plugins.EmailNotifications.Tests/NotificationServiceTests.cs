using Microsoft.EntityFrameworkCore;
using SplatDev.Umbraco.Plugins.EmailNotifications.Models;
using SplatDev.Umbraco.Plugins.EmailNotifications.Services;
using Xunit;

namespace SplatDev.Umbraco.Plugins.EmailNotifications.Tests;

public class NotificationServiceTests : IDisposable
{
    private readonly EmailNotificationsDbContext _db;
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        var options = new DbContextOptionsBuilder<EmailNotificationsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new EmailNotificationsDbContext(options);
        _service = new NotificationService(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task CreateAsync_PersistsNotification()
    {
        var notification = await _service.CreateAsync("member-1", NotificationType.Payment, "Your payment was received.");

        Assert.NotEqual(0, notification.Id);
        Assert.Equal("member-1", notification.MemberId);
        Assert.Equal(NotificationType.Payment, notification.Type);
        Assert.Equal("Your payment was received.", notification.Message);
        Assert.False(notification.IsRead);
        Assert.Null(notification.ReadAt);
    }

    [Fact]
    public async Task GetUnreadAsync_ReturnsOnlyUnread()
    {
        await _service.CreateAsync("member-2", NotificationType.System, "Unread 1");
        var n2 = await _service.CreateAsync("member-2", NotificationType.System, "Unread 2");
        await _service.MarkReadAsync(n2.Id);

        var unread = await _service.GetUnreadAsync("member-2");

        Assert.Single(unread);
        Assert.Equal("Unread 1", unread[0].Message);
    }

    [Fact]
    public async Task GetUnreadCountAsync_ReturnsCorrectCount()
    {
        await _service.CreateAsync("member-3", NotificationType.Contract, "Msg 1");
        await _service.CreateAsync("member-3", NotificationType.Contract, "Msg 2");
        await _service.CreateAsync("member-3", NotificationType.Contract, "Msg 3");

        var count = await _service.GetUnreadCountAsync("member-3");
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task MarkReadAsync_SetsIsReadAndReadAt()
    {
        var notification = await _service.CreateAsync("member-4", NotificationType.Newsletter, "Hello");

        var result = await _service.MarkReadAsync(notification.Id);

        var updated = await _db.Notifications.FindAsync(notification.Id);
        Assert.True(result);
        Assert.True(updated!.IsRead);
        Assert.NotNull(updated.ReadAt);
    }

    [Fact]
    public async Task MarkReadAsync_ReturnsFalse_ForUnknownId()
    {
        var result = await _service.MarkReadAsync(99999);
        Assert.False(result);
    }

    [Fact]
    public async Task MarkAllReadAsync_MarksAllUnreadForMember()
    {
        await _service.CreateAsync("member-5", NotificationType.System, "A");
        await _service.CreateAsync("member-5", NotificationType.System, "B");
        await _service.CreateAsync("member-5", NotificationType.System, "C");

        var count = await _service.MarkAllReadAsync("member-5");

        Assert.Equal(3, count);
        Assert.Equal(0, await _service.GetUnreadCountAsync("member-5"));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsNewestFirst()
    {
        var n1 = await _service.CreateAsync("member-6", NotificationType.System, "Old");
        var n2 = await _service.CreateAsync("member-6", NotificationType.System, "New");
        // Force n2 to have a later CreatedAt via direct mutation before save
        n1.CreatedAt = DateTime.UtcNow.AddMinutes(-5);
        n2.CreatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var all = await _service.GetAllAsync("member-6");

        Assert.Equal(2, all.Count);
        Assert.Equal("New", all[0].Message);
    }

    [Fact]
    public async Task GetAllAsync_DoesNotReturnNotificationsForOtherMembers()
    {
        await _service.CreateAsync("member-A", NotificationType.System, "For A");
        await _service.CreateAsync("member-B", NotificationType.System, "For B");

        var result = await _service.GetAllAsync("member-A");

        Assert.Single(result);
        Assert.Equal("For A", result[0].Message);
    }
}
