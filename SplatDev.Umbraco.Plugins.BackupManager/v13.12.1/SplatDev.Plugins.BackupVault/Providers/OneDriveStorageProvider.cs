using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace SplatDev.Plugins.BackupVault.Providers;

public class OneDriveStorageProvider : IBackupStorageProvider
{
    private readonly GraphServiceClient _client;
    private readonly string _driveId;
    private readonly string _folder;

    public string ProviderName => "OneDrive";

    public OneDriveStorageProvider(StorageProviderConfig config)
    {
        _driveId = config.OneDriveDriveId ?? string.Empty;
        _folder = config.Folder ?? "backups";

        var credential = new ClientSecretCredential(
            config.OneDriveTenantId ?? throw new ArgumentNullException(nameof(config.OneDriveTenantId)),
            config.OneDriveClientId ?? throw new ArgumentNullException(nameof(config.OneDriveClientId)),
            config.OneDriveClientSecret ?? throw new ArgumentNullException(nameof(config.OneDriveClientSecret)));

        _client = new GraphServiceClient(credential);
    }

    public async Task<bool> UploadAsync(string localFilePath, string remoteFileName, CancellationToken ct = default)
    {
        using var fileStream = File.OpenRead(localFilePath);
        var uploadPath = $"{_folder}/{remoteFileName}";

        if (string.IsNullOrEmpty(_driveId))
            await _client.Me.Drive.Root.ItemWithPath(uploadPath).Content.PutAsync(fileStream, cancellationToken: ct);
        else
            await _client.Drives[_driveId].Root.ItemWithPath(uploadPath).Content.PutAsync(fileStream, cancellationToken: ct);

        return true;
    }

    public async Task<IEnumerable<string>> ListAsync(string remoteFolder = "", CancellationToken ct = default)
    {
        var folder = string.IsNullOrEmpty(remoteFolder) ? _folder : remoteFolder;
        DriveItemCollectionResponse? items;

        if (string.IsNullOrEmpty(_driveId))
            items = await _client.Me.Drive.Root.ItemWithPath(folder).Children.GetAsync(cancellationToken: ct);
        else
            items = await _client.Drives[_driveId].Root.ItemWithPath(folder).Children.GetAsync(cancellationToken: ct);

        return items?.Value?.Select(i => i.Name ?? string.Empty) ?? Enumerable.Empty<string>();
    }

    public async Task<bool> DeleteAsync(string remoteFileName, CancellationToken ct = default)
    {
        var remotePath = $"{_folder}/{remoteFileName}";
        if (string.IsNullOrEmpty(_driveId))
            await _client.Me.Drive.Root.ItemWithPath(remotePath).DeleteAsync(cancellationToken: ct);
        else
            await _client.Drives[_driveId].Root.ItemWithPath(remotePath).DeleteAsync(cancellationToken: ct);
        return true;
    }

    public async Task<Stream> DownloadAsync(string remoteFileName, CancellationToken ct = default)
    {
        var remotePath = $"{_folder}/{remoteFileName}";
        Stream? stream;
        if (string.IsNullOrEmpty(_driveId))
            stream = await _client.Me.Drive.Root.ItemWithPath(remotePath).Content.GetAsync(cancellationToken: ct);
        else
            stream = await _client.Drives[_driveId].Root.ItemWithPath(remotePath).Content.GetAsync(cancellationToken: ct);
        return stream ?? Stream.Null;
    }
}
