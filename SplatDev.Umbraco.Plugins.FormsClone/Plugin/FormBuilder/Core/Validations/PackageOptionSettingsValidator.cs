using FormBuilder.Core.Configuration;

using Microsoft.Extensions.Options;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Configuration;
using Umbraco.Cms.Core.Configuration.Models.Validation;

namespace FormBuilder.Core.Validations
{
    public class PackageOptionSettingsValidator(ICronTabParser cronTabParser) :
        ConfigurationValidatorBase,
        IValidateOptions<PackageOptionSettings>
    {
        private readonly ICronTabParser _cronTabParser = cronTabParser;

        public ValidateOptionsResult Validate(
          string? name,
          PackageOptionSettings options)
        {
            return !ValidateScheduledRecordDeletionFirstRunTime(options.ScheduledRecordDeletion.FirstRunTime, out string message) ? ValidateOptionsResult.Fail(message) : ValidateOptionsResult.Success;
        }

        private bool ValidateScheduledRecordDeletionFirstRunTime(string value, out string message)
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(2, 3);
            interpolatedStringHandler.AppendFormatted(Constants.Configuration.SectionKeys.PackageOptions);
            interpolatedStringHandler.AppendLiteral(":");
            interpolatedStringHandler.AppendFormatted("ScheduledRecordDeletion");
            interpolatedStringHandler.AppendLiteral(":");
            interpolatedStringHandler.AppendFormatted("FirstRunTime");
            return ValidateOptionalCronTab(interpolatedStringHandler.ToStringAndClear(), value, out message);
        }

        private bool ValidateOptionalCronTab(string configPath, string value, out string message)
        {
            if (!string.IsNullOrEmpty(value) && !_cronTabParser.IsValidCronTab(value))
            {
                message = "Configuration entry " + configPath + " contains an invalid cron expression.";
                return false;
            }
            message = string.Empty;
            return true;
        }
    }
}