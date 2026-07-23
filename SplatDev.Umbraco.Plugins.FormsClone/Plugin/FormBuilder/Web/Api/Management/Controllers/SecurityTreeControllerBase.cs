using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.OperationStatus;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with the security tree.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Security")]
    [Route("/formBuilder/management/api/v1/tree/security")]
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "SectionAccessUsers")]
    public abstract class SecurityTreeControllerBase(
      IUserService userService,
      IOptions<SecuritySettings> formsSecuritySettings,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor) : FormsManagementApiControllerBase
    {
        /// <summary>
        /// Defines the ID used to represent the "group security " folder in the security tree, when permissions are managed by group.
        /// </summary>
        protected static readonly Guid GroupsFolderId = new("6d67e4b3-6c68-4629-86df-2a3014d503b4");

        /// <summary>
        /// Defines the ID used to represent the "user security" folder in the security tree, when permissions are managed by group.
        /// </summary>
        protected static readonly Guid UsersFolderId = new("207c2294-970b-4e1f-82fd-ae8996ef171d");

        /// <summary>
        /// Gets the         /// </summary>
        protected IUserService UserService { get; } = userService;

        /// <summary>
        /// Gets the         /// </summary>
        protected SecuritySettings FormsSecuritySettings { get; } = formsSecuritySettings.Value;

        /// <summary>
        /// Gets the         /// </summary>
        protected IBackOfficeSecurityAccessor BackOfficeSecurityAccessor { get; } = backOfficeSecurityAccessor;

        /// <summary>Populates the tree response for all users.</summary>
        protected async Task PopulateTreeForUsers(
          List<SecurityTreeItemResponseModel> items,
          int[]? ids = null)
        {
            IEnumerable<IUser> source = await GetAllUsers();
            if (ids is not null)
                source = [.. source.Where(x => ids.Contains(x.Id))];
            foreach (IUser user in source)
                items.Add(CreateItemResponse(user));
        }

        private async Task<IEnumerable<IUser>> GetAllUsers()
        {
            IUser? currentUser = (BackOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser) ?? throw new InvalidOperationException("Could not retrieve current user.");
            Umbraco.Cms.Core.Attempt<PagedModel<IUser>?, UserOperationStatus> allAsync = await UserService.GetAllAsync(currentUser.Key, 0, int.MaxValue);
            if (!allAsync.Success || allAsync.Result is null)
                throw new InvalidOperationException("Could not retrieve users.");
            return allAsync.Result.Items;
        }

        private static SecurityTreeItemResponseModel CreateItemResponse(IUser user)
        {
            SecurityTreeItemResponseModel itemResponse = new()
            {
                Id = user.Key,
                HasChildren = false,
                Name = user.Name ?? "(user)",
                IsFolder = false
            };
            return itemResponse;
        }
    }
}