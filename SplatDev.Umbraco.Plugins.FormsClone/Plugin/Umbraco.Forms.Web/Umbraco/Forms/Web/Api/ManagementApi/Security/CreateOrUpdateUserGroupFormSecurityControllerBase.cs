
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.CreateOrUpdateUserGroupFormSecurityControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security
{
  [Authorize(Policy = "SectionAccessUsers")]
  public abstract class CreateOrUpdateUserGroupFormSecurityControllerBase : 
    FormUserGroupSecurityControllerBase
  {
    protected CreateOrUpdateUserGroupFormSecurityControllerBase(
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

    protected FormSecurityForGroup CreateOrUpdateFormSecurity(
      FormSecurityForGroup security)
    {
      FormSecurityForGroup modelToReturn = new FormSecurityForGroup();
      this.SaveAndApplyUserGroupSecurity(security, modelToReturn);
      this.SaveAndApplyPerFormSecurity(security, modelToReturn);
      this.SaveAndApplyStartFolders(security, modelToReturn);
      return modelToReturn;
    }

    private void SaveAndApplyUserGroupSecurity(
      FormSecurityForGroup security,
      FormSecurityForGroup modelToReturn)
    {
      if (this.UserGroupSecurityStorage.GetUserGroupSecurity(security.UserGroupSecurity.UserGroupId) != null)
        modelToReturn.UserGroupSecurity = this.UserGroupSecurityStorage.UpdateUserGroupSecurity(security.UserGroupSecurity);
      else
        modelToReturn.UserGroupSecurity = this.UserGroupSecurityStorage.InsertUserGroupSecurity(security.UserGroupSecurity);
    }

    private void SaveAndApplyPerFormSecurity(
      FormSecurityForGroup security,
      FormSecurityForGroup modelToReturn)
    {
      List<UserGroupFormSecurity> groupFormSecurityList = new List<UserGroupFormSecurity>();
      foreach (UserGroupFormSecurity formSecurity in security.FormsSecurity)
      {
        if (this.UserGroupFormSecurityStorage.GetUserGroupFormSecurity(security.UserGroupSecurity.UserGroupId, formSecurity.Form) != null)
          groupFormSecurityList.Add(this.UserGroupFormSecurityStorage.UpdateUserGroupFormSecurity(formSecurity));
        else
          groupFormSecurityList.Add(this.UserGroupFormSecurityStorage.InsertUserGroupFormSecurity(formSecurity));
      }
      modelToReturn.FormsSecurity = groupFormSecurityList;
    }

    private void SaveAndApplyStartFolders(
      FormSecurityForGroup security,
      FormSecurityForGroup modelToReturn)
    {
      this.UserGroupStartFolderStorage.UpdateStartFolders(security.UserGroupSecurity.UserGroupId, (IEnumerable<Guid>) security.StartFolderIds);
      modelToReturn.StartFolderIds = security.StartFolderIds;
    }
  }
}
