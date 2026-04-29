using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SplatDev.Umbraco.Plugins.EmailNotifications.Models;
using SplatDev.Umbraco.Plugins.EmailNotifications.Services;
using Xunit;

namespace SplatDev.Umbraco.Plugins.EmailNotifications.Tests;

public class NewsletterServiceTests : IDisposable
{
    private readonly EmailNotificationsDbContext _db;
    private readonly Mock<IMailProvider> _mailProviderMock;
    private readonly EmailTemplateService _templateService;
    private readonly NewsletterService _service;

    public NewsletterServiceTests()
    {
        var options = new DbContextOptionsBuilder<EmailNotificationsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new EmailNotificationsDbContext(options);
        _templateService = new EmailTemplateService(_db);
        _mailProviderMock = new Mock<IMailProvider>();
        _service = new NewsletterService(_db, _templateService, _mailProviderMock.Object,
            NullLogger<NewsletterService>.Instance);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task SubscribeAsync_CreatesNewSubscriber()
    {
        var sub = await _service.SubscribeAsync("user@example.com", "list-1", memberId: "m1",
            firstName: "Alice", lastName: "Smith");

        Assert.NotNull(sub);
        Assert.Equal("user@example.com", sub!.Email);
        Assert.Equal("list-1", sub.ListId);
        Assert.True(sub.OptedIn);
        Assert.Null(sub.OptedOutAt);
    }

    [Fact]
    public async Task SubscribeAsync_ReturnsNull_ForBlankEmail()
    {
        var sub = await _service.SubscribeAsync(string.Empty);
        Assert.Null(sub);
    }

    [Fact]
    public async Task SubscribeAsync_ResubscribesOptedOutSubscriber()
    {
        var first = await _service.SubscribeAsync("user@example.com");
        await _service.UnsubscribeAsync("user@example.com");

        var resubscribed = await _service.SubscribeAsync("user@example.com");

        Assert.NotNull(resubscribed);
        Assert.True(resubscribed!.OptedIn);
        Assert.Null(resubscribed.OptedOutAt);
    }

    [Fact]
    public async Task UnsubscribeAsync_SetsOptedOutAtAndOptedIn()
    {
        await _service.SubscribeAsync("leave@example.com");

        var result = await _service.UnsubscribeAsync("leave@example.com");

        Assert.True(result);
        var sub = await _db.Subscribers.FirstOrDefaultAsync(s => s.Email == "leave@example.com");
        Assert.NotNull(sub);
        Assert.False(sub!.OptedIn);
        Assert.NotNull(sub.OptedOutAt);
    }

    [Fact]
    public async Task UnsubscribeAsync_ReturnsFalse_ForUnknownEmail()
    {
        var result = await _service.UnsubscribeAsync("nobody@example.com");
        Assert.False(result);
    }

    [Fact]
    public async Task GetSubscribersAsync_ExcludesOptedOut()
    {
        await _service.SubscribeAsync("active@example.com");
        await _service.SubscribeAsync("inactive@example.com");
        await _service.UnsubscribeAsync("inactive@example.com");

        var subs = await _service.GetSubscribersAsync();

        Assert.Single(subs);
        Assert.Equal("active@example.com", subs[0].Email);
    }

    [Fact]
    public async Task CreateCampaignAsync_StartsAsDraft()
    {
        var campaign = new Campaign { Subject = "Test Campaign" };
        var created = await _service.CreateCampaignAsync(campaign);

        Assert.NotEqual(0, created.Id);
        Assert.Equal(CampaignStatus.Draft, created.Status);
        Assert.Equal(0, created.SentCount);
    }

    [Fact]
    public async Task UpdateCampaignAsync_ThrowsForNonDraftCampaign()
    {
        var campaign = await _service.CreateCampaignAsync(new Campaign { Subject = "Original" });

        // Force the campaign to Sent status
        campaign.Status = CampaignStatus.Sent;
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdateCampaignAsync(campaign.Id, new Campaign { Subject = "Updated" }));
    }

    [Fact]
    public async Task ScheduleSendAsync_SendsToAllOptedInSubscribers()
    {
        await _service.SubscribeAsync("a@example.com");
        await _service.SubscribeAsync("b@example.com");
        await _service.SubscribeAsync("c@example.com");

        _mailProviderMock.Setup(m => m.SendAsync(It.IsAny<MailMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("<msg-id@mailgun.org>");

        var campaign = await _service.CreateCampaignAsync(new Campaign { Subject = "Hello Subscribers" });
        var stats = await _service.ScheduleSendAsync(campaign.Id);

        Assert.Equal(3, stats.TotalRecipients);
        Assert.Equal(3, stats.SentCount);
        _mailProviderMock.Verify(m => m.SendAsync(It.IsAny<MailMessage>(), It.IsAny<CancellationToken>()),
            Times.Exactly(3));
    }

    [Fact]
    public async Task ScheduleSendAsync_CountsPartialFailures()
    {
        await _service.SubscribeAsync("good@example.com");
        await _service.SubscribeAsync("bad@example.com");

        _mailProviderMock
            .Setup(m => m.SendAsync(It.Is<MailMessage>(msg => msg.To == "good@example.com"), It.IsAny<CancellationToken>()))
            .ReturnsAsync("<msg-id@mailgun.org>");
        _mailProviderMock
            .Setup(m => m.SendAsync(It.Is<MailMessage>(msg => msg.To == "bad@example.com"), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var campaign = await _service.CreateCampaignAsync(new Campaign { Subject = "Partial" });
        var stats = await _service.ScheduleSendAsync(campaign.Id);

        Assert.Equal(2, stats.TotalRecipients);
        Assert.Equal(1, stats.SentCount);
    }

    [Fact]
    public async Task ScheduleSendAsync_ThrowsForUnknownCampaign()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ScheduleSendAsync(99999));
    }
}
