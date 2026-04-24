using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for updating a form security record for a user.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class UpdateUserFormSecurityController(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserService userService,
      IUserSecurityStorage userSecurityStorage,
      IUserFormSecurityStorage userFormSecurityStorage,
      IUserStartFolderStorage userStartFolderStorage) : CreateOrUpdateUserFormSecurityControllerBase(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings, userService, userSecurityStorage, userFormSecurityStorage, userStartFolderStorage)
    {
        /// <summary>
        /// Management endpoint for updating a form security record for a user.
        /// </summary>
        [HttpPut("{id:guid}/form-security")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> Update(Guid id, FormSecurityForUser security)
        {
            UpdateUserFormSecurityController securityController = this;
            var async = await securityController.UserService.GetAsync(id);
            if (async is null)
                return securityController.NotFound();
            securityController.CreateOrUpdateFormSecurity(async.Id, security);
            return securityController.Ok();
        }
    }
}