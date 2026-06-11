using Microsoft.AspNetCore.Mvc;
using Moq;
using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Controllers;
using SplatDev.Umbraco.Plugins.Backups.Models;
using SplatDev.Umbraco.Plugins.Backups.Services;
using Xunit;

namespace SplatDev.Umbraco.Plugins.Backups.Tests;

public class BackupsApiControllerTests
{
    private readonly Mock<IBackupsService> _serviceMock;
    private readonly BackupsApiController _controller;

    public BackupsApiControllerTests()
    {
        _serviceMock = new Mock<IBackupsService>();
        _controller = new BackupsApiController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithBackupList()
    {
        var backups = new[]
        {
            new BackupInfo { Name = "backup-1", Extension = ".json" },
            new BackupInfo { Name = "backup-2", Extension = ".zip" }
        };
        _serviceMock.Setup(s => s.ListBackupsAsync()).ReturnsAsync(backups);

        var actionResult = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Same(backups, ok.Value);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsOkWithBackupInfo()
    {
        var info = new BackupInfo { Name = "new-backup", Extension = ".json" };
        _serviceMock
            .Setup(s => s.CreateBackupAsync(It.IsAny<BackupRequest>()))
            .ReturnsAsync(info);

        var result = await _controller.Create(new BackupRequest());

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(info, ok.Value);
    }

    [Fact]
    public async Task Create_InvalidModelState_ReturnsBadRequest()
    {
        _controller.ModelState.AddModelError("Scope", "Required");

        var result = await _controller.Create(new BackupRequest());

        Assert.IsType<BadRequestObjectResult>(result);
        _serviceMock.Verify(s => s.CreateBackupAsync(It.IsAny<BackupRequest>()), Times.Never);
    }

    [Fact]
    public async Task CreateAdvanced_ValidOptions_ReturnsOkWithResult()
    {
        var backupResult = new BackupResult { Id = "abc", Name = "adv-backup", CreatedAt = DateTime.UtcNow };
        _serviceMock
            .Setup(s => s.CreateBackupAsync(It.IsAny<BackupOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(backupResult);

        var result = await _controller.CreateAdvanced(new BackupOptions(), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(backupResult, ok.Value);
    }

    [Fact]
    public async Task CreateAdvanced_InvalidModelState_ReturnsBadRequest()
    {
        _controller.ModelState.AddModelError("Scope", "Invalid");

        var result = await _controller.CreateAdvanced(new BackupOptions(), CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
        _serviceMock.Verify(s => s.CreateBackupAsync(It.IsAny<BackupOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Restore_NullOrEmptyBackupPath_ReturnsBadRequest()
    {
        var result = await _controller.Restore(string.Empty, new RestoreOptions(), CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
        _serviceMock.Verify(s => s.RestoreBackupAsync(It.IsAny<string>(), It.IsAny<RestoreOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Restore_ValidPath_ReturnsOkWithRestoreResult()
    {
        var restoreResult = new RestoreResult { Success = true, RestoredContentCount = 3 };
        _serviceMock
            .Setup(s => s.RestoreBackupAsync(It.IsAny<string>(), It.IsAny<RestoreOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(restoreResult);

        var result = await _controller.Restore("/backups/backup.json", new RestoreOptions(), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(restoreResult, ok.Value);
    }

    [Fact]
    public async Task Delete_NullOrEmptyName_ReturnsBadRequest()
    {
        var result = await _controller.Delete(string.Empty);

        Assert.IsType<BadRequestObjectResult>(result);
        _serviceMock.Verify(s => s.DeleteBackupAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Delete_ValidName_ReturnsOkWithMessage()
    {
        _serviceMock.Setup(s => s.DeleteBackupAsync("backup-abc")).Returns(Task.CompletedTask);

        var result = await _controller.Delete("backup-abc");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task Delete_FileNotFound_ReturnsNotFound()
    {
        _serviceMock
            .Setup(s => s.DeleteBackupAsync("missing-backup"))
            .ThrowsAsync(new FileNotFoundException("Backup 'missing-backup' not found."));

        var result = await _controller.Delete("missing-backup");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetCloudProviders_ReturnsOkWithProviderList()
    {
        var configs = new[]
        {
            new CloudProviderConfig { Id = "azure-1", ProviderType = "AzureBlobStorage", Enabled = true }
        };
        _serviceMock.Setup(s => s.GetCloudProvidersAsync()).ReturnsAsync(configs);

        var result = await _controller.GetCloudProviders();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(configs, ok.Value);
    }

    [Fact]
    public async Task TestProvider_NullOrEmptyProviderId_ReturnsBadRequest()
    {
        var result = await _controller.TestProvider(string.Empty, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
        _serviceMock.Verify(s => s.TestCloudProviderAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task TestProvider_ValidProviderId_ReturnsOkWithValidFlag()
    {
        _serviceMock
            .Setup(s => s.TestCloudProviderAsync("azure-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _controller.TestProvider("azure-1", CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task TestProvider_ProviderValidationFails_ReturnsOkWithFalseFlag()
    {
        _serviceMock
            .Setup(s => s.TestCloudProviderAsync("sftp-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _controller.TestProvider("sftp-1", CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }
}
