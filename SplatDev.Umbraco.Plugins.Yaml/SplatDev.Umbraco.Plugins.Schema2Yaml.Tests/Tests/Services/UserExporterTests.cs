using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Configuration;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class UserExporterTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILogger<UserExporter>> _mockLogger;

    public UserExporterTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockLogger = new Mock<ILogger<UserExporter>>();
    }

    private UserExporter CreateSut(bool includeUsers = false) =>
        new(_mockUserService.Object,
            Options.Create(new Schema2YamlOptions { IncludeUsers = includeUsers }),
            _mockLogger.Object);

    [Fact]
    public async Task ExportAsync_WhenIncludeUsersIsFalse_ReturnsEmptyList()
    {
        var sut = CreateSut(includeUsers: false);

        var result = await sut.ExportAsync();

        Assert.Empty(result);
        _mockUserService.Verify(s => s.GetAll(It.IsAny<int>(), It.IsAny<int>(), out It.Ref<long>.IsAny), Times.Never);
    }

    [Fact]
    public async Task ExportAsync_DefaultOptions_DisablesUserExport()
    {
        var options = new Schema2YamlOptions();

        Assert.False(options.IncludeUsers);
    }

    [Fact]
    public async Task ExportAsync_WhenIncludeUsersIsTrue_CallsGetAll()
    {
        var total = 0L;
        _mockUserService.Setup(s => s.GetAll(0, int.MaxValue, out total)).Returns([]);

        var sut = CreateSut(includeUsers: true);
        var result = await sut.ExportAsync();

        Assert.Empty(result);
        _mockUserService.Verify(s => s.GetAll(0, int.MaxValue, out It.Ref<long>.IsAny), Times.Once);
    }

    [Fact]
    public async Task ExportAsync_MapsUserFields()
    {
        var mockGroup = new Mock<IReadOnlyUserGroup>();
        mockGroup.Setup(g => g.Alias).Returns("Administrators");

        var mockUser = new Mock<IUser>();
        mockUser.Setup(u => u.Name).Returns("Admin");
        mockUser.Setup(u => u.Email).Returns("admin@example.com");
        mockUser.Setup(u => u.Username).Returns("admin");
        mockUser.Setup(u => u.Groups).Returns([mockGroup.Object]);

        var total = 1L;
        _mockUserService.Setup(s => s.GetAll(0, int.MaxValue, out total))
            .Returns([mockUser.Object]);

        var sut = CreateSut(includeUsers: true);
        var result = await sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal("Admin", result[0].Name);
        Assert.Equal("admin@example.com", result[0].Email);
        Assert.Equal("admin", result[0].Username);
        Assert.Single(result[0].UserGroups);
        Assert.Equal("Administrators", result[0].UserGroups[0]);
    }

    [Fact]
    public async Task ExportAsync_DoesNotExportPassword()
    {
        var properties = typeof(SplatDev.Umbraco.Plugins.Schema2Yaml.Models.ExportUser).GetProperties();
        Assert.DoesNotContain(properties, p => p.Name.Equals("Password", StringComparison.OrdinalIgnoreCase));
    }
}
