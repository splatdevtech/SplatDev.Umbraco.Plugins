using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Models.Membership;

using Umbraco.Cms.Core.Security;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a single set of form security details for the current user.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "BackOfficeAccess")]
    public class GetForCurrentUserFormSecurityController(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserService userService,
      IUserSecurityStorage userSecurityStorage,
      IUserFormSecurityStorage userFormSecurityStorage,
      IUserStartFolderStorage userStartFolderStorage,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor) : GetUserFormSecurityControllerBase(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings, userService, userSecurityStorage, userFormSecurityStorage, userStartFolderStorage)
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor = backOfficeSecurityAccessor;

        /// <summary>
        /// Management API endpoint for retrieving a single set of form security details for the current user.
        /// </summary>
        [HttpGet("current/form-security")]
        [ProducesResponseType(typeof(FormSecurityForUser), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCurrent(bool includeFormFieldDetails)
        {
            GetForCurrentUserFormSecurityController securityController = this;
            IUser? currentUser = securityController._backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            if (currentUser is null)
                return securityController.NotFound();
            var securityByUserKey = await securityController.GetFormSecurityByUserKey(currentUser.Key, false, includeFormFieldDetails);
            return securityByUserKey is not null ? securityController.Ok(securityByUserKey) : securityController.NotFound();
        }
    }
}