
// Type: Umbraco.Forms.Core.HostedServices.ScheduledRecordDeletion
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration;
using Umbraco.Cms.Core.Runtime;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.HostedServices;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Services;

#nullable enable
namespace Umbraco.Forms.Core.HostedServices
{
    internal sealed class ScheduledRecordDeletion : RecurringHostedServiceBase
    {
        private readonly IRuntimeState _runtimeState;
        private readonly IServerRoleAccessor _serverRoleAccessor;
        private readonly IMainDom _mainDom;
        private readonly ILogger<ScheduledRecordDeletion> _logger;
        private readonly IScheduledRecordDeletionService _scheduledRecordDeletionService;
        private readonly PackageOptionSettings _packageOptionSettings;

        public ScheduledRecordDeletion(
          IRuntimeState runtimeState,
          IServerRoleAccessor serverRoleAccessor,
          IMainDom mainDom,
          ILogger<ScheduledRecordDeletion> logger,
          IOptions<PackageOptionSettings> packageOptionSettings,
          ICronTabParser cronTabParser,
          IScheduledRecordDeletionService scheduledRecordDeletionService)
          : base(logger, packageOptionSettings.Value.ScheduledRecordDeletion.Period, RecurringHostedServiceBase.GetDelay(packageOptionSettings.Value.ScheduledRecordDeletion.FirstRunTime, cronTabParser, logger, RecurringHostedServiceBase.DefaultDelay))
        {
            this._runtimeState = runtimeState;
            this._serverRoleAccessor = serverRoleAccessor;
            this._mainDom = mainDom;
            this._logger = logger;
            this._scheduledRecordDeletionService = scheduledRecordDeletionService;
            this._packageOptionSettings = packageOptionSettings.Value;
        }

        public override Task PerformExecuteAsync(object? state)
        {
            if (!this._packageOptionSettings.ScheduledRecordDeletion.Enabled)
            {
                ILogger<ScheduledRecordDeletion> logger = this._logger;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(159, 3);
                interpolatedStringHandler.AppendLiteral("Umbraco Forms scheduled record deletion task will not run as it has been not enabled via configuration. To enable, set the configuration value at '");
                interpolatedStringHandler.AppendFormatted(Umbraco.Forms.Core.Constants.Configuration.SectionKeys.PackageOptions);
                interpolatedStringHandler.AppendLiteral(":");
                interpolatedStringHandler.AppendFormatted(nameof(ScheduledRecordDeletion));
                interpolatedStringHandler.AppendLiteral(":");
                interpolatedStringHandler.AppendFormatted("Enabled");
                interpolatedStringHandler.AppendLiteral("' to true.");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                object[] objArray = Array.Empty<object>();
                logger.LogInformation(stringAndClear, objArray);
                return Task.CompletedTask;
            }
            if (this._runtimeState.Level != RuntimeLevel.Run)
                return Task.FromResult<bool>(true);
            switch (this._serverRoleAccessor.CurrentServerRole)
            {
                case ServerRole.Unknown:
                    this._logger.LogDebug("Umbraco Forms scheduled record deletion task will not run on servers with unknown role.");
                    return Task.CompletedTask;
                case ServerRole.Subscriber:
                    this._logger.LogDebug("Umbraco Forms scheduled record deletion task will not run on subscriber servers.");
                    return Task.CompletedTask;
                default:
                    if (!this._mainDom.IsMainDom)
                    {
                        this._logger.LogDebug("Umbraco Forms scheduled record deletion task will not run if not MainDom.");
                        return Task.FromResult<bool>(false);
                    }
                    ScheduledRecordDeletionResult recordDeletionResult = this._scheduledRecordDeletionService.PerformScheduledRecordDeletion(DateTime.Now);
                    if (recordDeletionResult.HasDeletedRecords)
                        this._logger.LogInformation("Umbraco Forms scheduled record deletion task complete, {RecordCount} records were deleted across {FormCount} forms.", recordDeletionResult.TotalRecordCount, recordDeletionResult.TotalFormCount);
                    else
                        this._logger.LogDebug("Umbraco Forms scheduled record deletion task complete, no records were deleted.");
                    return Task.FromResult<bool>(true);
            }
        }
    }
}
