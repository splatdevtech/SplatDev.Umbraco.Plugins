using Microsoft.Extensions.Hosting;
using Moq;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Engine;
using SplatDev.Umbraco.Plugins.Backups.Models;
using SplatDev.Umbraco.Plugins.Backups.Providers;
using SplatDev.Umbraco.Plugins.Backups.Services;
using Xunit;

namespace SplatDev.Umbraco.Plugins.Backups.Tests;

public class BackupsServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _backupDir;
    private readonly Mock<IHostEnvironment> _hostEnvMock;
    private readonly Mock<IContentService> _contentServiceMock;
    private readonly Mock<IMediaService> _mediaServiceMock;
    private readonly Mock<IBackupEngine> _engineMock;
    private readonly BackupSettings _settings;

    public BackupsServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"bs-test-{Guid.NewGuid():N}");
        _backupDir = Path.Combine(_tempDir, "App_Data", "Backups");
        Directory.CreateDirectory(_backupDir);

        _hostEnvMock = new Mock<IHostEnvironment>();
        _hostEnvMock.SetupGet(e => e.ContentRootPath).Returns(_tempDir);

        _contentServiceMock = new Mock<IContentService>();
        _mediaServiceMock = new Mock<IMediaService>();
        _engineMock = new Mock<IBackupEngine>();

        _settings = new BackupSettings
        {
            BackupPath = "Backups",
            LocalRetentionDays = 30,
            MaxLocalBackups = 10,
            AutoCleanup = false
        };
    }

    private BackupsService CreateService(IEnumerable<ICloudStorageProvider>? providers = null) =>
        new BackupsService(
            _hostEnvMock.Object,
            _contentServiceMock.Object,
            _mediaServiceMock.Object,
            _engineMock.Object,
            _settings,
            providers ?? Enumerable.Empty<ICloudStorageProvider>());

    [Fact]
    public async Task ListBackupsAsync_EmptyDirectory_ReturnsEmpty()
    {
        var service = CreateService();

        var result = await service.ListBackupsAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task ListBackupsAsync_WithJsonFile_ReturnsSingleItem()
    {
        var filePath = Path.Combine(_backupDir, "backup-20240101-120000-abc123.json");
        await File.WriteAllTextAsync(filePath, "{}");

        var service = CreateService();
        var result = (await service.ListBackupsAsync()).ToList();

        Assert.Single(result);
        Assert.Equal("backup-20240101-120000-abc123", result[0].Name);
        Assert.Equal(".json", result[0].Extension);
        Assert.False(result[0].IsCompressed);
        Assert.False(result[0].IsEncrypted);
    }

    [Fact]
    public async Task ListBackupsAsync_WithZipFile_SetsIsCompressedTrue()
    {
        await File.WriteAllTextAsync(Path.Combine(_backupDir, "backup-test.zip"), "ZIP");

        var service = CreateService();
        var result = (await service.ListBackupsAsync()).ToList();

        Assert.True(result[0].IsCompressed);
        Assert.False(result[0].IsEncrypted);
    }

    [Fact]
    public async Task ListBackupsAsync_WithEncFile_SetsIsEncryptedTrue()
    {
        await File.WriteAllTextAsync(Path.Combine(_backupDir, "backup-test.enc"), "ENC");

        var service = CreateService();
        var result = (await service.ListBackupsAsync()).ToList();

        Assert.True(result[0].IsEncrypted);
        Assert.False(result[0].IsCompressed);
    }

    [Fact]
    public async Task ListBackupsAsync_WithMultipleFiles_ReturnsAllExtensions()
    {
        await File.WriteAllTextAsync(Path.Combine(_backupDir, "backup-a.json"), "{}");
        await File.WriteAllTextAsync(Path.Combine(_backupDir, "backup-b.zip"), "ZIP");
        await File.WriteAllTextAsync(Path.Combine(_backupDir, "backup-c.enc"), "ENC");

        var service = CreateService();
        var result = (await service.ListBackupsAsync()).ToList();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task ListBackupsAsync_SetsFileSizeCorrectly()
    {
        var content = new byte[512];
        await File.WriteAllBytesAsync(Path.Combine(_backupDir, "backup-sized.json"), content);

        var service = CreateService();
        var result = (await service.ListBackupsAsync()).ToList();

        Assert.Equal(512, result[0].SizeBytes);
    }

    [Fact]
    public async Task CreateBackupAsync_SimpleRequest_DelegatesToEngine()
    {
        var engineResult = new BackupResult
        {
            Id = "abc123",
            Name = "backup-20240101-120000-abc123",
            LocalPath = Path.Combine(_backupDir, "backup-20240101-120000-abc123.json"),
            SizeBytes = 256,
            Compressed = false,
            Encrypted = false,
            CreatedAt = DateTime.UtcNow
        };
        await File.WriteAllTextAsync(engineResult.LocalPath, "{}");

        _engineMock
            .Setup(e => e.CreateFullBackupAsync(It.IsAny<BackupOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(engineResult);

        var service = CreateService();
        var request = new BackupRequest { Scope = BackupScope.Content, Compress = false };

        var info = await service.CreateBackupAsync(request);

        Assert.Equal(engineResult.Name, info.Name);
        _engineMock.Verify(e => e.CreateFullBackupAsync(It.IsAny<BackupOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBackupAsync_WhenIncludeMedia_AddsMediaToScope()
    {
        _engineMock
            .Setup(e => e.CreateFullBackupAsync(It.IsAny<BackupOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BackupResult { Name = "test", LocalPath = string.Empty, CreatedAt = DateTime.UtcNow });

        var service = CreateService();
        var request = new BackupRequest
        {
            Scope = BackupScope.Content,
            IncludeMedia = true
        };

        await service.CreateBackupAsync(request);

        _engineMock.Verify(e => e.CreateFullBackupAsync(
            It.Is<BackupOptions>(o => o.Scope.HasFlag(BackupScope.Media)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBackupAsync_WithKeepLocalTrue_AlwaysSetInOptions()
    {
        BackupOptions? capturedOptions = null;
        _engineMock
            .Setup(e => e.CreateFullBackupAsync(It.IsAny<BackupOptions>(), It.IsAny<CancellationToken>()))
            .Callback<BackupOptions, CancellationToken>((opts, _) => capturedOptions = opts)
            .ReturnsAsync(new BackupResult { Name = "test", LocalPath = string.Empty, CreatedAt = DateTime.UtcNow });

        var service = CreateService();
        await service.CreateBackupAsync(new BackupRequest());

        Assert.NotNull(capturedOptions);
        Assert.True(capturedOptions!.KeepLocal);
    }

    [Fact]
    public async Task CreateBackupAsync_AdvancedOptions_DelegatesToEngineDirectly()
    {
        var expected = new BackupResult { Id = "xyz", Name = "adv-backup", CreatedAt = DateTime.UtcNow };
        _engineMock
            .Setup(e => e.CreateFullBackupAsync(It.IsAny<BackupOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var service = CreateService();
        var options = new BackupOptions { Scope = BackupScope.Full, Compress = true };

        var result = await service.CreateBackupAsync(options, CancellationToken.None);

        Assert.Equal(expected.Id, result.Id);
        Assert.Equal(expected.Name, result.Name);
    }

    [Fact]
    public async Task RestoreBackupAsync_DelegatesToEngine()
    {
        var expected = new RestoreResult { Success = true, RestoredContentCount = 5 };
        _engineMock
            .Setup(e => e.RestoreAsync(It.IsAny<string>(), It.IsAny<RestoreOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var service = CreateService();
        var result = await service.RestoreBackupAsync("/path/to/backup.json", new RestoreOptions());

        Assert.True(result.Success);
        Assert.Equal(5, result.RestoredContentCount);
        _engineMock.Verify(e => e.RestoreAsync("/path/to/backup.json", It.IsAny<RestoreOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBackupAsync_ExistingFile_DeletesFromDisk()
    {
        var filePath = Path.Combine(_backupDir, "backup-to-delete.json");
        await File.WriteAllTextAsync(filePath, "{}");

        var service = CreateService();
        await service.DeleteBackupAsync("backup-to-delete");

        Assert.False(File.Exists(filePath));
    }

    [Fact]
    public async Task DeleteBackupAsync_NonExistentName_ThrowsFileNotFoundException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<FileNotFoundException>(
            () => service.DeleteBackupAsync("nonexistent-backup-name"));
    }

    [Fact]
    public async Task DeleteBackupAsync_FindsFileByNameCaseInsensitive()
    {
        var filePath = Path.Combine(_backupDir, "Backup-UPPERCASE.json");
        await File.WriteAllTextAsync(filePath, "{}");

        var service = CreateService();
        await service.DeleteBackupAsync("backup-uppercase");

        Assert.False(File.Exists(filePath));
    }

    [Fact]
    public async Task GetCloudProvidersAsync_ReturnsConfiguredProviders()
    {
        _settings.CloudProviders = new List<CloudProviderConfig>
        {
            new() { Id = "azure-1", ProviderType = "AzureBlobStorage", Enabled = true }
        };

        var service = CreateService();
        var providers = (await service.GetCloudProvidersAsync()).ToList();

        Assert.Single(providers);
        Assert.Equal("azure-1", providers[0].Id);
        Assert.Equal("AzureBlobStorage", providers[0].ProviderType);
        Assert.True(providers[0].Enabled);
    }

    [Fact]
    public async Task GetCloudProvidersAsync_InjectsRequiresOAuthAndApiKeyFromProvider()
    {
        _settings.CloudProviders = new List<CloudProviderConfig>
        {
            new() { Id = "prov-1", ProviderType = "TestProvider", Enabled = true }
        };

        var providerMock = new Mock<ICloudStorageProvider>();
        providerMock.SetupGet(p => p.ProviderName).Returns("TestProvider");
        providerMock.SetupGet(p => p.RequiresOAuth).Returns(true);
        providerMock.SetupGet(p => p.RequiresApiKey).Returns(false);

        var service = CreateService(new[] { providerMock.Object });
        var providers = (await service.GetCloudProvidersAsync()).ToList();

        Assert.Single(providers);
        Assert.Equal("True", providers[0].Settings["requiresOAuth"]);
        Assert.Equal("False", providers[0].Settings["requiresApiKey"]);
    }

    [Fact]
    public async Task TestCloudProviderAsync_UnknownProviderId_ReturnsFalse()
    {
        var service = CreateService();

        var result = await service.TestCloudProviderAsync("nonexistent-provider");

        Assert.False(result);
    }

    [Fact]
    public async Task TestCloudProviderAsync_KnownProvider_CallsValidateConnectionAndReturnsResult()
    {
        var providerMock = new Mock<ICloudStorageProvider>();
        providerMock.SetupGet(p => p.ProviderName).Returns("KnownProvider");
        providerMock.Setup(p => p.ValidateConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService(new[] { providerMock.Object });

        var result = await service.TestCloudProviderAsync("KnownProvider");

        Assert.True(result);
        providerMock.Verify(p => p.ValidateConnectionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }
}
