using FormBuilder.Core.Configuration;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for deleting form security details for a userGroup group.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "SectionAccessUsers")]
    public class DeleteUserGroupFormSecurityController(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserGroupService userGroupService,
      IUserGroupStartFolderStorage userGroupStartFolderStorage) : FormUserGroupSecurityControllerBase(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings, userGroupService, userGroupStartFolderStorage)
    {
        /// <summary>
        /// Management API endpoint for deleting form security details for a userGroup.
        /// </summary>
        [HttpDelete("{id:guid}/form-security")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            DeleteUserGroupFormSecurityController securityController = this;
            var async = await securityController.UserGroupService.GetAsync(id);
            if (async is null)
                return securityController.NotFound();
            var userGroupSecurity = securityController.UserGroupSecurityStorage.GetUserGroupSecurity(async.Id);
            if (userGroupSecurity is null)
                return securityController.NotFound();
            securityController.UserGroupSecurityStorage.DeleteUserGroupSecurity(userGroupSecurity);
            securityController.UserGroupStartFolderStorage.UpdateStartFolders(async.Id, []);
            securityController.UserGroupFormSecurityStorage.DeleteAllUserGroupFormSecurityForUserGroup(async.Id);
            return securityController.Ok();
        }
    }
}