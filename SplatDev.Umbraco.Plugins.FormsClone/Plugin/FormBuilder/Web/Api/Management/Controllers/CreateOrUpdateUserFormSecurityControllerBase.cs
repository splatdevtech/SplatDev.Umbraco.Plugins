using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Security;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using System.Globalization;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when creating or updating form security for a user group.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "SectionAccessUsers")]
    public abstract class CreateOrUpdateUserFormSecurityControllerBase(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserService userService,
      IUserSecurityStorage userSecurityStorage,
      IUserFormSecurityStorage userFormSecurityStorage,
      IUserStartFolderStorage userStartFolderStorage) : FormUserSecurityControllerBase(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings, userService, userSecurityStorage, userFormSecurityStorage, userStartFolderStorage)
    {
        /// <summary>
        /// Creates or updates the provided         /// </summary>
        protected FormSecurityForUser CreateOrUpdateFormSecurity(
          int userId,
          FormSecurityForUser security)
        {
            FormSecurityForUser modelToReturn = new();
            SaveAndApplyUserSecurity(userId, security, modelToReturn);
            SaveAndApplyPerFormSecurity(userId, security, modelToReturn);
            SaveAndApplyStartFolders(userId, security, modelToReturn);
            return modelToReturn;
        }

        private void SaveAndApplyUserSecurity(
          int userId,
          FormSecurityForUser security,
          FormSecurityForUser modelToReturn)
        {
            if (UserSecurityStorage.GetUserSecurity(userId) is not null)
            {
                modelToReturn.UserSecurity = UserSecurityStorage.UpdateUserSecurity(security.UserSecurity);
            }
            else
            {
                security.UserSecurity.User = userId.ToString(CultureInfo.InvariantCulture);
                modelToReturn.UserSecurity = UserSecurityStorage.InsertUserSecurity(security.UserSecurity);
            }
        }

        private void SaveAndApplyPerFormSecurity(
          int userId,
          FormSecurityForUser security,
          FormSecurityForUser modelToReturn)
        {
            List<UserFormSecurity> userFormSecurityList = [];
            foreach (UserFormSecurity formSecurity in security.FormsSecurity)
            {
                if (UserFormSecurityStorage.GetUserFormSecurity(userId.ToString(CultureInfo.InvariantCulture), formSecurity.Form) is not null)
                    userFormSecurityList.Add(UserFormSecurityStorage.UpdateUserFormSecurity(formSecurity));
                else
                    userFormSecurityList.Add(UserFormSecurityStorage.InsertUserFormSecurity(formSecurity));
            }
            modelToReturn.FormsSecurity = userFormSecurityList;
        }

        private void SaveAndApplyStartFolders(
          int userId,
          FormSecurityForUser security,
          FormSecurityForUser modelToReturn)
        {
            UserStartFolderStorage.UpdateStartFolders(userId, security.StartFolderIds);
            modelToReturn.StartFolderIds = security.StartFolderIds;
        }
    }
}