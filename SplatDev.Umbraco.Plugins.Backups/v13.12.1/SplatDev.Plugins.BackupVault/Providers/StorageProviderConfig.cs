namespace SplatDev.Plugins.BackupVault.Providers;

public class StorageProviderConfig
{
    public string Provider { get; set; } = string.Empty;
    public string? BucketOrContainer { get; set; }
    public string? Folder { get; set; }
    // Google Cloud
    public string? GoogleCredentialsJson { get; set; }
    // Box
    public string? BoxClientId { get; set; }
    public string? BoxClientSecret { get; set; }
    public string? BoxAccessToken { get; set; }
    public string? BoxFolderId { get; set; }
    // Dropbox
    public string? DropboxAccessToken { get; set; }
    // OneDrive
    public string? OneDriveTenantId { get; set; }
    public string? OneDriveClientId { get; set; }
    public string? OneDriveClientSecret { get; set; }
    public string? OneDriveDriveId { get; set; }
    // AWS S3
    public string? AwsAccessKeyId { get; set; }
    public string? AwsSecretAccessKey { get; set; }
    public string? AwsRegion { get; set; } = "us-east-1";
    // Azure Blob
    public string? AzureConnectionString { get; set; }
    public string? AzureAccountName { get; set; }
    public string? AzureAccountKey { get; set; }
}
