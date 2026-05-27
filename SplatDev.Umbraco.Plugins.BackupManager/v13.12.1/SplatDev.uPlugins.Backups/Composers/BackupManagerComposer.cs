using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace SplatDev.uPlugins.Backups.Composers;

public class BackupManagerComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // BackupController uses IConfiguration and IWebHostEnvironment — both are
        // standard ASP.NET Core services registered automatically by the host.
        // BackupVault providers are created via BackupStorageProviderFactory.Create()
        // and do not require DI registration.
    }
}
