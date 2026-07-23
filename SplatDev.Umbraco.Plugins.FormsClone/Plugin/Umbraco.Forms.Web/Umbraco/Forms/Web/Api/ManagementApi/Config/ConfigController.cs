
// Type: Umbraco.Forms.Web.Api.ManagementApi.Config.ConfigController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Config
{
  [ApiExplorerSettings(GroupName = "Config")]
  [Authorize(Policy = "SectionAccessForms")]
  [Route("/umbraco/forms/management/api/v1/config")]
  public class ConfigController : FormsManagementApiControllerBase
  {
    private readonly FormDesignSettings _formDesignSettings;
    private readonly SecuritySettings _securitySettings;
    private readonly PackageOptionSettings _packageOptionSettings;

    public ConfigController(
      IOptions<FormDesignSettings> formDesignSettings,
      IOptions<SecuritySettings> securitySettings,
      IOptions<PackageOptionSettings> packageOptionSettings)
    {
      this._formDesignSettings = formDesignSettings.Value;
      this._securitySettings = securitySettings.Value;
      this._packageOptionSettings = packageOptionSettings.Value;
    }

    [HttpGet]
    [ProducesResponseType(typeof (BackOfficeConfig), 200)]
    public IActionResult GetConfig() => (IActionResult) this.Ok((object) new BackOfficeConfig()
    {
      MaxNumberOfColumnsInFormGroup = this._formDesignSettings.MaxNumberOfColumnsInFormGroup,
      ManageSecurityWithUserGroups = this._securitySettings.ManageSecurityWithUserGroups,
      ScheduledRecordDeletionEnabled = this._packageOptionSettings.ScheduledRecordDeletion.Enabled,
      MandatoryFieldsetLegends = this._formDesignSettings.MandatoryFieldsetLegends,
      DisallowedFileUploadExtensions = this._securitySettings.DisallowedFileUploadExtensions.ToLowerInvariant(),
      AllowedFileUploadExtensions = this._securitySettings.AllowedFileUploadExtensions.ToLowerInvariant(),
      EnableMultiPageFormSettings = this._packageOptionSettings.EnableMultiPageFormSettings,
      EnableAdvancedValidationRules = this._packageOptionSettings.EnableAdvancedValidationRules
    });
  }
}
