using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using SplatDev.Umbraco.Plugins.RedirectManager.Migrations;
using SplatDev.Umbraco.Plugins.RedirectManager.Models;

namespace SplatDev.Umbraco.Plugins.RedirectManager.Components
{
    public class RedirectUrlsComponent(
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

            var migrationPlan = new MigrationPlan(RedirectUrl.TABLE_NAME);

            migrationPlan.From(string.Empty)
                .To<RedirectUrlsMigration>("redirectUrls-db");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
        }

        public void Terminate()
        {
        }
    }
}
