
// Type: Umbraco.Forms.Core.Configuration.Validation.PackageOptionSettingsValidator
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using Umbraco.Cms.Core.Configuration;
using Umbraco.Cms.Core.Configuration.Models.Validation;


#nullable enable
namespace Umbraco.Forms.Core.Configuration.Validation
{
  public class PackageOptionSettingsValidator : 
    ConfigurationValidatorBase,
    IValidateOptions<PackageOptionSettings>
  {
    private readonly ICronTabParser _cronTabParser;

    public PackageOptionSettingsValidator(ICronTabParser cronTabParser) => this._cronTabParser = cronTabParser;

    public ValidateOptionsResult Validate(
      string? name,
      PackageOptionSettings options)
    {
      string message;
      return !this.ValidateScheduledRecordDeletionFirstRunTime(options.ScheduledRecordDeletion.FirstRunTime, out message) ? ValidateOptionsResult.Fail(message) : ValidateOptionsResult.Success;
    }

    private bool ValidateScheduledRecordDeletionFirstRunTime(string value, out string message)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
      interpolatedStringHandler.AppendFormatted(Constants.Configuration.SectionKeys.PackageOptions);
      interpolatedStringHandler.AppendLiteral(":");
      interpolatedStringHandler.AppendFormatted("ScheduledRecordDeletion");
      interpolatedStringHandler.AppendLiteral(":");
      interpolatedStringHandler.AppendFormatted("FirstRunTime");
      return this.ValidateOptionalCronTab(interpolatedStringHandler.ToStringAndClear(), value, out message);
    }

    private bool ValidateOptionalCronTab(string configPath, string value, out string message)
    {
      if (!string.IsNullOrEmpty(value) && !this._cronTabParser.IsValidCronTab(value))
      {
        message = "Configuration entry " + configPath + " contains an invalid cron expression.";
        return false;
      }
      message = string.Empty;
      return true;
    }
  }
}
