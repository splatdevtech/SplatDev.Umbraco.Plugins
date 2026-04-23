
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.UpdateUserFormSecurityController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security
{
  public class UpdateUserFormSecurityController : CreateOrUpdateUserFormSecurityControllerBase
  {
    public UpdateUserFormSecurityController(
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

    [HttpPut("{id:guid}/form-security")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof (ProblemDetails), 400)]
    public async Task<IActionResult> Update(Guid id, FormSecurityForUser security)
    {
      UpdateUserFormSecurityController securityController = this;
      IUser async = await securityController.UserService.GetAsync(id);
      if (async == null)
        return (IActionResult) securityController.NotFound();
      securityController.CreateOrUpdateFormSecurity(async.Id, security);
      return (IActionResult) securityController.Ok();
    }
  }
}
