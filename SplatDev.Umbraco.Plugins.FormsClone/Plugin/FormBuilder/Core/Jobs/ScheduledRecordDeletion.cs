using FormBuilder.Core.Configuration;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Services.Results;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration;
using Umbraco.Cms.Core.Runtime;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.HostedServices;

using static FormBuilder.Constants.Configuration;

namespace FormBuilder.Core.Jobs
{
    internal sealed class ScheduledRecordDeletion(
      IRuntimeState runtimeState,
      IServerRoleAccessor serverRoleAccessor,
      IMainDom mainDom,
      ILogger<ScheduledRecordDeletion> logger,
      IOptions<PackageOptionSettings> packageOptionSettings,
      ICronTabParser cronTabParser,
      IScheduledRecordDeletionService scheduledRecordDeletionService) : RecurringHostedServiceBase(logger, packageOptionSettings.Value.ScheduledRecordDeletion.Period, GetDelay(packageOptionSettings.Value.ScheduledRecordDeletion.FirstRunTime, cronTabParser, logger, DefaultDelay))
    {
        private readonly IRuntimeState _runtimeState = runtimeState;
        private readonly IServerRoleAccessor _serverRoleAccessor = serverRoleAccessor;
        private readonly IMainDom _mainDom = mainDom;
        private readonly ILogger<ScheduledRecordDeletion> _logger = logger;
        private readonly IScheduledRecordDeletionService _scheduledRecordDeletionService = scheduledRecordDeletionService;
        private readonly PackageOptionSettings _packageOptionSettings = packageOptionSettings.Value;

        public override Task PerformExecuteAsync(object? state)
        {
            if (!_packageOptionSettings.ScheduledRecordDeletion.Enabled)
            {
                ILogger<ScheduledRecordDeletion> logger = _logger;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(159, 3);
                interpolatedStringHandler.AppendLiteral("Umbraco Forms scheduled record deletion task will not run as it has been not enabled via configuration. To enable, set the configuration value at '");
                interpolatedStringHandler.AppendFormatted(SectionKeys.PackageOptions);
                interpolatedStringHandler.AppendLiteral(":");
                interpolatedStringHandler.AppendFormatted(nameof(ScheduledRecordDeletion));
                interpolatedStringHandler.AppendLiteral(":");
                interpolatedStringHandler.AppendFormatted("Enabled");
                interpolatedStringHandler.AppendLiteral("' to true.");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                object[] objArray = [];
                logger.LogInformation(stringAndClear, objArray);
                return Task.CompletedTask;
            }
            if (_runtimeState.Level != RuntimeLevel.Run)
                return Task.FromResult(true);
            switch (_serverRoleAccessor.CurrentServerRole)
            {
                case ServerRole.Unknown:
                    _logger.LogDebug("Umbraco Forms scheduled record deletion task will not run on servers with unknown role.");
                    return Task.CompletedTask;

                case ServerRole.Subscriber:
                    _logger.LogDebug("Umbraco Forms scheduled record deletion task will not run on subscriber servers.");
                    return Task.CompletedTask;

                default:
                    if (!_mainDom.IsMainDom)
                    {
                        _logger.LogDebug("Umbraco Forms scheduled record deletion task will not run if not MainDom.");
                        return Task.FromResult(false);
                    }
                    ScheduledRecordDeletionResult recordDeletionResult = _scheduledRecordDeletionService.PerformScheduledRecordDeletion(DateTime.Now);
                    if (recordDeletionResult.HasDeletedRecords)
                        _logger.LogInformation("Umbraco Forms scheduled record deletion task complete, {RecordCount} records were deleted across {FormCount} forms.", recordDeletionResult.TotalRecordCount, recordDeletionResult.TotalFormCount);
                    else
                        _logger.LogDebug("Umbraco Forms scheduled record deletion task complete, no records were deleted.");
                    return Task.FromResult(true);
            }
        }
    }
}