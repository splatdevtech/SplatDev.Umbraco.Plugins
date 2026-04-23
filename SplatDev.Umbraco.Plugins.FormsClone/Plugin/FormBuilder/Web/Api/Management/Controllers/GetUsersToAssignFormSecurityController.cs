using FormBuilder.Core.Configuration;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System.Globalization;

using Umbraco.Cms.Api.Management.ViewModels.User.Item;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving the users available to assign specific user security.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "SectionAccessUsers")]
    public class GetUsersToAssignFormSecurityController(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserService userService,
      IUserSecurityStorage userSecurityStorage,
      IUserFormSecurityStorage userFormSecurityStorage,
      IUserStartFolderStorage userStartFolderStorage,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor) : FormUserSecurityControllerBase(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings, userService, userSecurityStorage, userFormSecurityStorage, userStartFolderStorage)
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor = backOfficeSecurityAccessor;

        /// <summary>
        /// Management API endpoint for retrieving the users available to assign specific user security.
        /// </summary>
        [HttpGet("users-to-assign")]
        [ProducesResponseType(typeof(IEnumerable<UserItemResponseModel>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUsersToAssign()
        {
            GetUsersToAssignFormSecurityController securityController = this;
            var user = securityController._backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser ?? throw new InvalidOperationException("Could not retrieve current user.");
            var allAsync = await securityController.UserService.GetAllAsync(user.Key, 0, int.MaxValue);
            if (!allAsync.Success || allAsync.Result is null)
                throw new InvalidOperationException("Could not retrieve users.");
            List<int> existingRecordIds = [.. securityController.UserSecurityStorage.GetAllUserSecurity().Select(x => int.Parse(x.User, NumberStyles.Integer, CultureInfo.InvariantCulture))];
            List<UserItemResponseModel> list = [.. allAsync.Result.Items.Where(x => !existingRecordIds.Contains(x.Id)).Select(x =>
            {
                return new UserItemResponseModel()
                {
                    Id = x.Key,
                    Name = x.Name ?? "(user)"
                };
            })];
            return securityController.Ok(list);
        }
    }
}