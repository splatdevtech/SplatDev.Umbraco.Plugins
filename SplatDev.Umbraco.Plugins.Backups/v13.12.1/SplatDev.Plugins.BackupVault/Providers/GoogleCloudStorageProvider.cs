using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace SplatDev.Plugins.BackupVault.Providers;

public class GoogleCloudStorageProvider : IBackupStorageProvider
{
    private readonly StorageClient _client;
    private readonly string _bucketName;
    private readonly string _folder;

    public string ProviderName => "Google Cloud Storage";

    public GoogleCloudStorageProvider(StorageProviderConfig config)
    {
        _bucketName = config.BucketOrContainer ?? throw new ArgumentNullException(nameof(config.BucketOrContainer));
        _folder = config.Folder ?? "backups";

        GoogleCredential credential;
        if (!string.IsNullOrWhiteSpace(config.GoogleCredentialsJson))
            credential = GoogleCredential.FromJson(config.GoogleCredentialsJson);
        else
            credential = GoogleCredential.GetApplicationDefault();

        _client = StorageClient.Create(credential);
    }

    public async Task<bool> UploadAsync(string localFilePath, string remoteFileName, CancellationToken ct = default)
    {
        var objectName = string.IsNullOrEmpty(_folder) ? remoteFileName : $"{_folder}/{remoteFileName}";
        using var fileStream = File.OpenRead(localFilePath);
        await _client.UploadObjectAsync(_bucketName, objectName, "application/octet-stream", fileStream, cancellationToken: ct);
        return true;
    }

    public async Task<IEnumerable<string>> ListAsync(string remoteFolder = "", CancellationToken ct = default)
    {
        var prefix = string.IsNullOrEmpty(remoteFolder) ? _folder : remoteFolder;
        var objects = _client.ListObjectsAsync(_bucketName, prefix, cancellationToken: ct);
        var names = new List<string>();
        await foreach (var obj in objects)
            names.Add(obj.Name);
        return names;
    }

    public async Task<bool> DeleteAsync(string remoteFileName, CancellationToken ct = default)
    {
        var objectName = string.IsNullOrEmpty(_folder) ? remoteFileName : $"{_folder}/{remoteFileName}";
        await _client.DeleteObjectAsync(_bucketName, objectName, cancellationToken: ct);
        return true;
    }

    public async Task<Stream> DownloadAsync(string remoteFileName, CancellationToken ct = default)
    {
        var objectName = string.IsNullOrEmpty(_folder) ? remoteFileName : $"{_folder}/{remoteFileName}";
        var ms = new MemoryStream();
        await _client.DownloadObjectAsync(_bucketName, objectName, ms, cancellationToken: ct);
        ms.Position = 0;
        return ms;
    }
}
