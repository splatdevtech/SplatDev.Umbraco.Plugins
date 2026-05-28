using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Umbraco.Cms.Core.Models;
using SplatDev.Umbraco.Plugins.MemberGroups.Controllers;
using SplatDev.Umbraco.Plugins.MemberGroups.Models;
using MGroup = SplatDev.Umbraco.Plugins.MemberGroups.Models.MemberGroup;
using SplatDev.Umbraco.Plugins.MemberGroups.Services;

namespace SplatDev.Umbraco.Plugins.MemberGroups.Tests;

public class MemberGroupsApiControllerTests
{
    private readonly Mock<IMemberGroupsService> _service;
    private readonly MemberGroupsApiController _sut;

    public MemberGroupsApiControllerTests()
    {
        _service = new Mock<IMemberGroupsService>();
        _sut = new MemberGroupsApiController(_service.Object);
    }

    [Fact]
    public void GetMemberGroups_ReturnsOk()
    {
        _service.Setup(s => s.GetMemberGroups()).Returns([
            Mock.Of<IMemberGroup>(g => g.Id == 1 && g.Name == "Admins")
        ]);

        var result = _sut.GetMemberGroups();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetMemberTypes_ReturnsOk()
    {
        _service.Setup(s => s.GetMemberTypes()).Returns([
            Mock.Of<IMemberType>(t => t.Id == 1 && t.Name == "Member" && t.Alias == "member")
        ]);

        var result = _sut.GetMemberTypes();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void AddToGroup_Valid_ReturnsOk()
    {
        var result = _sut.AddToGroup(new AddToGroupRequest("a@b.com", "Admins"));

        Assert.IsType<OkObjectResult>(result);
        _service.Verify(s => s.AddToGroup("a@b.com", "Admins"), Times.Once);
    }

    [Fact]
    public void AddToGroup_EmptyEmail_ReturnsBadRequest()
    {
        var result = _sut.AddToGroup(new AddToGroupRequest("", "Admins"));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddToGroup_EmptyGroup_ReturnsBadRequest()
    {
        var result = _sut.AddToGroup(new AddToGroupRequest("a@b.com", ""));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateGroup_Valid_ReturnsOk()
    {
        var group = new MGroup { Name = "New Group" };
        _service.Setup(s => s.CreateGroup(It.IsAny<MGroup>())).Returns(group);

        var result = _sut.CreateGroup(group);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void CreateGroup_Error_ReturnsBadRequest()
    {
        _service.Setup(s => s.CreateGroup(It.IsAny<MGroup>())).Throws(new Exception("dup"));

        var result = _sut.CreateGroup(new MGroup { Name = "X" });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void EnableUser_Valid_ReturnsOk()
    {
        var result = _sut.EnableUser("john");

        Assert.IsType<OkObjectResult>(result);
        _service.Verify(s => s.EnableUser("john"), Times.Once);
    }

    [Fact]
    public void EnableUser_Empty_ReturnsBadRequest()
    {
        var result = _sut.EnableUser("");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void DisableUser_Valid_ReturnsOk()
    {
        var result = _sut.DisableUser("john");

        Assert.IsType<OkObjectResult>(result);
        _service.Verify(s => s.DisableUser("john"), Times.Once);
    }

    [Fact]
    public void DisableUser_Empty_ReturnsBadRequest()
    {
        var result = _sut.DisableUser("");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetMemberByEmail_Valid_ReturnsOk()
    {
        var member = Mock.Of<IMember>(m =>
            m.Id == 1 && m.Name == "A" && m.Email == "a@b.com" &&
            m.Username == "a" && m.IsApproved && !m.IsLockedOut);
        _service.Setup(s => s.GetByEmail("a@b.com")).Returns(member);

        var result = _sut.GetMemberByEmail("a@b.com");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetMemberByEmail_NotFound_ReturnsNotFound()
    {
        _service.Setup(s => s.GetByEmail("no@no.com")).Returns((IMember?)null);

        var result = _sut.GetMemberByEmail("no@no.com");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void GetMemberByEmail_Empty_ReturnsBadRequest()
    {
        var result = _sut.GetMemberByEmail("");

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
