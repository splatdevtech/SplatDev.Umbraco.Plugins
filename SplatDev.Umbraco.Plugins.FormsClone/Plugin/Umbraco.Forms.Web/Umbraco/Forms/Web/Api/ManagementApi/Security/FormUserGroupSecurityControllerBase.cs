
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.FormUserGroupSecurityControllerBase
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
  [Route("/umbraco/forms/management/api/v1/security/user-group")]
  public abstract class FormUserGroupSecurityControllerBase : FormSecurityControllerBase
  {
    protected FormUserGroupSecurityControllerBase(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserGroupService userGroupService,
      IUserGroupStartFolderStorage userGroupStartFolderStorage)
      : base(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings)
    {
      this.UserGroupService = userGroupService;
      this.UserGroupStartFolderStorage = userGroupStartFolderStorage;
    }

    protected IUserGroupService UserGroupService { get; }

    public IUserGroupStartFolderStorage UserGroupStartFolderStorage { get; }
  }
}
