
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.FormUserSecurityControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security
{
  [Route("/umbraco/forms/management/api/v1/security/user")]
  public abstract class FormUserSecurityControllerBase : FormSecurityControllerBase
  {
    protected FormUserSecurityControllerBase(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserService userService,
      IUserSecurityStorage userSecurityStorage,
      IUserFormSecurityStorage userFormSecurityStorage,
      IUserStartFolderStorage userStartFolderStorage)
      : base(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings)
    {
      this.UserService = userService;
      this.UserSecurityStorage = userSecurityStorage;
      this.UserFormSecurityStorage = userFormSecurityStorage;
      this.UserStartFolderStorage = userStartFolderStorage;
    }

    protected IUserService UserService { get; }

    protected IUserSecurityStorage UserSecurityStorage { get; }

    protected IUserFormSecurityStorage UserFormSecurityStorage { get; }

    protected IUserStartFolderStorage UserStartFolderStorage { get; }
  }
}
