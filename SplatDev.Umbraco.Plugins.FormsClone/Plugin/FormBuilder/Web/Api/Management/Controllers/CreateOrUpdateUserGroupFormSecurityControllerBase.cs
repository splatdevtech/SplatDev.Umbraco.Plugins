using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Security;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when creating or updating form security for a user group.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "SectionAccessUsers")]
    public abstract class CreateOrUpdateUserGroupFormSecurityControllerBase(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserGroupService userGroupService,
      IUserGroupStartFolderStorage userGroupStartFolderStorage) :
      FormUserGroupSecurityControllerBase(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings, userGroupService, userGroupStartFolderStorage)
    {
        /// <summary>
        /// Saves the provided         /// </summary>
        protected FormSecurityForGroup CreateOrUpdateFormSecurity(
          FormSecurityForGroup security)
        {
            FormSecurityForGroup modelToReturn = new();
            SaveAndApplyUserGroupSecurity(security, modelToReturn);
            SaveAndApplyPerFormSecurity(security, modelToReturn);
            SaveAndApplyStartFolders(security, modelToReturn);
            return modelToReturn;
        }

        private void SaveAndApplyUserGroupSecurity(
          FormSecurityForGroup security,
          FormSecurityForGroup modelToReturn)
        {
            if (UserGroupSecurityStorage.GetUserGroupSecurity(security.UserGroupSecurity.UserGroupId) is not null)
                modelToReturn.UserGroupSecurity = UserGroupSecurityStorage.UpdateUserGroupSecurity(security.UserGroupSecurity);
            else
                modelToReturn.UserGroupSecurity = UserGroupSecurityStorage.InsertUserGroupSecurity(security.UserGroupSecurity);
        }

        private void SaveAndApplyPerFormSecurity(
          FormSecurityForGroup security,
          FormSecurityForGroup modelToReturn)
        {
            List<UserGroupFormSecurity> groupFormSecurityList = [];
            foreach (UserGroupFormSecurity formSecurity in security.FormsSecurity)
            {
                if (UserGroupFormSecurityStorage.GetUserGroupFormSecurity(security.UserGroupSecurity.UserGroupId, formSecurity.Form) is not null)
                    groupFormSecurityList.Add(UserGroupFormSecurityStorage.UpdateUserGroupFormSecurity(formSecurity));
                else
                    groupFormSecurityList.Add(UserGroupFormSecurityStorage.InsertUserGroupFormSecurity(formSecurity));
            }
            modelToReturn.FormsSecurity = groupFormSecurityList;
        }

        private void SaveAndApplyStartFolders(
          FormSecurityForGroup security,
          FormSecurityForGroup modelToReturn)
        {
            UserGroupStartFolderStorage.UpdateStartFolders(security.UserGroupSecurity.UserGroupId, security.StartFolderIds);
            modelToReturn.StartFolderIds = security.StartFolderIds;
        }
    }
}