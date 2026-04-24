
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.FormSecurityControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security
{
  [ApiExplorerSettings(GroupName = "Security")]
  public abstract class FormSecurityControllerBase : FormsManagementApiControllerBase
  {
    protected FormSecurityControllerBase(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings)
    {
      this.FormService = formService;
      this.FolderService = folderService;
      this.UserGroupSecurityStorage = userGroupSecurityStorage;
      this.UserGroupFormSecurityStorage = userGroupFormSecurityStorage;
      this.SecuritySettings = securitySettings.Value;
    }

    protected IFormService FormService { get; }

    protected IFolderService FolderService { get; }

    protected IUserGroupSecurityStorage UserGroupSecurityStorage { get; }

    protected IUserGroupFormSecurityStorage UserGroupFormSecurityStorage { get; }

    protected SecuritySettings SecuritySettings { get; }
  }
}
