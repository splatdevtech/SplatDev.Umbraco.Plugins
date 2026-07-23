
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.Tree.ChildrenSecurityTreeController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Data.Storage;
using Umbraco.Forms.Web.Models.ManagementApi.Form.Tree;
using Umbraco.Forms.Web.Models.ManagementApi.Security;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security.Tree
{
  public class ChildrenSecurityTreeController : SecurityTreeControllerBase
  {
    private readonly IUserGroupService _userGroupService;
    private readonly IUserSecurityStorage _userSecurityStorage;

    public ChildrenSecurityTreeController(
      IUserService userService,
      IOptions<SecuritySettings> formsSecuritySettings,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IUserGroupService userGroupService,
      IUserSecurityStorage userSecurityStorage)
      : base(userService, formsSecuritySettings, backOfficeSecurityAccessor)
    {
      this._userGroupService = userGroupService;
      this._userSecurityStorage = userSecurityStorage;
    }

    [HttpGet("children/{parentId:guid}")]
    [ProducesResponseType(typeof (PagedViewModel<SecurityTreeItemResponseModel>), 200)]
    public async Task<ActionResult<IEnumerable<FormTreeItemResponseModel>>> Children(
      Guid parentId)
    {
      ChildrenSecurityTreeController securityTreeController = this;
      List<SecurityTreeItemResponseModel> items = new List<SecurityTreeItemResponseModel>();
      if (securityTreeController.FormsSecuritySettings.ManageSecurityWithUserGroups)
      {
        if (parentId == SecurityTreeControllerBase.UsersFolderId)
        {
          int[] array = securityTreeController._userSecurityStorage.GetAllUserSecurity().Select<UserSecurity, int>((Func<UserSecurity, int>) (x => int.Parse(x.User, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture))).ToArray<int>();
          await securityTreeController.PopulateTreeForUsers(items, array);
        }
        else if (parentId == SecurityTreeControllerBase.GroupsFolderId)
          await securityTreeController.PopulateTreeForUserGroups(items);
      }
      ActionResult<IEnumerable<FormTreeItemResponseModel>> actionResult = (ActionResult<IEnumerable<FormTreeItemResponseModel>>) (ActionResult) securityTreeController.Ok((object) new PagedViewModel<SecurityTreeItemResponseModel>()
      {
        Items = (IEnumerable<SecurityTreeItemResponseModel>) items,
        Total = (long) items.Count<SecurityTreeItemResponseModel>()
      });
      items = (List<SecurityTreeItemResponseModel>) null;
      return actionResult;
    }

    private async Task PopulateTreeForUserGroups(List<SecurityTreeItemResponseModel> items)
    {
      foreach (IUserGroup allUserGroup in await this.GetAllUserGroups())
        items.Add(this.CreateItemResponse(allUserGroup));
    }

    private async Task<IEnumerable<IUserGroup>> GetAllUserGroups() => (await this._userGroupService.GetAllAsync(0, int.MaxValue)).Items;

    private SecurityTreeItemResponseModel CreateItemResponse(
      IUserGroup group)
    {
      SecurityTreeItemResponseModel itemResponse = new SecurityTreeItemResponseModel();
      itemResponse.Id = group.Key;
      itemResponse.HasChildren = false;
      itemResponse.Name = group.Name ?? "(user group)";
      itemResponse.IsFolder = false;
      itemResponse.IsGroup = true;
      return itemResponse;
    }
  }
}
