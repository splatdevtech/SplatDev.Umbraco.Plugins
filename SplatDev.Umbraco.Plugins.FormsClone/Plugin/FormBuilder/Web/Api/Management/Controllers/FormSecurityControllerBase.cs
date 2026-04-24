using FormBuilder.Core.Configuration;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with form security.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Security")]
    public abstract class FormSecurityControllerBase(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings) : FormsManagementApiControllerBase
    {
        /// <summary>
        /// Gets the         /// </summary>
        protected IFormService FormService { get; } = formService;

        /// <summary>
        /// Gets the         /// </summary>
        protected IFolderService FolderService { get; } = folderService;

        /// <summary>
        /// Gets the         /// </summary>
        protected IUserGroupSecurityStorage UserGroupSecurityStorage { get; } = userGroupSecurityStorage;

        /// <summary>
        /// Gets the         /// </summary>
        protected IUserGroupFormSecurityStorage UserGroupFormSecurityStorage { get; } = userGroupFormSecurityStorage;

        /// <summary>
        /// Gets the         /// </summary>
        protected SecuritySettings SecuritySettings { get; } = securitySettings.Value;
    }
}