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
    /// Management API controller for creating a form security record for a user group.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class CreateUserGroupFormSecurityController(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserGroupService userGroupService,
      IUserGroupStartFolderStorage userGroupStartFolderStorage) :
      CreateOrUpdateUserGroupFormSecurityControllerBase(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings, userGroupService, userGroupStartFolderStorage)
    {
        /// <summary>
        /// Management endpoint for creating a form security record for a user group.
        /// </summary>
        [HttpPost("{id:guid}/form-security")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult Create(Guid id, FormSecurityForGroup security)
        {
            CreateOrUpdateFormSecurity(security);
            return Created();
        }
    }
}