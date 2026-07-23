
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.DeleteUserFormSecurityController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security
{
  [Authorize(Policy = "SectionAccessForms")]
  [Authorize(Policy = "SectionAccessUsers")]
  public class DeleteUserFormSecurityController : FormUserSecurityControllerBase
  {
    public DeleteUserFormSecurityController(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserService userService,
      IUserSecurityStorage userSecurityStorage,
      IUserFormSecurityStorage userFormSecurityStorage,
      IUserStartFolderStorage userStartFolderStorage)
      : base(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings, userService, userSecurityStorage, userFormSecurityStorage, userStartFolderStorage)
    {
    }

    [HttpDelete("{id:guid}/form-security")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
      DeleteUserFormSecurityController securityController = this;
      IUser async = await securityController.UserService.GetAsync(id);
      if (async == null)
        return (IActionResult) securityController.NotFound();
      UserSecurity userSecurity = securityController.UserSecurityStorage.GetUserSecurity(async.Id);
      if (userSecurity == null)
        return (IActionResult) securityController.NotFound();
      securityController.UserSecurityStorage.DeleteUserSecurity(userSecurity);
      securityController.UserStartFolderStorage.UpdateStartFolders(async.Id, Enumerable.Empty<Guid>());
      securityController.UserFormSecurityStorage.DeleteAllUserFormSecurityForUser(async.Id);
      return (IActionResult) securityController.Ok();
    }
  }
}
