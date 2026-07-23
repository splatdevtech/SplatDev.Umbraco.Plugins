using System.Threading.Tasks;
﻿using Microsoft.Extensions.Logging;

using Umbraco.Cms.Infrastructure.Migrations;
using SplatDev.Umbraco.Plugins.CacheManager.Models;

namespace SplatDev.Umbraco.Plugins.CacheManager.Migrations
{
    public class CacheWarmerMigration(IMigrationContext context, ILogger<CacheWarmerMigration> logger) : MigrationBase(context)
    {
        private readonly ILogger<CacheWarmerMigration> _logger = logger;

        protected override void Migrate()
        {
            _logger.LogDebug("Running migration {MigrationStep}", "AddCacheManager");

            // Lots of methods available in the MigrationBase class - discover with this.
            if (TableExists(CacheWarmerEntry.TABLE_NAME) == false)
            {
                Create.Table<CacheWarmerEntry>().Do();
            }
            else
            {
                _logger.LogDebug("The database table {DbTable} already exists, skipping", CacheWarmerEntry.TABLE_NAME);
            }
        }
    }
}