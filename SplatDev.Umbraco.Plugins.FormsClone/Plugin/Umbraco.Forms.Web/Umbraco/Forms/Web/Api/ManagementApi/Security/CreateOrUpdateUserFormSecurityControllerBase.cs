
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.CreateOrUpdateUserFormSecurityControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security
{
  [Authorize(Policy = "SectionAccessForms")]
  [Authorize(Policy = "SectionAccessUsers")]
  public abstract class CreateOrUpdateUserFormSecurityControllerBase : FormUserSecurityControllerBase
  {
    protected CreateOrUpdateUserFormSecurityControllerBase(
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

    protected FormSecurityForUser CreateOrUpdateFormSecurity(
      int userId,
      FormSecurityForUser security)
    {
      FormSecurityForUser modelToReturn = new FormSecurityForUser();
      this.SaveAndApplyUserSecurity(userId, security, modelToReturn);
      this.SaveAndApplyPerFormSecurity(userId, security, modelToReturn);
      this.SaveAndApplyStartFolders(userId, security, modelToReturn);
      return modelToReturn;
    }

    private void SaveAndApplyUserSecurity(
      int userId,
      FormSecurityForUser security,
      FormSecurityForUser modelToReturn)
    {
      if (this.UserSecurityStorage.GetUserSecurity(userId) != null)
      {
        modelToReturn.UserSecurity = this.UserSecurityStorage.UpdateUserSecurity(security.UserSecurity);
      }
      else
      {
        security.UserSecurity.User = userId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        modelToReturn.UserSecurity = this.UserSecurityStorage.InsertUserSecurity(security.UserSecurity);
      }
    }

    private void SaveAndApplyPerFormSecurity(
      int userId,
      FormSecurityForUser security,
      FormSecurityForUser modelToReturn)
    {
      List<UserFormSecurity> userFormSecurityList = new List<UserFormSecurity>();
      foreach (UserFormSecurity formSecurity in security.FormsSecurity)
      {
        if (this.UserFormSecurityStorage.GetUserFormSecurity(userId.ToString((IFormatProvider) CultureInfo.InvariantCulture), formSecurity.Form) != null)
          userFormSecurityList.Add(this.UserFormSecurityStorage.UpdateUserFormSecurity(formSecurity));
        else
          userFormSecurityList.Add(this.UserFormSecurityStorage.InsertUserFormSecurity(formSecurity));
      }
      modelToReturn.FormsSecurity = userFormSecurityList;
    }

    private void SaveAndApplyStartFolders(
      int userId,
      FormSecurityForUser security,
      FormSecurityForUser modelToReturn)
    {
      this.UserStartFolderStorage.UpdateStartFolders(userId, (IEnumerable<Guid>) security.StartFolderIds);
      modelToReturn.StartFolderIds = security.StartFolderIds;
    }
  }
}
