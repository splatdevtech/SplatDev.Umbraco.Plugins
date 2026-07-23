
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.Tree.RootSecurityTreeController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Api.Management.ViewModels.Tree;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Web.Models.ManagementApi.Security;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security.Tree
{
  public class RootSecurityTreeController : SecurityTreeControllerBase
  {
    public RootSecurityTreeController(
      IUserService userService,
      IOptions<SecuritySettings> formsSecuritySettings,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
      : base(userService, formsSecuritySettings, backOfficeSecurityAccessor)
    {
    }

    [HttpGet("root")]
    [ProducesResponseType(typeof (PagedViewModel<SecurityTreeItemResponseModel>), 200)]
    public async Task<ActionResult<IEnumerable<FolderTreeItemResponseModel>>> Root()
    {
      RootSecurityTreeController securityTreeController = this;
      List<SecurityTreeItemResponseModel> items = new List<SecurityTreeItemResponseModel>();
      if (securityTreeController.FormsSecuritySettings.ManageSecurityWithUserGroups)
      {
        List<SecurityTreeItemResponseModel> itemResponseModelList1 = items;
        SecurityTreeItemResponseModel itemResponseModel1 = new SecurityTreeItemResponseModel();
        itemResponseModel1.HasChildren = true;
        itemResponseModel1.IsFolder = true;
        itemResponseModel1.Name = "Group Security";
        itemResponseModel1.Id = SecurityTreeControllerBase.GroupsFolderId;
        itemResponseModelList1.Add(itemResponseModel1);
        List<SecurityTreeItemResponseModel> itemResponseModelList2 = items;
        SecurityTreeItemResponseModel itemResponseModel2 = new SecurityTreeItemResponseModel();
        itemResponseModel2.HasChildren = true;
        itemResponseModel2.IsFolder = true;
        itemResponseModel2.Name = "User Security";
        itemResponseModel2.Id = SecurityTreeControllerBase.UsersFolderId;
        itemResponseModelList2.Add(itemResponseModel2);
      }
      else
        await securityTreeController.PopulateTreeForUsers(items);
      ActionResult<IEnumerable<FolderTreeItemResponseModel>> actionResult = (ActionResult<IEnumerable<FolderTreeItemResponseModel>>) (ActionResult) securityTreeController.Ok((object) new PagedViewModel<SecurityTreeItemResponseModel>()
      {
        Items = (IEnumerable<SecurityTreeItemResponseModel>) items,
        Total = (long) items.Count<SecurityTreeItemResponseModel>()
      });
      items = (List<SecurityTreeItemResponseModel>) null;
      return actionResult;
    }
  }
}
