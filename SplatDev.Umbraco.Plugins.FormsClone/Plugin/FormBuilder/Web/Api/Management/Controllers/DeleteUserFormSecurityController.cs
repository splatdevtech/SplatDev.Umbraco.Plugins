using FormBuilder.Core.Configuration;
using FormBuilder.Core.Persistence.Security;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Models.Membership;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for deleting form security details for a user.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "SectionAccessUsers")]
    public class DeleteUserFormSecurityController(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserService userService,
      IUserSecurityStorage userSecurityStorage,
      IUserFormSecurityStorage userFormSecurityStorage,
      IUserStartFolderStorage userStartFolderStorage) : FormUserSecurityControllerBase(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings, userService, userSecurityStorage, userFormSecurityStorage, userStartFolderStorage)
    {
        /// <summary>
        /// Management API endpoint for deleting form security details for a user.
        /// </summary>
        [HttpDelete("{id:guid}/form-security")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            DeleteUserFormSecurityController securityController = this;
            IUser? async = await securityController.UserService.GetAsync(id);
            if (async is null)
                return securityController.NotFound();
            UserSecurity? userSecurity = securityController.UserSecurityStorage.GetUserSecurity(async.Id);
            if (userSecurity is null)
                return securityController.NotFound();
            securityController.UserSecurityStorage.DeleteUserSecurity(userSecurity);
            securityController.UserStartFolderStorage.UpdateStartFolders(async.Id, []);
            securityController.UserFormSecurityStorage.DeleteAllUserFormSecurityForUser(async.Id);
            return securityController.Ok();
        }
    }
}