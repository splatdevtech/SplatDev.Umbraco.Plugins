using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Engine;
using SplatDev.Umbraco.Plugins.Backups.Providers;
using SplatDev.Umbraco.Plugins.Backups.Services;

namespace SplatDev.Umbraco.Plugins.Backups.Composers;

public class BackupsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<BackupSettings>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var settings = new BackupSettings();
            config.GetSection("SplatDev:Backups").Bind(settings);
            return settings;
        });
        builder.Services.AddScoped<IBackupsService, BackupsService>();
        builder.Services.AddScoped<IBackupEngine, BackupEngine>();
        builder.Services.AddScoped<ICloudStorageProvider, LocalFileSystemStorageProvider>();
        builder.Services.AddScoped<ICloudStorageProvider, AzureBlobStorageProvider>();
    }
}
