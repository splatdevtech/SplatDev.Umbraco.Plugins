namespace SplatDev.Umbraco.Plugins.Backups.Providers;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Models;

public class AwsS3StorageProvider : ICloudStorageProvider
{
    private readonly BackupSettings _settings;

    public string ProviderName => "AwsS3";
    public string ProviderIcon => "aws-s3";
    public bool RequiresOAuth => false;
    public bool RequiresApiKey => true;

    public AwsS3StorageProvider(BackupSettings settings)
    {
        _settings = settings;
    }

    private (string AccessKey, string SecretKey, string Region, string Bucket, string Prefix) GetConfig()
    {
        var cfg = _settings.CloudProviders
            .FirstOrDefault(c => c.ProviderType == ProviderName && c.Enabled);
        if (cfg is null)
            throw new InvalidOperationException("AWS S3 provider not configured.");

        var accessKey = cfg.Settings.GetValueOrDefault("AccessKeyId") ?? string.Empty;
        var secretKey = cfg.Settings.GetValueOrDefault("SecretAccessKey") ?? string.Empty;
        var region = cfg.Settings.GetValueOrDefault("Region") ?? "us-east-1";
        var bucket = cfg.Settings.GetValueOrDefault("BucketName") ?? string.Empty;
        var prefix = (cfg.Settings.GetValueOrDefault("Prefix") ?? "backups").TrimEnd('/');

        return (accessKey, secretKey, region, bucket, prefix);
    }

    private AmazonS3Client CreateClient()
    {
        var (accessKey, secretKey, region, _, _) = GetConfig();
        var regionEndpoint = RegionEndpoint.GetBySystemName(region);
        return new AmazonS3Client(accessKey, secretKey, regionEndpoint);
    }

    public async Task UploadAsync(Stream data, string remotePath, string fileName, CancellationToken ct)
    {
        var (_, _, _, bucket, prefix) = GetConfig();
        using var client = CreateClient();

        var key = $"{prefix}/{remotePath}/{fileName}".Replace("//", "/").TrimStart('/');
        var request = new PutObjectRequest
        {
            BucketName = bucket,
            Key = key,
            InputStream = data,
            AutoCloseStream = false
        };

        await client.PutObjectAsync(request, ct);
    }

    public async Task<Stream> DownloadAsync(string remotePath, CancellationToken ct)
    {
        var (_, _, _, bucket, _) = GetConfig();
        using var client = CreateClient();

        var response = await client.GetObjectAsync(bucket, remotePath, ct);
        var ms = new MemoryStream();
        await response.ResponseStream.CopyToAsync(ms, ct);
        ms.Position = 0;
        return ms;
    }

    public async Task DeleteAsync(string remotePath, CancellationToken ct)
    {
        var (_, _, _, bucket, _) = GetConfig();
        using var client = CreateClient();

        await client.DeleteObjectAsync(bucket, remotePath, ct);
    }

    public async Task<bool> ValidateConnectionAsync(CancellationToken ct)
    {
        try
        {
            var (_, _, _, bucket, _) = GetConfig();
            using var client = CreateClient();

            var request = new ListObjectsV2Request
            {
                BucketName = bucket,
                MaxKeys = 1
            };

            await client.ListObjectsV2Async(request, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<StorageItem>> ListAsync(string remotePath, CancellationToken ct)
    {
        var (_, _, _, bucket, prefix) = GetConfig();
        try
        {
            using var client = CreateClient();
            var fullPrefix = $"{prefix}/{remotePath}/".Replace("//", "/").TrimStart('/');

            var request = new ListObjectsV2Request
            {
                BucketName = bucket,
                Prefix = fullPrefix
            };

            var response = await client.ListObjectsV2Async(request, ct);

            return response.S3Objects
                .Where(o => !o.Key.EndsWith('/'))
                .Select(o => new StorageItem
                {
                    Name = o.Key.Split('/').Last(),
                    Path = o.Key,
                    SizeBytes = o.Size,
                    LastModified = o.LastModified,
                    IsDirectory = false
                }).ToList();
        }
        catch
        {
            return [];
        }
    }
}
