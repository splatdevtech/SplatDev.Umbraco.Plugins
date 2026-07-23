using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System.Globalization;

using Umbraco.Cms.Api.Common.ViewModels.Pagination;

using Umbraco.Cms.Core.Models.Membership;

using Umbraco.Cms.Core.Security;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for rendering the child items of the security tree.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class ChildrenSecurityTreeController(
      IUserService userService,
      IOptions<SecuritySettings> formsSecuritySettings,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IUserGroupService userGroupService,
      IUserSecurityStorage userSecurityStorage) : SecurityTreeControllerBase(userService, formsSecuritySettings, backOfficeSecurityAccessor)
    {
        private readonly IUserGroupService _userGroupService = userGroupService;
        private readonly IUserSecurityStorage _userSecurityStorage = userSecurityStorage;

        /// <summary>
        /// Management API endpoint for rendering the child items of the prevalue source's tree.
        /// </summary>
        [HttpGet("children/{parentId:guid}")]
        [ProducesResponseType(typeof(PagedViewModel<SecurityTreeItemResponseModel>), 200)]
        public async Task<ActionResult<IEnumerable<FormTreeItemResponseModel>>> Children(
          Guid parentId)
        {
            ChildrenSecurityTreeController securityTreeController = this;
            List<SecurityTreeItemResponseModel>? items = [];
            if (securityTreeController.FormsSecuritySettings.ManageSecurityWithUserGroups)
            {
                if (parentId == UsersFolderId)
                {
                    int[] array = [.. securityTreeController._userSecurityStorage.GetAllUserSecurity().Select(x => int.Parse(x.User, NumberStyles.Integer, CultureInfo.InvariantCulture))];
                    await securityTreeController.PopulateTreeForUsers(items, array);
                }
                else if (parentId == GroupsFolderId)
                    await securityTreeController.PopulateTreeForUserGroups(items);
            }
            ActionResult<IEnumerable<FormTreeItemResponseModel>> actionResult = (ActionResult<IEnumerable<FormTreeItemResponseModel>>)securityTreeController.Ok(new PagedViewModel<SecurityTreeItemResponseModel>()
            {
                Items = items,
                Total = items.Count
            });
            items = null;
            return actionResult;
        }

        private async Task PopulateTreeForUserGroups(List<SecurityTreeItemResponseModel> items)
        {
            foreach (IUserGroup allUserGroup in await GetAllUserGroups())
                items.Add(CreateItemResponse(allUserGroup));
        }

        private async Task<IEnumerable<IUserGroup>> GetAllUserGroups() => (await _userGroupService.GetAllAsync(0, int.MaxValue)).Items;

        private static SecurityTreeItemResponseModel CreateItemResponse(
          IUserGroup group)
        {
            SecurityTreeItemResponseModel itemResponse = new()
            {
                Id = group.Key,
                HasChildren = false,
                Name = group.Name ?? "(user group)",
                IsFolder = false,
                IsGroup = true
            };
            return itemResponse;
        }
    }
}