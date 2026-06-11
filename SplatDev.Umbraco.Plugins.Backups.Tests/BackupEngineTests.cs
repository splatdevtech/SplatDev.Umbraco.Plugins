using System.IO.Compression;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Engine;
using SplatDev.Umbraco.Plugins.Backups.Models;
using SplatDev.Umbraco.Plugins.Backups.Providers;
using Xunit;
using IOFile = System.IO.File;
using IOPath = System.IO.Path;
using IODirectory = System.IO.Directory;

namespace SplatDev.Umbraco.Plugins.Backups.Tests;

public class BackupEngineTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _backupSubDir;
    private readonly Mock<IHostEnvironment> _hostEnvMock;
    private readonly Mock<IContentService> _contentServiceMock;
    private readonly Mock<IMediaService> _mediaServiceMock;
    private readonly Mock<ILogger<BackupEngine>> _loggerMock;
    private readonly BackupSettings _settings;

    public BackupEngineTests()
    {
        _tempDir = IOPath.Combine(IOPath.GetTempPath(), $"be-test-{Guid.NewGuid():N}");
        IODirectory.CreateDirectory(_tempDir);
        _backupSubDir = IOPath.Combine(_tempDir, "Backups");

        _hostEnvMock = new Mock<IHostEnvironment>();
        _hostEnvMock.SetupGet(e => e.ContentRootPath).Returns(_tempDir);

        _contentServiceMock = new Mock<IContentService>();
        _mediaServiceMock = new Mock<IMediaService>();
        _loggerMock = new Mock<ILogger<BackupEngine>>();

        _settings = new BackupSettings
        {
            BackupPath = "Backups",
            LocalRetentionDays = 30,
            MaxLocalBackups = 10,
            AutoCleanup = false
        };

        SetupEmptyTrees();
    }

    private void SetupEmptyTrees()
    {
        _contentServiceMock.Setup(s => s.GetRootContent()).Returns(Array.Empty<IContent>());
        _mediaServiceMock.Setup(s => s.GetRootMedia()).Returns(Array.Empty<IMedia>());
    }

    private void SetupEmptyContentChildren()
    {
        long total = 0L;
        _contentServiceMock
            .Setup(s => s.GetPagedChildren(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), out total, null, null))
            .Returns(Array.Empty<IContent>());
    }

    private void SetupEmptyMediaChildren()
    {
        long total = 0L;
        _mediaServiceMock
            .Setup(s => s.GetPagedChildren(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), out total, null, null))
            .Returns(Array.Empty<IMedia>());
    }

    private BackupEngine CreateEngine(IEnumerable<ICloudStorageProvider>? providers = null) =>
        new BackupEngine(
            _hostEnvMock.Object,
            _contentServiceMock.Object,
            _mediaServiceMock.Object,
            _settings,
            providers ?? Enumerable.Empty<ICloudStorageProvider>(),
            _loggerMock.Object);

    private static Mock<IContent> BuildContent(int id, string name = "Page", string contentType = "page", int parentId = -1)
    {
        var ctMock = new Mock<ISimpleContentType>();
        ctMock.SetupGet(ct => ct.Alias).Returns(contentType);

        var mock = new Mock<IContent>();
        mock.SetupGet(c => c.Id).Returns(id);
        mock.SetupGet(c => c.Key).Returns(Guid.NewGuid());
        mock.SetupGet(c => c.Name).Returns(name);
        mock.SetupGet(c => c.ContentType).Returns(ctMock.Object);
        mock.SetupGet(c => c.Published).Returns(true);
        mock.SetupGet(c => c.ParentId).Returns(parentId);
        return mock;
    }

    private static Mock<IMedia> BuildMedia(int id, string name = "Image", string mediaType = "image", int parentId = -1)
    {
        var ctMock = new Mock<ISimpleContentType>();
        ctMock.SetupGet(ct => ct.Alias).Returns(mediaType);

        var mock = new Mock<IMedia>();
        mock.SetupGet(m => m.Id).Returns(id);
        mock.SetupGet(m => m.Key).Returns(Guid.NewGuid());
        mock.SetupGet(m => m.Name).Returns(name);
        mock.SetupGet(m => m.ContentType).Returns(ctMock.Object);
        mock.SetupGet(m => m.ParentId).Returns(parentId);
        return mock;
    }

    [Fact]
    public async Task CreateFullBackupAsync_Default_WritesJsonFile()
    {
        var engine = CreateEngine();
        var options = new BackupOptions { Scope = BackupScope.ContentAndMedia, Compress = false, KeepLocal = true };

        var result = await engine.CreateFullBackupAsync(options);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Name));
        Assert.True(IOFile.Exists(result.LocalPath));
        Assert.EndsWith(".json", result.LocalPath);
    }

    [Fact]
    public async Task CreateFullBackupAsync_WithCompress_WritesZipFile()
    {
        var engine = CreateEngine();
        var options = new BackupOptions { Compress = true, Encrypt = false, KeepLocal = true };

        var result = await engine.CreateFullBackupAsync(options);

        Assert.True(IOFile.Exists(result.LocalPath));
        Assert.EndsWith(".zip", result.LocalPath);
        Assert.True(result.Compressed);

        using var archive = ZipFile.OpenRead(result.LocalPath);
        Assert.Single(archive.Entries);
        Assert.EndsWith(".json", archive.Entries[0].Name);
    }

    [Fact]
    public async Task CreateFullBackupAsync_WithEncryptAndCompress_WritesEncFile()
    {
        var engine = CreateEngine();
        var options = new BackupOptions
        {
            Compress = true,
            Encrypt = true,
            EncryptionKey = "secret-key-123",
            KeepLocal = true
        };

        var result = await engine.CreateFullBackupAsync(options);

        Assert.True(IOFile.Exists(result.LocalPath));
        Assert.EndsWith(".enc", result.LocalPath);
        Assert.True(result.Encrypted);
    }

    [Fact]
    public async Task CreateFullBackupAsync_WithContentItems_SetsContentCount()
    {
        var root = BuildContent(1, "Home");
        _contentServiceMock.Setup(s => s.GetRootContent()).Returns(new[] { root.Object });
        SetupEmptyContentChildren();

        var engine = CreateEngine();
        var result = await engine.CreateFullBackupAsync(new BackupOptions { Scope = BackupScope.Content });

        Assert.Equal(1, result.ContentCount);
        Assert.Equal(0, result.MediaCount);
    }

    [Fact]
    public async Task CreateFullBackupAsync_WithMediaItems_SetsMediaCount()
    {
        var root = BuildMedia(100, "Images");
        _mediaServiceMock.Setup(s => s.GetRootMedia()).Returns(new[] { root.Object });
        SetupEmptyMediaChildren();

        var engine = CreateEngine();
        var result = await engine.CreateFullBackupAsync(new BackupOptions { Scope = BackupScope.Media });

        Assert.Equal(0, result.ContentCount);
        Assert.Equal(1, result.MediaCount);
    }

    [Fact]
    public async Task CreateFullBackupAsync_WithMatchingCloudProvider_CallsUploadAsync()
    {
        var providerMock = new Mock<ICloudStorageProvider>();
        providerMock.SetupGet(p => p.ProviderName).Returns("MyProvider");
        providerMock.Setup(p => p.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var engine = CreateEngine(new[] { providerMock.Object });
        var options = new BackupOptions
        {
            Compress = false,
            KeepLocal = true,
            CloudProviderIds = new List<string> { "MyProvider" }
        };

        var result = await engine.CreateFullBackupAsync(options);

        providerMock.Verify(
            p => p.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
        Assert.Single(result.CloudUploads);
        Assert.True(result.CloudUploads[0].Success);
    }

    [Fact]
    public async Task CreateFullBackupAsync_WhenCloudProviderIdNotInList_SkipsUpload()
    {
        var providerMock = new Mock<ICloudStorageProvider>();
        providerMock.SetupGet(p => p.ProviderName).Returns("OtherProvider");

        var engine = CreateEngine(new[] { providerMock.Object });
        var options = new BackupOptions
        {
            KeepLocal = true,
            CloudProviderIds = new List<string> { "DifferentProvider" }
        };

        var result = await engine.CreateFullBackupAsync(options);

        providerMock.Verify(
            p => p.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        Assert.Empty(result.CloudUploads);
    }

    [Fact]
    public async Task CreateFullBackupAsync_WhenCloudUploadFails_RecordsError()
    {
        var providerMock = new Mock<ICloudStorageProvider>();
        providerMock.SetupGet(p => p.ProviderName).Returns("FailProvider");
        providerMock.Setup(p => p.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IOException("Connection failed"));

        var engine = CreateEngine(new[] { providerMock.Object });
        var options = new BackupOptions
        {
            KeepLocal = true,
            CloudProviderIds = new List<string> { "FailProvider" }
        };

        var result = await engine.CreateFullBackupAsync(options);

        Assert.Single(result.CloudUploads);
        Assert.False(result.CloudUploads[0].Success);
        Assert.Contains("Connection failed", result.CloudUploads[0].ErrorMessage);
    }

    [Fact]
    public async Task CreateFullBackupAsync_WithAutoCleanup_RemovesFilesOverLimit()
    {
        _settings.AutoCleanup = true;
        _settings.MaxLocalBackups = 2;
        _settings.LocalRetentionDays = 365;

        IODirectory.CreateDirectory(_backupSubDir);
        for (var i = 1; i <= 4; i++)
        {
            var path = IOPath.Combine(_backupSubDir, $"backup-20240101-00000{i}-aabbcc{i}.json");
            await IOFile.WriteAllTextAsync(path, "{}");
            IOFile.SetCreationTimeUtc(path, DateTime.UtcNow.AddHours(-i));
        }

        var engine = CreateEngine();
        var options = new BackupOptions { Compress = false, KeepLocal = false };

        await engine.CreateFullBackupAsync(options);

        var remaining = IODirectory.GetFiles(_backupSubDir, "*.json");
        Assert.True(remaining.Length <= _settings.MaxLocalBackups + 1);
    }

    [Fact]
    public async Task CreateContentBackupAsync_ForcesScopeToContent()
    {
        var engine = CreateEngine();
        var options = new BackupOptions { Scope = BackupScope.Full, KeepLocal = false };

        await engine.CreateContentBackupAsync(options);

        Assert.Equal(BackupScope.Content, options.Scope);
    }

    [Fact]
    public async Task RestoreAsync_FileDoesNotExist_ReturnsErrorResult()
    {
        var engine = CreateEngine();
        var result = await engine.RestoreAsync("/nonexistent/path/backup.json", new RestoreOptions());

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("not found", result.Errors[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RestoreAsync_WithValidJsonFile_RestoresContentAndMedia()
    {
        var json = """
            {
              "createdAt": "2024-01-01T00:00:00Z",
              "content": [
                { "id": 1, "key": "00000000-0000-0000-0000-000000000001", "name": "Home", "contentType": "page", "published": true, "parentId": -1 }
              ],
              "media": [
                { "id": 100, "key": "00000000-0000-0000-0000-000000000002", "name": "Images", "mediaType": "folder", "parentId": -1 }
              ],
              "databaseBackup": null
            }
            """;

        var backupPath = IOPath.Combine(_tempDir, "backup-test.json");
        await IOFile.WriteAllTextAsync(backupPath, json);

        _contentServiceMock.Setup(s => s.GetById(It.IsAny<int>())).Returns((IContent?)null);
        _mediaServiceMock.Setup(s => s.GetById(It.IsAny<int>())).Returns((IMedia?)null);

        var engine = CreateEngine();
        var result = await engine.RestoreAsync(backupPath, new RestoreOptions { Scope = BackupScope.Full });

        Assert.True(result.Success);
        Assert.Equal(1, result.RestoredContentCount);
        Assert.Equal(1, result.RestoredMediaCount);
    }

    [Fact]
    public async Task RestoreAsync_ContentScopeOnly_DoesNotRestoreMedia()
    {
        var json = """
            {
              "createdAt": "2024-01-01T00:00:00Z",
              "content": [
                { "id": 1, "key": "00000000-0000-0000-0000-000000000001", "name": "Home", "contentType": "page", "published": true, "parentId": -1 }
              ],
              "media": [
                { "id": 100, "key": "00000000-0000-0000-0000-000000000002", "name": "Images", "mediaType": "folder", "parentId": -1 }
              ],
              "databaseBackup": null
            }
            """;

        var backupPath = IOPath.Combine(_tempDir, "backup-scope.json");
        await IOFile.WriteAllTextAsync(backupPath, json);
        _contentServiceMock.Setup(s => s.GetById(It.IsAny<int>())).Returns((IContent?)null);

        var engine = CreateEngine();
        var result = await engine.RestoreAsync(backupPath, new RestoreOptions { Scope = BackupScope.Content });

        Assert.True(result.Success);
        Assert.Equal(1, result.RestoredContentCount);
        Assert.Equal(0, result.RestoredMediaCount);
    }

    [Fact]
    public async Task RestoreAsync_OverwriteFalse_SkipsExistingContent()
    {
        var json = """
            {
              "createdAt": "2024-01-01T00:00:00Z",
              "content": [
                { "id": 42, "key": "00000000-0000-0000-0000-000000000042", "name": "Existing", "contentType": "page", "published": true, "parentId": -1 }
              ],
              "media": [],
              "databaseBackup": null
            }
            """;

        var backupPath = IOPath.Combine(_tempDir, "backup-overwrite.json");
        await IOFile.WriteAllTextAsync(backupPath, json);

        var existingMock = BuildContent(42, "Existing");
        _contentServiceMock.Setup(s => s.GetById(42)).Returns(existingMock.Object);

        var engine = CreateEngine();
        var result = await engine.RestoreAsync(backupPath, new RestoreOptions
        {
            Scope = BackupScope.Content,
            OverwriteExisting = false
        });

        Assert.True(result.Success);
        Assert.Equal(0, result.RestoredContentCount);
    }

    [Fact]
    public async Task RestoreAsync_WithZipFile_ExtractsAndRestores()
    {
        var json = """
            {
              "createdAt": "2024-01-01T00:00:00Z",
              "content": [
                { "id": 5, "key": "00000000-0000-0000-0000-000000000005", "name": "Zipped", "contentType": "article", "published": true, "parentId": -1 }
              ],
              "media": [],
              "databaseBackup": null
            }
            """;

        var zipPath = IOPath.Combine(_tempDir, "backup-zip-test.zip");
        await using (var zipStream = IOFile.Create(zipPath))
        {
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Create);
            var entry = archive.CreateEntry("backup-zip-test.json");
            await using var entryStream = entry.Open();
            await entryStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(json));
        }

        _contentServiceMock.Setup(s => s.GetById(It.IsAny<int>())).Returns((IContent?)null);

        var engine = CreateEngine();
        var result = await engine.RestoreAsync(zipPath, new RestoreOptions { Scope = BackupScope.Content });

        Assert.True(result.Success);
        Assert.Equal(1, result.RestoredContentCount);
    }

    [Fact]
    public async Task RestoreAsync_WithEncryptedFile_DecryptsAndRestores()
    {
        var json = """
            {
              "createdAt": "2024-01-01T00:00:00Z",
              "content": [
                { "id": 7, "key": "00000000-0000-0000-0000-000000000007", "name": "Secret", "contentType": "page", "published": true, "parentId": -1 }
              ],
              "media": [],
              "databaseBackup": null
            }
            """;

        var jsonPath = IOPath.Combine(_tempDir, "backup-enc-source.json");
        var encPath = IOPath.Combine(_tempDir, "backup-enc-test.enc");
        await IOFile.WriteAllTextAsync(jsonPath, json);
        await BackupEngineTestHelpers.EncryptFileAsync(jsonPath, encPath, "my-secret-key", CancellationToken.None);
        IOFile.Delete(jsonPath);

        IODirectory.CreateDirectory(_backupSubDir);
        _contentServiceMock.Setup(s => s.GetById(It.IsAny<int>())).Returns((IContent?)null);

        var engine = CreateEngine();
        var result = await engine.RestoreAsync(encPath, new RestoreOptions
        {
            Scope = BackupScope.Content,
            DecryptionKey = "my-secret-key"
        });

        Assert.True(result.Success);
        Assert.Equal(1, result.RestoredContentCount);
    }

    [Fact]
    public async Task CreateFullBackupAsync_ContentTree_WithNestedNodes_IncludesAll()
    {
        var parent = BuildContent(1, "Parent");
        var child = BuildContent(2, "Child", parentId: 1);

        _contentServiceMock.Setup(s => s.GetRootContent()).Returns(new[] { parent.Object });

        long total1 = 0L;
        _contentServiceMock
            .Setup(s => s.GetPagedChildren(1, It.IsAny<long>(), It.IsAny<int>(), out total1, null, null))
            .Returns(new[] { child.Object });

        long total2 = 0L;
        _contentServiceMock
            .Setup(s => s.GetPagedChildren(2, It.IsAny<long>(), It.IsAny<int>(), out total2, null, null))
            .Returns(Array.Empty<IContent>());

        var engine = CreateEngine();
        var result = await engine.CreateFullBackupAsync(new BackupOptions { Scope = BackupScope.Content });

        Assert.Equal(2, result.ContentCount);
    }

    [Fact]
    public async Task CreateFullBackupAsync_ContentTree_CycleDetection_DoesNotLoopInfinitely()
    {
        var nodeA = BuildContent(1, "CycleNode");
        _contentServiceMock.Setup(s => s.GetRootContent()).Returns(new[] { nodeA.Object });

        long total = 0L;
        _contentServiceMock
            .Setup(s => s.GetPagedChildren(1, It.IsAny<long>(), It.IsAny<int>(), out total, null, null))
            .Returns(new[] { nodeA.Object });

        var engine = CreateEngine();

        var ex = await Record.ExceptionAsync(() =>
            engine.CreateFullBackupAsync(new BackupOptions { Scope = BackupScope.Content }));

        Assert.Null(ex);
    }

    public void Dispose()
    {
        if (IODirectory.Exists(_tempDir))
            IODirectory.Delete(_tempDir, recursive: true);
    }
}
