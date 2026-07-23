using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a single set of form security details for a user.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "SectionAccessUsers")]
    public class GetByUserKeyFormSecurityController(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserService userService,
      IUserSecurityStorage userSecurityStorage,
      IUserFormSecurityStorage userFormSecurityStorage,
      IUserStartFolderStorage userStartFolderStorage) : GetUserFormSecurityControllerBase(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings, userService, userSecurityStorage, userFormSecurityStorage, userStartFolderStorage)
    {
        /// <summary>
        /// Management API endpoint for retrieving a single set of form security details for a user by user Id.
        /// </summary>
        [HttpGet("{id:guid}/form-security")]
        [ProducesResponseType(typeof(FormSecurityForUser), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByUserKey(Guid id, bool explicitOnly)
        {
            GetByUserKeyFormSecurityController securityController = this;
            FormSecurityForUser? securityByUserKey = await securityController.GetFormSecurityByUserKey(id, explicitOnly, true);
            return securityByUserKey is not null ? securityController.Ok(securityByUserKey) : securityController.NotFound();
        }
    }
}