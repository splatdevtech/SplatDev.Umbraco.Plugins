namespace SplatDev.Umbraco.Plugins.Backups.Providers;

using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Models;

public class AzureBlobStorageProvider : ICloudStorageProvider
{
    private readonly BackupSettings _settings;

    public string ProviderName => "AzureBlobStorage";
    public string ProviderIcon => "cloud-azure";
    public bool RequiresOAuth => false;
    public bool RequiresApiKey => true;

    public AzureBlobStorageProvider(BackupSettings settings)
    {
        _settings = settings;
    }

    private string? GetConnectionString()
    {
        return _settings.CloudProviders
            .FirstOrDefault(c => c.ProviderType == ProviderName && c.Enabled)
            ?.Settings.GetValueOrDefault("ConnectionString");
    }

    public async Task UploadAsync(Stream data, string remotePath, string fileName, CancellationToken ct)
    {
        var connStr = GetConnectionString()
            ?? throw new InvalidOperationException("Azure Blob Storage connection string not configured.");

        var containerClient = new Azure.Storage.Blobs.BlobContainerClient(connStr, "backups");
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobClient = containerClient.GetBlobClient($"{remotePath}/{fileName}");
        await blobClient.UploadAsync(data, overwrite: true, ct);
    }

    public async Task<Stream> DownloadAsync(string remotePath, CancellationToken ct)
    {
        var connStr = GetConnectionString()
            ?? throw new InvalidOperationException("Azure Blob Storage connection string not configured.");

        var containerClient = new Azure.Storage.Blobs.BlobContainerClient(connStr, "backups");
        var blobClient = containerClient.GetBlobClient(remotePath);

        var ms = new MemoryStream();
        await blobClient.DownloadToAsync(ms, ct);
        ms.Position = 0;
        return ms;
    }

    public async Task DeleteAsync(string remotePath, CancellationToken ct)
    {
        var connStr = GetConnectionString()
            ?? throw new InvalidOperationException("Azure Blob Storage connection string not configured.");

        var containerClient = new Azure.Storage.Blobs.BlobContainerClient(connStr, "backups");
        var blobClient = containerClient.GetBlobClient(remotePath);
        await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
    }

    public async Task<bool> ValidateConnectionAsync(CancellationToken ct)
    {
        try
        {
            var connStr = GetConnectionString();
            if (string.IsNullOrEmpty(connStr))
                return false;

            var containerClient = new Azure.Storage.Blobs.BlobContainerClient(connStr, "backups");
            await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<StorageItem>> ListAsync(string remotePath, CancellationToken ct)
    {
        var connStr = GetConnectionString();
        if (string.IsNullOrEmpty(connStr))
            return Enumerable.Empty<StorageItem>();

        var containerClient = new Azure.Storage.Blobs.BlobContainerClient(connStr, "backups");
        if (!await containerClient.ExistsAsync(ct))
            return Enumerable.Empty<StorageItem>();

        var items = new List<StorageItem>();
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: remotePath, cancellationToken: ct))
        {
            items.Add(new StorageItem
            {
                Name = blobItem.Name,
                Path = blobItem.Name,
                SizeBytes = blobItem.Properties.ContentLength ?? 0,
                LastModified = blobItem.Properties.LastModified?.UtcDateTime ?? DateTime.MinValue,
                IsDirectory = false
            });
        }

        return items;
    }
}
