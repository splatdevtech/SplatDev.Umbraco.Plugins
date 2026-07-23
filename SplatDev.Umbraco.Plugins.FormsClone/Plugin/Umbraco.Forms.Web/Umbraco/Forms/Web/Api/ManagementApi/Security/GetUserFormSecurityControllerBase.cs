
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.GetUserFormSecurityControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.Extensions.Options;

using System.Globalization;

using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security
{
    public abstract class GetUserFormSecurityControllerBase : FormUserSecurityControllerBase
    {
        protected GetUserFormSecurityControllerBase(
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

        protected async Task<FormSecurityForUser?> GetFormSecurityByUserKey(
          Guid userKey,
          bool explicitOnly,
          bool includeFormFieldDetails)
        {
            GetUserFormSecurityControllerBase securityControllerBase = this;
            IUser async = await securityControllerBase.UserService.GetAsync(userKey);
            if (async == null)
                return null;
            FormSecurityForUser formSecurity = new FormSecurityForUser()
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
            UserSecurity userSecurity = this.UserSecurityStorage.GetUserSecurity(user.Id);
            if (userSecurity != null)
            {
                formSecurity.UserSecurity = userSecurity;
            }
            else
            {
                UserSecurity usersecurity = UserSecurity.Create();
                usersecurity.User = user.Id.ToString();
                bool securityWithUserGroups = this.SecuritySettings.ManageSecurityWithUserGroups;
                if (!explicitOnly & securityWithUserGroups)
                {
                    List<UserGroupSecurity> userGroupSecurity = this.UserGroupSecurityStorage.GetAllUserGroupSecurity(user.Groups.Select<IReadOnlyUserGroup, int>(x => x.Id).ToArray<int>());
                    usersecurity.ManageForms = userGroupSecurity.Any<UserGroupSecurity>(x => x.ManageForms);
                    usersecurity.ManageDataSources = userGroupSecurity.Any<UserGroupSecurity>(x => x.ManageDataSources);
                    usersecurity.ManagePreValueSources = userGroupSecurity.Any<UserGroupSecurity>(x => x.ManagePreValueSources);
                    usersecurity.ManageWorkflows = userGroupSecurity.Any<UserGroupSecurity>(x => x.ManageWorkflows);
                    usersecurity.ViewEntries = userGroupSecurity.Any<UserGroupSecurity>(x => x.ViewEntries);
                    usersecurity.EditEntries = userGroupSecurity.Any<UserGroupSecurity>(x => x.EditEntries);
                    usersecurity.DeleteEntries = userGroupSecurity.Any<UserGroupSecurity>(x => x.DeleteEntries);
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
                        this.UserSecurityStorage.InsertUserSecurity(usersecurity);
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
            List<UserFormSecurity> userFormSecurity1 = this.UserFormSecurityStorage.GetAllUserFormSecurity(user.Id.ToString(CultureInfo.InvariantCulture));
            bool securityWithUserGroups = this.SecuritySettings.ManageSecurityWithUserGroups;
            IEnumerable<IType> source = includeFormFieldDetails ? this.FormService.Get() : this.FormService.GetSlim();
            foreach (IType type in source)
            {
                IType form = type;
                string str = string.Empty;
                if (includeFormFieldDetails)
                    str = ((Umbraco.Forms.Core.Models.Form)form).GetFormFieldSummary();
                UserFormSecurity userFormSecurity2 = userFormSecurity1.SingleOrDefault<UserFormSecurity>(x => x.Form == form.Id);
                if (userFormSecurity2 != null)
                {
                    userFormSecurity2.FormName = form.Name;
                    userFormSecurity2.FormCreated = form.Created;
                    userFormSecurity2.Fields = str;
                }
                if (userFormSecurity2 == null)
                {
                    bool flag = isUserAdmin || (!(!explicitOnly & securityWithUserGroups) ? this.SecuritySettings.DefaultUserAccessToNewForms == FormAccess.Grant : this.UserGroupFormSecurityStorage.GetAllUserGroupFormSecurity(form.Id, user.Groups.Select<IReadOnlyUserGroup, int>(x => x.Id).ToArray<int>()).Any<UserGroupFormSecurity>(x => x.HasAccess));
                    UserFormSecurity userFormSecurity3 = new UserFormSecurity();
                    userFormSecurity3.Form = form.Id;
                    userFormSecurity3.HasAccess = flag;
                    userFormSecurity3.User = user.Id.ToString(CultureInfo.InvariantCulture);
                    userFormSecurity3.FormName = form.Name;
                    userFormSecurity3.FormCreated = form.Created;
                    userFormSecurity3.Fields = str;
                    userFormSecurity3.SecurityType = FormSecurityType.Full;
                    userFormSecurity3.AllowInEditor = true;
                    UserFormSecurity formSecurity1 = userFormSecurity3;
                    if (!securityWithUserGroups)
                        this.UserFormSecurityStorage.InsertUserFormSecurity(formSecurity1);
                    userFormSecurity1.Add(formSecurity1);
                }
            }
            List<UserFormSecurity> userFormSecurityList = new List<UserFormSecurity>();
            foreach (UserFormSecurity userFormSecurity4 in userFormSecurity1)
            {
                UserFormSecurity formSecurityDbEntry = userFormSecurity4;
                if (source.SingleOrDefault<IType>(x => x.Id == formSecurityDbEntry.Form) == null)
                    userFormSecurityList.Add(formSecurityDbEntry);
            }
            foreach (UserFormSecurity formSecurity2 in userFormSecurityList)
            {
                userFormSecurity1.Remove(formSecurity2);
                this.UserFormSecurityStorage.DeleteUserFormSecurity(formSecurity2);
            }
            formSecurity.FormsSecurity = userFormSecurity1.OrderBy<UserFormSecurity, string>(x => x.FormName).ToList<UserFormSecurity>();
        }

        private void ApplyStartFolders(FormSecurityForUser formSecurity, int userId) => formSecurity.StartFolderIds = this.UserStartFolderStorage.GetStartFolderKeys(userId).ToList<Guid>();
    }
}
