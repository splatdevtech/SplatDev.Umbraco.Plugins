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
    [Route("/formBuilder/management/api/v1/security/user-group")]
    public abstract class FormUserGroupSecurityControllerBase(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserGroupService userGroupService,
      IUserGroupStartFolderStorage userGroupStartFolderStorage) : FormSecurityControllerBase(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings)
    {
        /// <summary>
        /// Gets the         /// </summary>
        protected IUserGroupService UserGroupService { get; } = userGroupService;

        /// <summary>
        /// Gets the         /// </summary>
        public IUserGroupStartFolderStorage UserGroupStartFolderStorage { get; } = userGroupStartFolderStorage;
    }
}