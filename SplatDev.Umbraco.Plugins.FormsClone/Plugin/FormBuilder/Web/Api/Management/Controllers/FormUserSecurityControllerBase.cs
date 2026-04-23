using FormBuilder.Core.Configuration;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with form security for users.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Route("/formBuilder/management/api/v1/security/user")]
    public abstract class FormUserSecurityControllerBase(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserService userService,
      IUserSecurityStorage userSecurityStorage,
      IUserFormSecurityStorage userFormSecurityStorage,
      IUserStartFolderStorage userStartFolderStorage) : FormSecurityControllerBase(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings)
    {
        /// <summary>
        /// Gets the         /// </summary>
        protected IUserService UserService { get; } = userService;

        /// <summary>
        /// Gets the         /// </summary>
        protected IUserSecurityStorage UserSecurityStorage { get; } = userSecurityStorage;

        /// <summary>
        /// Gets the         /// </summary>
        protected IUserFormSecurityStorage UserFormSecurityStorage { get; } = userFormSecurityStorage;

        /// <summary>
        /// Gets the         /// </summary>
        protected IUserStartFolderStorage UserStartFolderStorage { get; } = userStartFolderStorage;
    }
}