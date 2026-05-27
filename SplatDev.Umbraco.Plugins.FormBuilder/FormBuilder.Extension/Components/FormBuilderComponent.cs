using FormBuilder.Extension.Migrations;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace FormBuilder.Extension.Components
{
    public class FormBuilderMigrationComposer : ComponentComposer<FormBuilderComponent>
    {
    }

#if NET10_0_OR_GREATER
    public class FormBuilderComponent(
        ICoreScopeProvider coreScopeProvider,
        IMigrationPlanExecutor migrationPlanExecutor,
        IKeyValueService keyValueService,
        IRuntimeState runtimeState) : IAsyncComponent
    {
        private readonly ICoreScopeProvider _coreScopeProvider = coreScopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor = migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService = keyValueService;
        private readonly IRuntimeState _runtimeState = runtimeState;
        private const string MigrationPlanName = "formBuilderMigration";

        public Task InitializeAsync(bool isRestarting, CancellationToken cancellationToken)
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
                return Task.CompletedTask;

            var migrationPlan = new MigrationPlan(MigrationPlanName);
            migrationPlan.From(string.Empty)
                .To<FormBuilderMigration>($"{MigrationPlanName}-db");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
            return Task.CompletedTask;
        }

        public Task TerminateAsync(bool isRestarting, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
#else
    public class FormBuilderComponent(
        ICoreScopeProvider coreScopeProvider,
        IMigrationPlanExecutor migrationPlanExecutor,
        IKeyValueService keyValueService,
        IRuntimeState runtimeState) : Umbraco.Cms.Core.Composing.IComponent
    {
        private readonly ICoreScopeProvider _coreScopeProvider = coreScopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor = migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService = keyValueService;
        private readonly IRuntimeState _runtimeState = runtimeState;
        private const string MigrationPlanName = "formBuilderMigration";

        public void Initialize()
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
                return;

            var migrationPlan = new MigrationPlan(MigrationPlanName);
            migrationPlan.From(string.Empty)
                .To<FormBuilderMigration>($"{MigrationPlanName}-db");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
        }

        public void Terminate()
        {
        }
    }
#endif
}
