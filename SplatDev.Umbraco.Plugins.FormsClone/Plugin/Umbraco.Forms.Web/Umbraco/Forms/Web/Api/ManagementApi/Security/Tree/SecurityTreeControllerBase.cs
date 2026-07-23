
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.Tree.SecurityTreeControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.OperationStatus;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Web.Models.ManagementApi.Security;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security.Tree
{
  [ApiExplorerSettings(GroupName = "Security")]
  [Route("/umbraco/forms/management/api/v1/tree/security")]
  [Authorize(Policy = "SectionAccessForms")]
  [Authorize(Policy = "SectionAccessUsers")]
  public abstract class SecurityTreeControllerBase : FormsManagementApiControllerBase
  {
    protected static readonly Guid GroupsFolderId = new Guid("6d67e4b3-6c68-4629-86df-2a3014d503b4");
    protected static readonly Guid UsersFolderId = new Guid("207c2294-970b-4e1f-82fd-ae8996ef171d");

    protected SecurityTreeControllerBase(
      IUserService userService,
      IOptions<SecuritySettings> formsSecuritySettings,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
    {
      this.UserService = userService;
      this.FormsSecuritySettings = formsSecuritySettings.Value;
      this.BackOfficeSecurityAccessor = backOfficeSecurityAccessor;
    }

    protected IUserService UserService { get; }

    protected SecuritySettings FormsSecuritySettings { get; }

    protected IBackOfficeSecurityAccessor BackOfficeSecurityAccessor { get; }

    protected async Task PopulateTreeForUsers(
      List<SecurityTreeItemResponseModel> items,
      int[]? ids = null)
    {
      IEnumerable<IUser> source = await this.GetAllUsers();
      if (ids != null)
        source = (IEnumerable<IUser>) source.Where<IUser>((Func<IUser, bool>) (x => ((IEnumerable<int>) ids).Contains<int>(x.Id))).ToList<IUser>();
      foreach (IUser user in source)
        items.Add(this.CreateItemResponse(user));
    }

    private async Task<IEnumerable<IUser>> GetAllUsers()
    {
      IUser currentUser = this.BackOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
      if (currentUser == null)
        throw new InvalidOperationException("Could not retrieve current user.");
      Umbraco.Cms.Core.Attempt<PagedModel<IUser>, UserOperationStatus> allAsync = await this.UserService.GetAllAsync(currentUser.Key, 0, int.MaxValue);
      if (!allAsync.Success || allAsync.Result == null)
        throw new InvalidOperationException("Could not retrieve users.");
      return allAsync.Result.Items;
    }

    private SecurityTreeItemResponseModel CreateItemResponse(IUser user)
    {
      SecurityTreeItemResponseModel itemResponse = new SecurityTreeItemResponseModel();
      itemResponse.Id = user.Key;
      itemResponse.HasChildren = false;
      itemResponse.Name = user.Name ?? "(user)";
      itemResponse.IsFolder = false;
      return itemResponse;
    }
  }
}
