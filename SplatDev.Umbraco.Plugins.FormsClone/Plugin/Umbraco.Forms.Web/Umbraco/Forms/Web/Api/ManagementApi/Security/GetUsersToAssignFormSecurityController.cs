
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.GetUsersToAssignFormSecurityController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Api.Management.ViewModels.User.Item;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.OperationStatus;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security
{
  [Authorize(Policy = "SectionAccessForms")]
  [Authorize(Policy = "SectionAccessUsers")]
  public class GetUsersToAssignFormSecurityController : FormUserSecurityControllerBase
  {
    private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

    public GetUsersToAssignFormSecurityController(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserService userService,
      IUserSecurityStorage userSecurityStorage,
      IUserFormSecurityStorage userFormSecurityStorage,
      IUserStartFolderStorage userStartFolderStorage,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
      : base(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings, userService, userSecurityStorage, userFormSecurityStorage, userStartFolderStorage)
    {
      this._backOfficeSecurityAccessor = backOfficeSecurityAccessor;
    }

    [HttpGet("users-to-assign")]
    [ProducesResponseType(typeof (IEnumerable<UserItemResponseModel>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUsersToAssign()
    {
      GetUsersToAssignFormSecurityController securityController = this;
      IUser user = securityController._backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser ?? throw new InvalidOperationException("Could not retrieve current user.");
      Umbraco.Cms.Core.Attempt<PagedModel<IUser>, UserOperationStatus> allAsync = await securityController.UserService.GetAllAsync(user.Key, 0, int.MaxValue);
      if (!allAsync.Success || allAsync.Result == null)
        throw new InvalidOperationException("Could not retrieve users.");
      List<int> existingRecordIds = securityController.UserSecurityStorage.GetAllUserSecurity().Select<UserSecurity, int>((Func<UserSecurity, int>) (x => int.Parse(x.User, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture))).ToList<int>();
      List<UserItemResponseModel> list = allAsync.Result.Items.Where<IUser>((Func<IUser, bool>) (x => !existingRecordIds.Contains(x.Id))).Select<IUser, UserItemResponseModel>((Func<IUser, UserItemResponseModel>) (x =>
      {
        return new UserItemResponseModel()
        {
          Id = x.Key,
          Name = x.Name ?? "(user)"
        };
      })).ToList<UserItemResponseModel>();
      return (IActionResult) securityController.Ok((object) list);
    }
  }
}
