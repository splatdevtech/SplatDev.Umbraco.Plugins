using Microsoft.Extensions.Logging;

using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Plugins.CacheManager.Models;

namespace Umbraco.Plugins.CacheManager.Migrations
{
    public class UrlNotFoundMigration(IMigrationContext context, ILogger<UrlNotFoundMigration> logger) : MigrationBase(context)
    {
        private readonly ILogger<UrlNotFoundMigration> _logger = logger;

        protected override void Migrate()
        {
            _logger.LogDebug("Running migration {MigrationStep}", "UrlNotFound");

            // Lots of methods available in the MigrationBase class - discover with this.
            if (TableExists(UrlNotFound.TABLE_NAME) == false)
            {
                Create.Table<UrlNotFound>().Do();
            }
            else
            {
                _logger.LogDebug("The database table {DbTable} already exists, skipping", UrlNotFound.TABLE_NAME);
            }
        }
    }
}