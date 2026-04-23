namespace UmbracoCms.CodeFirst
{
    using Microsoft.Extensions.Logging;

    using Umbraco.Cms.Core.Composing;
    using Umbraco.Cms.Core.Services;
    using Umbraco.Cms.Infrastructure.Migrations;
    using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
    using Umbraco.Cms.Infrastructure.Scoping;

    public class Migrations
    {

    }

    public class MigrationUpgradeComponentComposer : ComponentComposer<MigrationUpgradeComponent>
    {
        public override void Compose(IUmbracoBuilder builder)
        {
            builder.Components().Append<MigrationUpgradeComponent>();
        }
    }

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
            //.To<NoAppIdMigration>("state-1");

            var upgrader = new Upgrader(plan);
            upgrader.Execute(migrationPlanExecutor, scopeProvider, keyValueService);
        }

        public void Terminate() { }
    }
}
