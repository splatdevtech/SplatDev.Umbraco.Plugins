
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.GetForCurrentUserFormSecurityController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security
{
  [Authorize(Policy = "BackOfficeAccess")]
  public class GetForCurrentUserFormSecurityController : GetUserFormSecurityControllerBase
  {
    private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

    public GetForCurrentUserFormSecurityController(
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

    [HttpGet("current/form-security")]
    [ProducesResponseType(typeof (FormSecurityForUser), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCurrent(bool includeFormFieldDetails)
    {
      GetForCurrentUserFormSecurityController securityController = this;
      IUser currentUser = securityController._backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
      if (currentUser == null)
        return (IActionResult) securityController.NotFound();
      FormSecurityForUser securityByUserKey = await securityController.GetFormSecurityByUserKey(currentUser.Key, false, includeFormFieldDetails);
      return securityByUserKey != null ? (IActionResult) securityController.Ok((object) securityByUserKey) : (IActionResult) securityController.NotFound();
    }
  }
}
