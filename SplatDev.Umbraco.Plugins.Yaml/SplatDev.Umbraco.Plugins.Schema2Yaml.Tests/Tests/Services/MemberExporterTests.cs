using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Configuration;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class MemberExporterTests
{
    private readonly Mock<IMemberService> _mockMemberService;
    private readonly Mock<ILogger<MemberExporter>> _mockLogger;

    public MemberExporterTests()
    {
        _mockMemberService = new Mock<IMemberService>();
        _mockLogger = new Mock<ILogger<MemberExporter>>();
    }

    private MemberExporter CreateSut(bool includeMembers = true) =>
        new(_mockMemberService.Object,
            Options.Create(new Schema2YamlOptions { IncludeMembers = includeMembers }),
            _mockLogger.Object);

    [Fact]
    public async Task ExportAsync_WhenIncludeMembersIsFalse_ReturnsEmptyList()
    {
        var sut = CreateSut(includeMembers: false);

        var result = await sut.ExportAsync();

        Assert.Empty(result);
        _mockMemberService.Verify(s => s.GetAll(It.IsAny<int>(), It.IsAny<int>(), out It.Ref<long>.IsAny), Times.Never);
    }

    [Fact]
    public async Task ExportAsync_WhenNoMembers_ReturnsEmptyList()
    {
        var total = 0L;
        _mockMemberService.Setup(s => s.GetAll(0, int.MaxValue, out total)).Returns([]);

        var sut = CreateSut();
        var result = await sut.ExportAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task ExportAsync_MapsMemberFields()
    {
        var mockMemberType = new Mock<ISimpleContentType>();
        mockMemberType.Setup(mt => mt.Alias).Returns("Member");

        var mockMember = new Mock<IMember>();
        mockMember.Setup(m => m.Name).Returns("John Doe");
        mockMember.Setup(m => m.Email).Returns("john@example.com");
        mockMember.Setup(m => m.Username).Returns("johndoe");
        mockMember.Setup(m => m.ContentType).Returns(mockMemberType.Object);
        mockMember.Setup(m => m.IsApproved).Returns(true);
        mockMember.Setup(m => m.Properties).Returns(new PropertyCollection([]));

        var total = 1L;
        _mockMemberService.Setup(s => s.GetAll(0, int.MaxValue, out total))
            .Returns([mockMember.Object]);

        var sut = CreateSut();
        var result = await sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal("John Doe", result[0].Name);
        Assert.Equal("john@example.com", result[0].Email);
        Assert.Equal("johndoe", result[0].Username);
        Assert.Equal("Member", result[0].MemberType);
        Assert.True(result[0].IsApproved);
    }

    [Fact]
    public async Task ExportAsync_DoesNotExportPassword()
    {
        var mockMemberType = new Mock<ISimpleContentType>();
        mockMemberType.Setup(mt => mt.Alias).Returns("Member");

        var mockMember = new Mock<IMember>();
        mockMember.Setup(m => m.Name).Returns("Jane");
        mockMember.Setup(m => m.Email).Returns("jane@example.com");
        mockMember.Setup(m => m.Username).Returns("jane");
        mockMember.Setup(m => m.ContentType).Returns(mockMemberType.Object);
        mockMember.Setup(m => m.IsApproved).Returns(true);
        mockMember.Setup(m => m.Properties).Returns(new PropertyCollection([]));

        var total = 1L;
        _mockMemberService.Setup(s => s.GetAll(0, int.MaxValue, out total))
            .Returns([mockMember.Object]);

        var sut = CreateSut();
        var result = await sut.ExportAsync();

        Assert.Single(result);
        // ExportMember has no password field — verify it doesn't exist on the model
        var properties = typeof(SplatDev.Umbraco.Plugins.Schema2Yaml.Models.ExportMember).GetProperties();
        Assert.DoesNotContain(properties, p => p.Name.Equals("Password", StringComparison.OrdinalIgnoreCase));
    }
}
