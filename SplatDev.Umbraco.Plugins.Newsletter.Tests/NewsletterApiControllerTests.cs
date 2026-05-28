using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SplatDev.Umbraco.Plugins.Newsletter.Controllers;
using SplatDev.Umbraco.Plugins.Newsletter.Models;
using SplatDev.Umbraco.Plugins.Newsletter.Services;

namespace SplatDev.Umbraco.Plugins.Newsletter.Tests;

public class NewsletterApiControllerTests
{
    private readonly Mock<INewsletterService> _service;
    private readonly NewsletterApiController _sut;

    public NewsletterApiControllerTests()
    {
        _service = new Mock<INewsletterService>();
        _sut = new NewsletterApiController(
            _service.Object,
            new Mock<ILogger<NewsletterApiController>>().Object);
    }

    [Fact]
    public void GetLists_ReturnsOkWithLists()
    {
        var lists = new[] { new SubscriberList { Id = 1, Name = "Test" } };
        _service.Setup(s => s.GetAllLists()).Returns(lists);

        var result = _sut.GetLists();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(lists, ok.Value);
    }

    [Fact]
    public void CreateList_ValidName_ReturnsCreated()
    {
        var req = new CreateListRequest("My List");
        var created = new SubscriberList { Id = 2, Name = "My List" };
        _service.Setup(s => s.CreateList("My List")).Returns(created);

        var result = _sut.CreateList(req);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Same(created, createdResult.Value);
    }

    [Fact]
    public void CreateList_EmptyName_ReturnsBadRequest()
    {
        var result = _sut.CreateList(new CreateListRequest(""));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void DeleteList_Exists_ReturnsNoContent()
    {
        _service.Setup(s => s.DeleteList(1)).Returns(true);

        var result = _sut.DeleteList(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void DeleteList_NotFound_ReturnsNotFound()
    {
        _service.Setup(s => s.DeleteList(99)).Returns(false);

        var result = _sut.DeleteList(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GetSubscribers_ReturnsOk()
    {
        var subs = new[] { new Subscriber { Id = 1, Email = "a@b.com", ListId = 1 } };
        _service.Setup(s => s.GetSubscribers(1)).Returns(subs);

        var result = _sut.GetSubscribers(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(subs, ok.Value);
    }

    [Fact]
    public void Subscribe_ValidEmail_ReturnsOk()
    {
        var sub = new Subscriber { Id = 3, Email = "test@example.com", ListId = 1, Active = true };
        _service.Setup(s => s.Subscribe(1, "test@example.com", null, null)).Returns(sub);

        var result = _sut.Subscribe(1, new SubscribeRequest("test@example.com"));

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(sub, ok.Value);
    }

    [Fact]
    public void Subscribe_EmptyEmail_ReturnsBadRequest()
    {
        var result = _sut.Subscribe(1, new SubscribeRequest(""));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Unsubscribe_Exists_ReturnsNoContent()
    {
        _service.Setup(s => s.Unsubscribe(1, "x@y.com")).Returns(true);

        var result = _sut.Unsubscribe(1, "x@y.com");

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void Unsubscribe_NotFound_ReturnsNotFound()
    {
        _service.Setup(s => s.Unsubscribe(1, "no@no.com")).Returns(false);

        var result = _sut.Unsubscribe(1, "no@no.com");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void DeleteSubscriber_Exists_ReturnsNoContent()
    {
        _service.Setup(s => s.DeleteSubscriber(5)).Returns(true);

        var result = _sut.DeleteSubscriber(5);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void GetCampaigns_ReturnsOk()
    {
        var campaigns = new[] { new Campaign { Id = 1, Name = "C1", Subject = "S1", Status = "Draft" } };
        _service.Setup(s => s.GetAllCampaigns()).Returns(campaigns);

        var result = _sut.GetCampaigns();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(campaigns, ok.Value);
    }

    [Fact]
    public void GetCampaign_Exists_ReturnsOk()
    {
        var campaign = new Campaign { Id = 1, Name = "C1" };
        _service.Setup(s => s.GetCampaignById(1)).Returns(campaign);

        var result = _sut.GetCampaign(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(campaign, ok.Value);
    }

    [Fact]
    public void GetCampaign_NotFound_ReturnsNotFound()
    {
        _service.Setup(s => s.GetCampaignById(99)).Returns((Campaign?)null);

        var result = _sut.GetCampaign(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void CreateCampaign_Valid_ReturnsCreated()
    {
        var campaign = new Campaign { Name = "Test", Subject = "S", ListId = 1 };
        var created = new Campaign { Id = 10, Name = "Test", Subject = "S", ListId = 1, Status = "Draft" };
        _service.Setup(s => s.Create(It.IsAny<Campaign>())).Returns(created);

        var result = _sut.CreateCampaign(campaign);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public void CreateCampaign_EmptyName_ReturnsBadRequest()
    {
        var result = _sut.CreateCampaign(new Campaign { Name = "" });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateCampaign_Exists_ReturnsOk()
    {
        var campaign = new Campaign { Name = "Updated" };
        _service.Setup(s => s.Update(1, It.IsAny<Campaign>())).Returns(campaign);

        var result = _sut.UpdateCampaign(1, campaign);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(campaign, ok.Value);
    }

    [Fact]
    public void UpdateCampaign_NotFound_ReturnsNotFound()
    {
        _service.Setup(s => s.Update(99, It.IsAny<Campaign>())).Returns((Campaign?)null);

        var result = _sut.UpdateCampaign(99, new Campaign { Name = "X" });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void DeleteCampaign_Exists_ReturnsNoContent()
    {
        _service.Setup(s => s.DeleteCampaign(1)).Returns(true);

        var result = _sut.DeleteCampaign(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void DeleteCampaign_NotFound_ReturnsNotFound()
    {
        _service.Setup(s => s.DeleteCampaign(99)).Returns(false);

        var result = _sut.DeleteCampaign(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GetStats_Exists_ReturnsOk()
    {
        var stats = new CampaignStats { Id = 1, CampaignId = 1, Delivered = 100, Opens = 50 };
        _service.Setup(s => s.GetStats(1)).Returns(stats);

        var result = _sut.GetStats(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(stats, ok.Value);
    }

    [Fact]
    public void GetStats_NotFound_ReturnsNotFound()
    {
        _service.Setup(s => s.GetStats(99)).Returns((CampaignStats?)null);

        var result = _sut.GetStats(99);

        Assert.IsType<NotFoundResult>(result);
    }
}
