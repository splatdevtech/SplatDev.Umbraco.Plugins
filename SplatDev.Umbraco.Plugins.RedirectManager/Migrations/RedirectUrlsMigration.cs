using Microsoft.Extensions.Logging;

using Umbraco.Cms.Infrastructure.Migrations;
using SplatDev.Umbraco.Plugins.RedirectManager.Models;

namespace SplatDev.Umbraco.Plugins.RedirectManager.Migrations
{
    public class RedirectUrlsMigration(IMigrationContext context, ILogger<RedirectUrlsMigration> logger) : MigrationBase(context)
    {
        private readonly ILogger<RedirectUrlsMigration> _logger = logger;

        protected override void Migrate()
        {
            _logger.LogDebug("Running migration {MigrationStep}", "RedirectionUrls");

            // Lots of methods available in the MigrationBase class - discover with this.
            if (TableExists(RedirectUrl.TABLE_NAME) == false)
            {
                Create.Table<RedirectUrl>().Do();
            }
            else
            {
                _logger.LogDebug("The database table {DbTable} already exists, skipping", RedirectUrl.TABLE_NAME);
            }
        }
    }
}