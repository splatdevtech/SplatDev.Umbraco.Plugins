#pragma warning disable CS0618
#if !NET10_0_OR_GREATER
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
#endif

namespace SplatDev.Umbraco.Plugins.CodeFirst
{
    [System.Obsolete("CodeFirst migrations are deprecated. Use Yaml2Schema.")]
    public class Migrations
    {
    }

#if !NET10_0_OR_GREATER
    [System.Obsolete("CodeFirst is deprecated. Use Yaml2Schema.")]
    public class MigrationUpgradeComponentComposer : ComponentComposer<MigrationUpgradeComponent>
    {
        public override void Compose(IUmbracoBuilder builder)
        {
            builder.Components().Append<MigrationUpgradeComponent>();
        }
    }

    [System.Obsolete("CodeFirst is deprecated. Use Yaml2Schema.")]
    public class MigrationUpgradeComponent : IComponent
    {
        private readonly ICoreScopeProvider scopeProvider;
        private readonly IMigrationPlanExecutor migrationPlanExecutor;
        private readonly IKeyValueService keyValueService;
        private readonly ILogger<MigrationUpgradeComponent> logger;

        public MigrationUpgradeComponent(
            ICoreScopeProvider scopeProvider,
            IMigrationPlanExecutor migrationPlanExecutor,
            IKeyValueService keyValueService,
            ILogger<MigrationUpgradeComponent> logger)
        {
            this.scopeProvider = scopeProvider;
            this.migrationPlanExecutor = migrationPlanExecutor;
            this.keyValueService = keyValueService;
            this.logger = logger;
        }

        public void Initialize()
        {
            var plan = new MigrationPlan("pluginApiKeys");
            plan.From(string.Empty)
                .To<InitialMigration>("state-0");

            var upgrader = new Upgrader(plan);
            upgrader.Execute(migrationPlanExecutor, scopeProvider, keyValueService);
        }

        public void Terminate() { }
    }
#endif
}
#pragma warning restore CS0618
