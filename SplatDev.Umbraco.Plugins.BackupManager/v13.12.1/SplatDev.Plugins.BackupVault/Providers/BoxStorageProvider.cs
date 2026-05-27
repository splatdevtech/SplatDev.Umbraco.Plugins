using Box.V2;
using Box.V2.Auth;
using Box.V2.Config;
using Box.V2.Models;
using Box.V2.Models.Request;

namespace SplatDev.Plugins.BackupVault.Providers;

public class BoxStorageProvider : IBackupStorageProvider
{
    private readonly BoxClient _client;
    private readonly string _folderId;

    public string ProviderName => "Box";

    public BoxStorageProvider(StorageProviderConfig config)
    {
        _folderId = config.BoxFolderId ?? "0";
        var boxConfig = new BoxConfigBuilder(
            config.BoxClientId ?? throw new ArgumentNullException(nameof(config.BoxClientId)),
            config.BoxClientSecret ?? throw new ArgumentNullException(nameof(config.BoxClientSecret)))
            .Build();
        var session = new OAuthSession(
            config.BoxAccessToken ?? throw new ArgumentNullException(nameof(config.BoxAccessToken)),
            string.Empty, 3600, "bearer");
        _client = new BoxClient(boxConfig, session);
    }

    public async Task<bool> UploadAsync(string localFilePath, string remoteFileName, CancellationToken ct = default)
    {
        using var fileStream = File.OpenRead(localFilePath);
        var request = new BoxFileRequest { Name = remoteFileName, Parent = new BoxRequestEntity { Id = _folderId } };
        await _client.FilesManager.UploadAsync(request, fileStream);
        return true;
    }

    public async Task<IEnumerable<string>> ListAsync(string remoteFolder = "", CancellationToken ct = default)
    {
        var folderId = string.IsNullOrEmpty(remoteFolder) ? _folderId : remoteFolder;
        var items = await _client.FoldersManager.GetFolderItemsAsync(folderId, 200);
        return items.Entries.Select(e => e.Name);
    }

    public async Task<bool> DeleteAsync(string remoteFileName, CancellationToken ct = default)
    {
        var items = await _client.FoldersManager.GetFolderItemsAsync(_folderId, 200);
        var file = items.Entries.FirstOrDefault(e => e.Name == remoteFileName && e.Type == "file");
        if (file is not null)
            await _client.FilesManager.DeleteAsync(file.Id);
        return true;
    }

    public async Task<Stream> DownloadAsync(string remoteFileName, CancellationToken ct = default)
    {
        var items = await _client.FoldersManager.GetFolderItemsAsync(_folderId, 200);
        var file = items.Entries.FirstOrDefault(e => e.Name == remoteFileName && e.Type == "file");
        if (file is null) throw new FileNotFoundException(remoteFileName);
        return await _client.FilesManager.DownloadAsync(file.Id);
    }
}
