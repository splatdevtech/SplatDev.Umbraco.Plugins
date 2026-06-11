using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace SplatDev.Plugins.BackupVault.Providers;

public class AwsS3StorageProvider : IBackupStorageProvider
{
    private readonly AmazonS3Client _client;
    private readonly string _bucketName;
    private readonly string _folder;

    public string ProviderName => "AWS S3";

    public AwsS3StorageProvider(StorageProviderConfig config)
    {
        _bucketName = config.BucketOrContainer ?? throw new ArgumentNullException(nameof(config.BucketOrContainer));
        _folder = config.Folder ?? "backups";

        var region = RegionEndpoint.GetBySystemName(config.AwsRegion ?? "us-east-1");
        _client = new AmazonS3Client(
            config.AwsAccessKeyId ?? throw new ArgumentNullException(nameof(config.AwsAccessKeyId)),
            config.AwsSecretAccessKey ?? throw new ArgumentNullException(nameof(config.AwsSecretAccessKey)),
            region);
    }

    public async Task<bool> UploadAsync(string localFilePath, string remoteFileName, CancellationToken ct = default)
    {
        var key = string.IsNullOrEmpty(_folder) ? remoteFileName : $"{_folder}/{remoteFileName}";
        var transfer = new TransferUtility(_client);
        await transfer.UploadAsync(localFilePath, _bucketName, key, ct);
        return true;
    }

    public async Task<IEnumerable<string>> ListAsync(string remoteFolder = "", CancellationToken ct = default)
    {
        var prefix = string.IsNullOrEmpty(remoteFolder) ? _folder : remoteFolder;
        var request = new ListObjectsV2Request { BucketName = _bucketName, Prefix = prefix };
        var response = await _client.ListObjectsV2Async(request, ct);
        return response.S3Objects.Select(o => o.Key);
    }

    public async Task<bool> DeleteAsync(string remoteFileName, CancellationToken ct = default)
    {
        var key = string.IsNullOrEmpty(_folder) ? remoteFileName : $"{_folder}/{remoteFileName}";
        await _client.DeleteObjectAsync(_bucketName, key, ct);
        return true;
    }

    public async Task<Stream> DownloadAsync(string remoteFileName, CancellationToken ct = default)
    {
        var key = string.IsNullOrEmpty(_folder) ? remoteFileName : $"{_folder}/{remoteFileName}";
        var response = await _client.GetObjectAsync(_bucketName, key, ct);
        var ms = new MemoryStream();
        await response.ResponseStream.CopyToAsync(ms, ct);
        ms.Position = 0;
        return ms;
    }
}
