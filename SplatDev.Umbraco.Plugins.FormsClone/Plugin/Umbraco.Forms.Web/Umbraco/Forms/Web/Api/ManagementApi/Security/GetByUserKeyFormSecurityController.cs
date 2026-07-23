
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.GetByUserKeyFormSecurityController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security
{
  [Authorize(Policy = "SectionAccessForms")]
  [Authorize(Policy = "SectionAccessUsers")]
  public class GetByUserKeyFormSecurityController : GetUserFormSecurityControllerBase
  {
    public GetByUserKeyFormSecurityController(
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

    [HttpGet("{id:guid}/form-security")]
    [ProducesResponseType(typeof (FormSecurityForUser), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByUserKey(Guid id, bool explicitOnly)
    {
      GetByUserKeyFormSecurityController securityController = this;
      FormSecurityForUser securityByUserKey = await securityController.GetFormSecurityByUserKey(id, explicitOnly, true);
      return securityByUserKey != null ? (IActionResult) securityController.Ok((object) securityByUserKey) : (IActionResult) securityController.NotFound();
    }
  }
}
