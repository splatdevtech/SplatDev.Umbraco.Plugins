using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Models.Membership;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for creating a form security record for a user.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class CreateUserFormSecurityController(
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
        /// Management endpoint for creating a form security record for a user.
        /// </summary>
        [HttpPost("{id:guid}/form-security")]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> Create(Guid id, FormSecurityForUser security)
        {
            CreateUserFormSecurityController securityController = this;
            IUser? async = await securityController.UserService.GetAsync(id);
            if (async is null)
                return securityController.NotFound();
            securityController.CreateOrUpdateFormSecurity(async.Id, security);
            return securityController.Created();
        }
    }
}