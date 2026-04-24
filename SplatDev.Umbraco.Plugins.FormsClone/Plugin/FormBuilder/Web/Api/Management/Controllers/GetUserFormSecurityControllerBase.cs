using FormBuilder.Core.Configuration;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Security;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.Extensions.Options;

using System.Globalization;

using Umbraco.Cms.Core.Models.Membership;

using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with form security.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public abstract class GetUserFormSecurityControllerBase(
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
        /// Gets an instance of         /// </summary>
        /// <param name="userKey">The user key.</param>
        /// <param name="explicitOnly">
        /// A flag indicating whether to retrieve only the security information defined explicitly on the user record (used
        /// when editing this information), or whether to use information defined on the user groups to define the net
        /// permissions for the user (used when checking for permissions).
        /// </param>
        /// <param name="includeFormFieldDetails">
        /// A flag indicating whether to respond with details of the form field.
        /// If not required we can optimize the retrieval by pulling back "slim" form instances.
        /// </param>
        protected async Task<FormSecurityForUser?> GetFormSecurityByUserKey(
          Guid userKey,
          bool explicitOnly,
          bool includeFormFieldDetails)
        {
            GetUserFormSecurityControllerBase securityControllerBase = this;
            IUser? async = await securityControllerBase.UserService.GetAsync(userKey);
            if (async is null)
                return null;
            FormSecurityForUser formSecurity = new()
            {
                Key = userKey,
                Name = async.Name ?? "(user)"
            };
            bool isUserAdmin = async.IsAdmin();
            securityControllerBase.ApplyUserSecurity(formSecurity, async, isUserAdmin, explicitOnly);
            securityControllerBase.ApplyPerFormSecurity(formSecurity, async, isUserAdmin, explicitOnly, includeFormFieldDetails);
            securityControllerBase.ApplyStartFolders(formSecurity, async.Id);
            return formSecurity;
        }

        private void ApplyUserSecurity(
          FormSecurityForUser formSecurity,
          IUser user,
          bool isUserAdmin,
          bool explicitOnly)
        {
            UserSecurity? userSecurity = UserSecurityStorage.GetUserSecurity(user.Id);
            if (userSecurity is not null)
            {
                formSecurity.UserSecurity = userSecurity;
            }
            else
            {
                UserSecurity usersecurity = UserSecurity.Create();
                usersecurity.User = user.Id.ToString();
                bool securityWithUserGroups = SecuritySettings.ManageSecurityWithUserGroups;
                if (!explicitOnly & securityWithUserGroups)
                {
                    List<UserGroupSecurity> userGroupSecurity = UserGroupSecurityStorage.GetAllUserGroupSecurity([.. user.Groups.Select(x => x.Id)]);
                    usersecurity.ManageForms = userGroupSecurity.Any(x => x.ManageForms);
                    usersecurity.ManageDataSources = userGroupSecurity.Any(x => x.ManageDataSources);
                    usersecurity.ManagePreValueSources = userGroupSecurity.Any(x => x.ManagePreValueSources);
                    usersecurity.ManageWorkflows = userGroupSecurity.Any(x => x.ManageWorkflows);
                    usersecurity.ViewEntries = userGroupSecurity.Any(x => x.ViewEntries);
                    usersecurity.EditEntries = userGroupSecurity.Any(x => x.EditEntries);
                    usersecurity.DeleteEntries = userGroupSecurity.Any(x => x.DeleteEntries);
                }
                else
                {
                    usersecurity.ManageForms = true;
                    usersecurity.ViewEntries = true;
                    if (isUserAdmin)
                    {
                        usersecurity.ManageDataSources = true;
                        usersecurity.ManagePreValueSources = true;
                        usersecurity.ManageWorkflows = true;
                        usersecurity.EditEntries = true;
                        usersecurity.DeleteEntries = true;
                    }
                    else
                    {
                        usersecurity.ManageDataSources = false;
                        usersecurity.ManagePreValueSources = false;
                        usersecurity.ManageWorkflows = false;
                        usersecurity.EditEntries = false;
                        usersecurity.DeleteEntries = false;
                    }
                    if (!securityWithUserGroups)
                        UserSecurityStorage.InsertUserSecurity(usersecurity);
                }
                formSecurity.UserSecurity = usersecurity;
            }
        }

        private void ApplyPerFormSecurity(
          FormSecurityForUser formSecurity,
          IUser user,
          bool isUserAdmin,
          bool explicitOnly,
          bool includeFormFieldDetails)
        {
            List<UserFormSecurity> userFormSecurity1 = UserFormSecurityStorage.GetAllUserFormSecurity(user.Id.ToString(CultureInfo.InvariantCulture));
            bool securityWithUserGroups = SecuritySettings.ManageSecurityWithUserGroups;
            IEnumerable<IType> source = includeFormFieldDetails ? FormService.Get() : FormService.GetSlim();
            foreach (IType type in source)
            {
                IType form = type;
                string str = string.Empty;
                if (includeFormFieldDetails)
                    str = ((Form)form).GetFormFieldSummary();
                UserFormSecurity? userFormSecurity2 = userFormSecurity1.SingleOrDefault(x => x.Form == form.Id);
                if (userFormSecurity2 is not null)
                {
                    userFormSecurity2.FormName = form.Name;
                    userFormSecurity2.FormCreated = form.Created;
                    userFormSecurity2.Fields = str;
                }
                if (userFormSecurity2 is null)
                {
                    bool flag = isUserAdmin || (!(!explicitOnly & securityWithUserGroups) ? SecuritySettings.DefaultUserAccessToNewForms == FormAccess.Grant : UserGroupFormSecurityStorage.GetAllUserGroupFormSecurity(form.Id, [.. user.Groups.Select(x => x.Id)]).Any(x => x.HasAccess));
                    UserFormSecurity userFormSecurity3 = new()
                    {
                        Form = form.Id,
                        HasAccess = flag,
                        User = user.Id.ToString(CultureInfo.InvariantCulture),
                        FormName = form.Name,
                        FormCreated = form.Created,
                        Fields = str,
                        SecurityType = FormSecurityType.Full,
                        AllowInEditor = true
                    };
                    UserFormSecurity formSecurity1 = userFormSecurity3;
                    if (!securityWithUserGroups)
                        UserFormSecurityStorage.InsertUserFormSecurity(formSecurity1);
                    userFormSecurity1.Add(formSecurity1);
                }
            }
            List<UserFormSecurity> userFormSecurityList = [];
            foreach (UserFormSecurity userFormSecurity4 in userFormSecurity1)
            {
                UserFormSecurity formSecurityDbEntry = userFormSecurity4;
                if (source.SingleOrDefault(x => x.Id == formSecurityDbEntry.Form) is null)
                    userFormSecurityList.Add(formSecurityDbEntry);
            }
            foreach (UserFormSecurity formSecurity2 in userFormSecurityList)
            {
                userFormSecurity1.Remove(formSecurity2);
                UserFormSecurityStorage.DeleteUserFormSecurity(formSecurity2);
            }
            formSecurity.FormsSecurity = [.. userFormSecurity1.OrderBy(x => x.FormName)];
        }

        private void ApplyStartFolders(FormSecurityForUser formSecurity, int userId) => formSecurity.StartFolderIds = [.. UserStartFolderStorage.GetStartFolderKeys(userId)];
    }
}