using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using SplatDev.Umbraco.Plugins.Newsletters.Controllers;
using SplatDev.Umbraco.Plugins.Newsletters.Models;
using SplatDev.Umbraco.Plugins.Newsletters.Services;

namespace SplatDev.Umbraco.Plugins.Newsletters.Tests;

public class NewslettersApiControllerTests
{
    private readonly Mock<INewslettersService> _service;
    private readonly NewslettersApiController _sut;

    public NewslettersApiControllerTests()
    {
        _service = new Mock<INewslettersService>();
        _sut = new NewslettersApiController(_service.Object);
    }

    [Fact]
    public async Task Subscribe_ValidEmail_ReturnsOk()
    {
        _service.Setup(s => s.SubscribeAsync("test@example.com", "John", "Doe"))
            .ReturnsAsync(true);
        var req = new NewslettersApiController.SubscribeRequest("test@example.com", "John", "Doe");

        var result = await _sut.Subscribe(req);

        var ok = Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Subscribe_EmptyEmail_ReturnsBadRequest()
    {
        var req = new NewslettersApiController.SubscribeRequest("", null!, null!);

        var result = await _sut.Subscribe(req);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Unsubscribe_ValidEmail_ReturnsOk()
    {
        _service.Setup(s => s.UnsubscribeAsync("a@b.com")).ReturnsAsync(true);
        var req = new NewslettersApiController.UnsubscribeRequest("a@b.com");

        var result = await _sut.Unsubscribe(req);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Unsubscribe_EmptyEmail_ReturnsBadRequest()
    {
        var req = new NewslettersApiController.UnsubscribeRequest("");

        var result = await _sut.Unsubscribe(req);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetSubscribers_ReturnsOkWithList()
    {
        var subscribers = new List<NewsletterSubscriber>
        {
            new() { Id = 1, Email = "a@b.com", FirstName = "A" }
        };
        _service.Setup(s => s.GetSubscribersAsync()).ReturnsAsync(subscribers);

        var result = await _sut.GetSubscribers();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(subscribers, ok.Value);
    }

    [Fact]
    public async Task GetCampaigns_ReturnsOkWithList()
    {
        var campaigns = new List<NewsletterCampaign>
        {
            new() { Id = 1, Subject = "Test", Status = CampaignStatus.Draft }
        };
        _service.Setup(s => s.GetCampaignsAsync()).ReturnsAsync(campaigns);

        var result = await _sut.GetCampaigns();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(campaigns, ok.Value);
    }

    [Fact]
    public async Task SendCampaign_Valid_ReturnsOk()
    {
        _service.Setup(s => s.SendCampaignAsync(1)).Returns(Task.CompletedTask);

        var result = await _sut.SendCampaign(new NewslettersApiController.SendCampaignRequest(1));

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task SendCampaign_NotFound_ReturnsBadRequest()
    {
        _service.Setup(s => s.SendCampaignAsync(99))
            .ThrowsAsync(new InvalidOperationException("Campaign 99 not found."));

        var result = await _sut.SendCampaign(new NewslettersApiController.SendCampaignRequest(99));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateCampaign_Valid_ReturnsOk()
    {
        var campaign = new NewsletterCampaign { Subject = "Hello World" };

        var result = await _sut.CreateCampaign(campaign);

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<NewsletterCampaign>(ok.Value);
        Assert.Equal(CampaignStatus.Draft, returned.Status);
        Assert.Equal("Hello World", returned.Subject);
    }

    [Fact]
    public async Task CreateCampaign_EmptySubject_ReturnsBadRequest()
    {
        var campaign = new NewsletterCampaign { Subject = "" };

        var result = await _sut.CreateCampaign(campaign);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
