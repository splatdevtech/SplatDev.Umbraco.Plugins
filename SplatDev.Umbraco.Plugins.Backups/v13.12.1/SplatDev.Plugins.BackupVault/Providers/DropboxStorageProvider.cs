using Dropbox.Api;
using Dropbox.Api.Files;

namespace SplatDev.Plugins.BackupVault.Providers;

public class DropboxStorageProvider : IBackupStorageProvider
{
    private readonly DropboxClient _client;
    private readonly string _folder;

    public string ProviderName => "Dropbox";

    public DropboxStorageProvider(StorageProviderConfig config)
    {
        _folder = config.Folder ?? "/backups";
        _client = new DropboxClient(config.DropboxAccessToken ?? throw new ArgumentNullException(nameof(config.DropboxAccessToken)));
    }

    public async Task<bool> UploadAsync(string localFilePath, string remoteFileName, CancellationToken ct = default)
    {
        var remotePath = $"{_folder}/{remoteFileName}";
        using var fileStream = File.OpenRead(localFilePath);
        await _client.Files.UploadAsync(new CommitInfo(remotePath, WriteMode.Overwrite.Instance), fileStream);
        return true;
    }

    public async Task<IEnumerable<string>> ListAsync(string remoteFolder = "", CancellationToken ct = default)
    {
        var folder = string.IsNullOrEmpty(remoteFolder) ? _folder : remoteFolder;
        var result = await _client.Files.ListFolderAsync(folder);
        return result.Entries.Select(e => e.Name);
    }

    public async Task<bool> DeleteAsync(string remoteFileName, CancellationToken ct = default)
    {
        var remotePath = $"{_folder}/{remoteFileName}";
        await _client.Files.DeleteV2Async(remotePath);
        return true;
    }

    public async Task<Stream> DownloadAsync(string remoteFileName, CancellationToken ct = default)
    {
        var remotePath = $"{_folder}/{remoteFileName}";
        var response = await _client.Files.DownloadAsync(remotePath);
        return await response.GetContentAsStreamAsync();
    }
}
