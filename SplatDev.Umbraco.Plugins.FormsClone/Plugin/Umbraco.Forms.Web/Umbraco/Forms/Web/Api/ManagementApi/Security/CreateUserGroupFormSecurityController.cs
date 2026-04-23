
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.CreateUserGroupFormSecurityController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security
{
  public class CreateUserGroupFormSecurityController : 
    CreateOrUpdateUserGroupFormSecurityControllerBase
  {
    public CreateUserGroupFormSecurityController(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserGroupService userGroupService,
      IUserGroupStartFolderStorage userGroupStartFolderStorage)
      : base(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings, userGroupService, userGroupStartFolderStorage)
    {
    }

    [HttpPost("{id:guid}/form-security")]
    [ProducesResponseType(201)]
    [ProducesResponseType(typeof (ProblemDetails), 400)]
    public IActionResult Create(Guid id, FormSecurityForGroup security)
    {
      this.CreateOrUpdateFormSecurity(security);
      return (IActionResult) this.Created();
    }
  }
}
