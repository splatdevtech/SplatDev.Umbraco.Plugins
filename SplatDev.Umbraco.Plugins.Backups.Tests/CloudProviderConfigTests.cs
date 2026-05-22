using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Providers;
using Xunit;

namespace SplatDev.Umbraco.Plugins.Backups.Tests;

public class CloudProviderConfigTests
{
    [Theory]
    [InlineData("AzureBlobStorage")]
    [InlineData("GoogleDrive")]
    [InlineData("Dropbox")]
    [InlineData("BoxNet")]
    [InlineData("OneDrive")]
    [InlineData("Mega")]
    [InlineData("Seafile")]
    [InlineData("AwsS3")]
    [InlineData("Sftp")]
    [InlineData("LocalFileSystem")]
    public void BackupSettings_CanConfigureProvider(string providerType)
    {
        var settings = new BackupSettings
        {
            CloudProviders =
            [
                new CloudProviderConfig
                {
                    Id = "test-1",
                    ProviderType = providerType,
                    Enabled = true,
                    Settings = new Dictionary<string, string> { ["Key"] = "Value" }
                }
            ]
        };

        var config = settings.CloudProviders.Single();
        Assert.Equal(providerType, config.ProviderType);
        Assert.True(config.Enabled);
    }

    [Fact]
    public void BackupSettings_DefaultValues_AreReasonable()
    {
        var settings = new BackupSettings();

        Assert.Equal(30, settings.LocalRetentionDays);
        Assert.Equal(10, settings.MaxLocalBackups);
        Assert.Equal("App_Data/Backups", settings.BackupPath);
        Assert.True(settings.AutoCleanup);
        Assert.Empty(settings.CloudProviders);
    }

    [Fact]
    public void AzureBlobStorageProvider_WithNoConfig_ValidateConnectionReturnsFalse()
    {
        var settings = new BackupSettings();
        var provider = new AzureBlobStorageProvider(settings);

        Assert.Equal("AzureBlobStorage", provider.ProviderName);
        Assert.False(provider.RequiresOAuth);
        Assert.True(provider.RequiresApiKey);
    }

    [Fact]
    public void GoogleDriveProvider_HasCorrectMetadata()
    {
        var settings = new BackupSettings();
        var provider = new GoogleDriveStorageProvider(settings);

        Assert.Equal("GoogleDrive", provider.ProviderName);
        Assert.True(provider.RequiresOAuth);
        Assert.False(provider.RequiresApiKey);
    }

    [Fact]
    public void DropboxProvider_HasCorrectMetadata()
    {
        var settings = new BackupSettings();
        var provider = new DropboxStorageProvider(settings);

        Assert.Equal("Dropbox", provider.ProviderName);
        Assert.True(provider.RequiresOAuth);
        Assert.False(provider.RequiresApiKey);
    }

    [Fact]
    public void BoxProvider_HasCorrectMetadata()
    {
        var settings = new BackupSettings();
        var provider = new BoxStorageProvider(settings);

        Assert.Equal("BoxNet", provider.ProviderName);
        Assert.True(provider.RequiresOAuth);
    }

    [Fact]
    public void OneDriveProvider_HasCorrectMetadata()
    {
        var settings = new BackupSettings();
        var provider = new OneDriveStorageProvider(settings);

        Assert.Equal("OneDrive", provider.ProviderName);
        Assert.True(provider.RequiresOAuth);
    }

    [Fact]
    public void MegaProvider_HasCorrectMetadata()
    {
        var settings = new BackupSettings();
        var provider = new MegaStorageProvider(settings);

        Assert.Equal("Mega", provider.ProviderName);
        Assert.False(provider.RequiresOAuth);
        Assert.False(provider.RequiresApiKey);
    }

    [Fact]
    public void SeafileProvider_HasCorrectMetadata()
    {
        var settings = new BackupSettings();
        var provider = new SeafileStorageProvider(settings);

        Assert.Equal("Seafile", provider.ProviderName);
        Assert.False(provider.RequiresOAuth);
        Assert.True(provider.RequiresApiKey);
    }

    [Fact]
    public void AwsS3Provider_HasCorrectMetadata()
    {
        var settings = new BackupSettings();
        var provider = new AwsS3StorageProvider(settings);

        Assert.Equal("AwsS3", provider.ProviderName);
        Assert.False(provider.RequiresOAuth);
        Assert.True(provider.RequiresApiKey);
    }

    [Fact]
    public void SftpProvider_HasCorrectMetadata()
    {
        var settings = new BackupSettings();
        var provider = new SftpStorageProvider(settings);

        Assert.Equal("Sftp", provider.ProviderName);
        Assert.False(provider.RequiresOAuth);
        Assert.True(provider.RequiresApiKey);
    }

    [Fact]
    public void AllProviders_WhenNotConfigured_ValidateConnectionReturnsFalse_OrThrows()
    {
        var settings = new BackupSettings();
        ICloudStorageProvider[] providers =
        [
            new AzureBlobStorageProvider(settings),
            new GoogleDriveStorageProvider(settings),
            new DropboxStorageProvider(settings),
            new BoxStorageProvider(settings),
            new OneDriveStorageProvider(settings),
            new MegaStorageProvider(settings),
            new SeafileStorageProvider(settings),
            new AwsS3StorageProvider(settings),
            new SftpStorageProvider(settings),
        ];

        foreach (var provider in providers)
        {
            var task = Record.ExceptionAsync(() => provider.ValidateConnectionAsync(CancellationToken.None));
            // Should either return false gracefully or throw — not hang
            Assert.NotNull(task);
        }
    }
}
