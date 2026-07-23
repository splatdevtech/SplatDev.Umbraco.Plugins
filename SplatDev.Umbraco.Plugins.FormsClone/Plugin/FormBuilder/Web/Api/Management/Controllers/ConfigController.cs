using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Exposes configuration information required for customziing front-end backoffice functionality.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Config")]
    [Authorize(Policy = "SectionAccessForms")]
    [Route("/formBuilder/management/api/v1/config")]
    public class ConfigController(
      IOptions<FormDesignSettings> formDesignSettings,
      IOptions<SecuritySettings> securitySettings,
      IOptions<PackageOptionSettings> packageOptionSettings) : FormsManagementApiControllerBase
    {
        private readonly FormDesignSettings _formDesignSettings = formDesignSettings.Value;
        private readonly SecuritySettings _securitySettings = securitySettings.Value;
        private readonly PackageOptionSettings _packageOptionSettings = packageOptionSettings.Value;

        /// <summary>
        /// Exposes configuration information required for customziing front-end backoffice functionality.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(BackOfficeConfig), 200)]
        public IActionResult GetConfig() => Ok(new BackOfficeConfig()
        {
            MaxNumberOfColumnsInFormGroup = _formDesignSettings.MaxNumberOfColumnsInFormGroup,
            ManageSecurityWithUserGroups = _securitySettings.ManageSecurityWithUserGroups,
            ScheduledRecordDeletionEnabled = _packageOptionSettings.ScheduledRecordDeletion.Enabled,
            MandatoryFieldsetLegends = _formDesignSettings.MandatoryFieldsetLegends,
            DisallowedFileUploadExtensions = _securitySettings.DisallowedFileUploadExtensions.ToLowerInvariant(),
            AllowedFileUploadExtensions = _securitySettings.AllowedFileUploadExtensions.ToLowerInvariant(),
            EnableMultiPageFormSettings = _packageOptionSettings.EnableMultiPageFormSettings,
            EnableAdvancedValidationRules = _packageOptionSettings.EnableAdvancedValidationRules
        });
    }
}