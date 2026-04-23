using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Api.Management.ViewModels.Tree;

using Umbraco.Cms.Core.Security;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for rendering the root items of the security tree.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class RootSecurityTreeController(
      IUserService userService,
      IOptions<SecuritySettings> formsSecuritySettings,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor) : SecurityTreeControllerBase(userService, formsSecuritySettings, backOfficeSecurityAccessor)
    {
        /// <summary>
        /// Management API endpoint for rendering the root items of the prevalue source's tree.
        /// </summary>
        [HttpGet("root")]
        [ProducesResponseType(typeof(PagedViewModel<SecurityTreeItemResponseModel>), 200)]
        public async Task<ActionResult<IEnumerable<FolderTreeItemResponseModel>>> Root()
        {
            RootSecurityTreeController securityTreeController = this;
            List<SecurityTreeItemResponseModel>? items = [];
            if (securityTreeController.FormsSecuritySettings.ManageSecurityWithUserGroups)
            {
                List<SecurityTreeItemResponseModel> itemResponseModelList1 = items;
                SecurityTreeItemResponseModel itemResponseModel1 = new()
                {
                    HasChildren = true,
                    IsFolder = true,
                    Name = "Group Security",
                    Id = GroupsFolderId
                };
                itemResponseModelList1.Add(itemResponseModel1);
                List<SecurityTreeItemResponseModel> itemResponseModelList2 = items;
                SecurityTreeItemResponseModel itemResponseModel2 = new()
                {
                    HasChildren = true,
                    IsFolder = true,
                    Name = "User Security",
                    Id = UsersFolderId
                };
                itemResponseModelList2.Add(itemResponseModel2);
            }
            else
                await securityTreeController.PopulateTreeForUsers(items);
            ActionResult<IEnumerable<FolderTreeItemResponseModel>> actionResult = (ActionResult<IEnumerable<FolderTreeItemResponseModel>>)securityTreeController.Ok(new PagedViewModel<SecurityTreeItemResponseModel>()
            {
                Items = items,
                Total = items.Count
            });
            return actionResult;
        }
    }
}