using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Providers;
using Xunit;

namespace SplatDev.Umbraco.Plugins.Backups.Tests;

public class LocalFileSystemStorageProviderTests : IDisposable
{
    private readonly string _tempDir;
    private readonly BackupSettings _settings;
    private readonly LocalFileSystemStorageProvider _provider;

    public LocalFileSystemStorageProviderTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"backups-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        _settings = new BackupSettings
        {
            BackupPath = _tempDir,
            LocalRetentionDays = 30,
            MaxLocalBackups = 10
        };

        _provider = new LocalFileSystemStorageProvider(_settings);
    }

    [Fact]
    public void ProviderName_ReturnsLocalFileSystem()
    {
        Assert.Equal("LocalFileSystem", _provider.ProviderName);
    }

    [Fact]
    public void RequiresOAuth_ReturnsFalse()
    {
        Assert.False(_provider.RequiresOAuth);
    }

    [Fact]
    public void RequiresApiKey_ReturnsFalse()
    {
        Assert.False(_provider.RequiresApiKey);
    }

    [Fact]
    public async Task ValidateConnectionAsync_WithValidDirectory_ReturnsTrue()
    {
        var result = await _provider.ValidateConnectionAsync(CancellationToken.None);
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateConnectionAsync_WithNonExistentDirectory_CreatesAndReturnsTrue()
    {
        var newDir = Path.Combine(_tempDir, "subdir");
        var settings = new BackupSettings { BackupPath = newDir };
        var provider = new LocalFileSystemStorageProvider(settings);

        var result = await provider.ValidateConnectionAsync(CancellationToken.None);

        Assert.True(result);
        Assert.True(Directory.Exists(newDir));
    }

    [Fact]
    public async Task UploadAsync_CreatesFileInCorrectLocation()
    {
        var content = "test backup content"u8.ToArray();
        using var stream = new MemoryStream(content);

        await _provider.UploadAsync(stream, "2024/01", "backup.zip", CancellationToken.None);

        var expectedPath = Path.Combine(_tempDir, "2024", "01", "backup.zip");
        Assert.True(File.Exists(expectedPath));
        Assert.Equal(content, await File.ReadAllBytesAsync(expectedPath));
    }

    [Fact]
    public async Task UploadAsync_CreatesSubdirectoriesAutomatically()
    {
        var content = "data"u8.ToArray();
        using var stream = new MemoryStream(content);

        await _provider.UploadAsync(stream, "deep/nested/path", "file.zip", CancellationToken.None);

        var dir = Path.Combine(_tempDir, "deep", "nested", "path");
        Assert.True(Directory.Exists(dir));
    }

    [Fact]
    public async Task DownloadAsync_ReturnsCorrectContent()
    {
        var content = "download test"u8.ToArray();
        var filePath = Path.Combine(_tempDir, "test-download.zip");
        await File.WriteAllBytesAsync(filePath, content);

        await using var result = await _provider.DownloadAsync(filePath, CancellationToken.None);
        var ms = new MemoryStream();
        await result.CopyToAsync(ms);

        Assert.Equal(content, ms.ToArray());
    }

    [Fact]
    public async Task DownloadAsync_FileNotFound_ThrowsFileNotFoundException()
    {
        await Assert.ThrowsAsync<FileNotFoundException>(
            () => _provider.DownloadAsync("/nonexistent/path/file.zip", CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_RemovesFile()
    {
        var filePath = Path.Combine(_tempDir, "to-delete.zip");
        await File.WriteAllTextAsync(filePath, "content");

        await _provider.DeleteAsync(filePath, CancellationToken.None);

        Assert.False(File.Exists(filePath));
    }

    [Fact]
    public async Task DeleteAsync_NonExistentFile_DoesNotThrow()
    {
        await _provider.DeleteAsync("/nonexistent/file.zip", CancellationToken.None);
    }

    [Fact]
    public async Task ListAsync_ReturnsFilesInDirectory()
    {
        var subDir = Path.Combine(_tempDir, "backups");
        Directory.CreateDirectory(subDir);
        await File.WriteAllTextAsync(Path.Combine(subDir, "backup1.zip"), "a");
        await File.WriteAllTextAsync(Path.Combine(subDir, "backup2.json"), "b");

        var items = (await _provider.ListAsync("backups", CancellationToken.None)).ToList();

        Assert.Equal(2, items.Count);
        Assert.Contains(items, i => i.Name == "backup1.zip");
        Assert.Contains(items, i => i.Name == "backup2.json");
    }

    [Fact]
    public async Task ListAsync_EmptyDirectory_ReturnsEmpty()
    {
        var items = await _provider.ListAsync("nonexistent-subdir", CancellationToken.None);
        Assert.Empty(items);
    }

    [Fact]
    public async Task ListAsync_ItemsHaveCorrectProperties()
    {
        var subDir = Path.Combine(_tempDir, "props-test");
        Directory.CreateDirectory(subDir);
        var content = new byte[1024];
        await File.WriteAllBytesAsync(Path.Combine(subDir, "test.zip"), content);

        var items = (await _provider.ListAsync("props-test", CancellationToken.None)).ToList();

        var item = Assert.Single(items);
        Assert.Equal("test.zip", item.Name);
        Assert.Equal(1024, item.SizeBytes);
        Assert.False(item.IsDirectory);
        Assert.True(item.LastModified > DateTime.MinValue);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }
}
