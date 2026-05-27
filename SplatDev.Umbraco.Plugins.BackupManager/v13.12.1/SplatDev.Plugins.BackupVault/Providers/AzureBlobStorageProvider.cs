using Azure.Storage.Blobs;
using Azure.Storage;

namespace SplatDev.Plugins.BackupVault.Providers;

public class AzureBlobStorageProvider : IBackupStorageProvider
{
    private readonly BlobContainerClient _container;
    private readonly string _folder;

    public string ProviderName => "Azure Blob Storage";

    public AzureBlobStorageProvider(StorageProviderConfig config)
    {
        _folder = config.Folder ?? "backups";
        var containerName = config.BucketOrContainer ?? throw new ArgumentNullException(nameof(config.BucketOrContainer));

        BlobServiceClient serviceClient;
        if (!string.IsNullOrEmpty(config.AzureConnectionString))
            serviceClient = new BlobServiceClient(config.AzureConnectionString);
        else
        {
            var credential = new StorageSharedKeyCredential(
                config.AzureAccountName ?? throw new ArgumentNullException(nameof(config.AzureAccountName)),
                config.AzureAccountKey ?? throw new ArgumentNullException(nameof(config.AzureAccountKey)));
            serviceClient = new BlobServiceClient(new Uri($"https://{config.AzureAccountName}.blob.core.windows.net"), credential);
        }

        _container = serviceClient.GetBlobContainerClient(containerName);
        _container.CreateIfNotExists();
    }

    public async Task<bool> UploadAsync(string localFilePath, string remoteFileName, CancellationToken ct = default)
    {
        var blobName = string.IsNullOrEmpty(_folder) ? remoteFileName : $"{_folder}/{remoteFileName}";
        var blobClient = _container.GetBlobClient(blobName);
        await blobClient.UploadAsync(localFilePath, overwrite: true, ct);
        return true;
    }

    public async Task<IEnumerable<string>> ListAsync(string remoteFolder = "", CancellationToken ct = default)
    {
        var prefix = string.IsNullOrEmpty(remoteFolder) ? _folder : remoteFolder;
        var names = new List<string>();
        await foreach (var blob in _container.GetBlobsAsync(prefix: prefix, cancellationToken: ct))
            names.Add(blob.Name);
        return names;
    }

    public async Task<bool> DeleteAsync(string remoteFileName, CancellationToken ct = default)
    {
        var blobName = string.IsNullOrEmpty(_folder) ? remoteFileName : $"{_folder}/{remoteFileName}";
        await _container.DeleteBlobIfExistsAsync(blobName, cancellationToken: ct);
        return true;
    }

    public async Task<Stream> DownloadAsync(string remoteFileName, CancellationToken ct = default)
    {
        var blobName = string.IsNullOrEmpty(_folder) ? remoteFileName : $"{_folder}/{remoteFileName}";
        var blobClient = _container.GetBlobClient(blobName);
        var response = await blobClient.DownloadStreamingAsync(cancellationToken: ct);
        var ms = new MemoryStream();
        await response.Value.Content.CopyToAsync(ms, ct);
        ms.Position = 0;
        return ms;
    }
}
