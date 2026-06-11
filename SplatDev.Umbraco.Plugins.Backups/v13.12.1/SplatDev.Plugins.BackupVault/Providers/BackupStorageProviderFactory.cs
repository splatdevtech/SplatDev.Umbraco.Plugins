namespace SplatDev.Plugins.BackupVault.Providers;

public static class BackupStorageProviderFactory
{
    public static IBackupStorageProvider Create(StorageProviderConfig config) =>
        config.Provider.ToLowerInvariant() switch
        {
            "googlecloud" or "gcs" => new GoogleCloudStorageProvider(config),
            "box" => new BoxStorageProvider(config),
            "dropbox" => new DropboxStorageProvider(config),
            "onedrive" => new OneDriveStorageProvider(config),
            "s3" or "awss3" => new AwsS3StorageProvider(config),
            "azure" or "azureblob" => new AzureBlobStorageProvider(config),
            _ => throw new NotSupportedException($"Storage provider '{config.Provider}' is not supported.")
        };
}
