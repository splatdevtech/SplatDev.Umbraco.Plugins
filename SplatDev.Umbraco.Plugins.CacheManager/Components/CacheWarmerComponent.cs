using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using SplatDev.Umbraco.Plugins.CacheManager.Migrations;
using SplatDev.Umbraco.Plugins.CacheManager.Models;

namespace SplatDev.Umbraco.Plugins.CacheManager.Components
{
    public class CacheWarmerComponent(
        ICoreScopeProvider coreScopeProvider,
        IMigrationPlanExecutor migrationPlanExecutor,
        IKeyValueService keyValueService,
        IRuntimeState runtimeState) : IComponent
    {
        private readonly ICoreScopeProvider _coreScopeProvider = coreScopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor = migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService = keyValueService;
        private readonly IRuntimeState _runtimeState = runtimeState;

        public void Initialize()
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }

            // Create a migration plan for a specific project/feature
            // We can then track that latest migration state/step for this project/feature
            var migrationPlan = new MigrationPlan(CacheWarmerEntry.TABLE_NAME);

            // This is the steps we need to take
            // Each step in the migration adds a unique value
            migrationPlan.From(string.Empty)
                .To<CacheWarmerMigration>("cacheWarmer-db")
                .To<UrlNotFoundMigration>("urlNotFound-db");

            // Go and upgrade our site (Will check if it needs to do the work or not)
            // Based on the current/latest step
            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
        }

        public void Terminate()
        {
        }
    }
}
