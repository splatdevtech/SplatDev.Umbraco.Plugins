using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Dtos;
using FormBuilder.Core.Persistence.Security;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Globalization;

using Umbraco.Cms.Core.Models.Membership;

using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;

namespace FormBuilder.Core.Security
{
    internal sealed class FormsSecurity(
      IUserFormSecurityStorage userFormSecurityStorage,
      IUserSecurityStorage userSecurityStorage,
      IUserStartFolderStorage userStartFolderStorage,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IUserGroupStartFolderStorage userGroupStartFolderStorage,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      ILogger<FormsSecurity> logger,
      IFormService formService,
      IFolderService folderService,
      IOptions<SecuritySettings> securitySettings) : IFormsSecurity
    {
        private readonly IUserFormSecurityStorage _userFormSecurityStorage = userFormSecurityStorage;
        private readonly IUserSecurityStorage _userSecurityStorage = userSecurityStorage;
        private readonly IUserStartFolderStorage _userStartFolderStorage = userStartFolderStorage;
        private readonly IUserGroupSecurityStorage _userGroupSecurityStorage = userGroupSecurityStorage;
        private readonly IUserGroupFormSecurityStorage _userGroupFormSecurityStorage = userGroupFormSecurityStorage;
        private readonly IUserGroupStartFolderStorage _userGroupStartFolderStorage = userGroupStartFolderStorage;
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        private readonly ILogger<FormsSecurity> _logger = logger;
        private readonly IFormService _formService = formService;
        private readonly IFolderService _folderService = folderService;
        private readonly IOptions<SecuritySettings> _securitySettings = securitySettings;

        public bool HasAccessToForm(Guid form) => DoesCurrentUserHavePermissionForForm(form, x => x.HasAccess, true);

        public bool HasAccessToForm(Guid form, IUser user) => DoesUserHavePermissionForForm(user, form, x => x.HasAccess, true);

        private bool DoesCurrentUserHavePermissionForForm(
          Guid form,
          Func<FormSecurityBaseDto, bool> permission,
          bool allowForAdmin = false)
        {
            IUser? currentUser = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            if (currentUser is not null)
                return DoesUserHavePermissionForForm(currentUser, form, permission, allowForAdmin);
            _logger.LogWarning("Cannot determine the current user");
            return false;
        }

        private bool DoesUserHavePermissionForForm(
          IUser user,
          Guid form,
          Func<FormSecurityBaseDto, bool> permission,
          bool allowForAdmin = false)
        {
            UserFormSecurity? userFormSecurity = _userFormSecurityStorage.GetUserFormSecurity(user.Id.ToString(), form);
            if (userFormSecurity is not null)
                return permission(userFormSecurity);
            if (allowForAdmin && user.IsAdmin())
                return true;
            if (_userSecurityStorage.GetUserSecurity(user.Id) is not null)
                return _securitySettings.Value.DefaultUserAccessToNewForms == FormAccess.Grant;
            return user.Groups is not null && user.Groups.Any() && _securitySettings.Value.ManageSecurityWithUserGroups && _userGroupFormSecurityStorage.GetAllUserGroupFormSecurity(form, [.. user.Groups.Select(x => x.Id)]).Any(x => permission(x));
        }

        public IEnumerable<Guid> FilterFormIdsForCurrentUser(IEnumerable<Guid> formIds)
        {
            IUser? currentUser = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            if (currentUser is not null)
                return FilterFormIdsForUser(formIds, currentUser);
            _logger.LogWarning("Cannot determine the current user");
            return [];
        }

        public IEnumerable<Guid> FilterFormIdsForUser(
          IEnumerable<Guid> formIds,
          IUser user)
        {
            if (formIds is null)
                return [];
            List<Guid> list1 = [.. formIds];
            if (list1.Count == 0)
                return [];
            List<Guid> source = !_securitySettings.Value.ManageSecurityWithUserGroups ? GetFilteredFormIdsUsingUserSecurity(user, list1) : _userSecurityStorage.GetUserSecurity(user.Id) is null ? GetFilteredFormIdsUsingUserGroupSecurity(user, list1) : GetFilteredFormIdsUsingUserSecurity(user, list1);
            if (source.Count == 0)
                return [];
            List<Guid> list2 = [.. GetStartFolderKeysForUser(user)];
            if (list2.Count == 0)
                return source;
            List<Form> list3 = [.. _formService.Get([.. source])];
            List<Guid> rootFormIds = [.. list3.Where(x => !x.FolderId.HasValue).Select(x => x.Id)];
            List<Guid> list4 = [.. source.Where(x => !rootFormIds.Contains(x))];
            if (list4.Count == 0)
                return [];
            List<Guid> formIdsOutsideStartFolderPaths = [];
            foreach (Form form in list3.Where(x => x.FolderId.HasValue))
            {
                List<string> formFolderPath = [.. _folderService.GetPath(form.FolderId!.Value).Split(',')];
                if (!list2.Any(x => formFolderPath.Contains(x.ToString())))
                    formIdsOutsideStartFolderPaths.Add(form.Id);
            }
            return [.. list4.Where(x => !formIdsOutsideStartFolderPaths.Contains(x))];
        }

        private List<Guid> GetFilteredFormIdsUsingUserSecurity(IUser user, List<Guid> formIds)
        {
            List<UserFormSecurity> userFormSecurity = _userFormSecurityStorage.GetAllUserFormSecurity(user.Id.ToString());
            List<Guid> formIdsWithExplicitAccess = [.. userFormSecurity.Where(x => x.HasAccess).Select(x => x.Form)];
            List<Guid> formIdsWithImplicitAccess = [];
            if (_securitySettings.Value.DefaultUserAccessToNewForms == FormAccess.Grant)
                formIdsWithImplicitAccess = [.. formIds.Where(x => !userFormSecurity.Select(y => y.Form).Contains(x))];
            return [.. formIds.Where(x => formIdsWithExplicitAccess.Contains(x) || formIdsWithImplicitAccess.Contains(x))];
        }

        private List<Guid> GetFilteredFormIdsUsingUserGroupSecurity(
          IUser user,
          List<Guid> formIds)
        {
            List<Guid> formIdsAccessibleToUser = [];
            foreach (IReadOnlyUserGroup group in user.Groups)
            {
                List<Guid> list = [.. _userGroupFormSecurityStorage.GetAllUserGroupFormSecurity(group.Id).Where(x => x.HasAccess).Select(x => x.Form)];
                formIdsAccessibleToUser.AddRange(list);
            }
            return [.. formIds.Where(x => formIdsAccessibleToUser.Contains(x))];
        }

        private void EnsureUserExists(IUser user)
        {
            if (_userSecurityStorage.GetUserSecurity(user.Id) is not null)
                return;
            UserSecurity? usersecurity = UserSecurity.Create();
            usersecurity.ManageForms = true;
            usersecurity.ViewEntries = true;
            int num = user.IsAdmin() ? 1 : 0;
            usersecurity.User = user.Id.ToString(CultureInfo.InvariantCulture);
            if (num != 0)
            {
                usersecurity.ManageDataSources = true;
                usersecurity.ManagePreValueSources = true;
                usersecurity.ManageWorkflows = true;
                usersecurity.EditEntries = true;
                usersecurity.DeleteEntries = false;
            }
            else
            {
                usersecurity.ManageDataSources = false;
                usersecurity.ManagePreValueSources = false;
                usersecurity.ManageWorkflows = false;
                usersecurity.EditEntries = false;
                usersecurity.DeleteEntries = false;
            }
            _userSecurityStorage.InsertUserSecurity(usersecurity);
        }

        public bool CanCurrentUserManageWorkflows() => DoesCurrentUserHavePermission(x => x.ManageWorkflows, true);

        public bool CanCurrentUserManageForms() => DoesCurrentUserHavePermission(x => x.ManageForms, true, !_securitySettings.Value.ManageSecurityWithUserGroups);

        public bool CanUserManageForms(IUser user) => DoesUserHavePermission(user, x => x.ManageForms, true, !_securitySettings.Value.ManageSecurityWithUserGroups);

        public bool CanCurrentUserManageDataSources() => DoesCurrentUserHavePermission(x => x.ManageDataSources, true);

        public bool CanCurrentUserManagePreValues() => DoesCurrentUserHavePermission(x => x.ManagePreValueSources, true);

        public bool CanCurrentUserViewEntries() => DoesCurrentUserHavePermission(x => x.ViewEntries, true);

        public bool CanUserViewEntries(IUser user) => DoesUserHavePermission(user, x => x.ViewEntries, true, !_securitySettings.Value.ManageSecurityWithUserGroups);

        public bool CanCurrentUserEditEntries() => DoesCurrentUserHavePermission(x => x.EditEntries);

        public bool CanCurrentUserDeleteEntries() => DoesCurrentUserHavePermission(x => x.DeleteEntries);

        private bool DoesCurrentUserHavePermission(
          Func<SecurityBaseDto, bool> permission,
          bool allowForAdmin = false,
          bool createUserSecurityRecordIfNotExists = false)
        {
            IUser? currentUser = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            if (currentUser is not null)
                return DoesUserHavePermission(currentUser, permission, allowForAdmin, createUserSecurityRecordIfNotExists);
            _logger.LogWarning("Cannot determine the current user");
            return false;
        }

        private bool DoesUserHavePermission(
          IUser user,
          Func<SecurityBaseDto, bool> permission,
          bool allowForAdmin = false,
          bool createUserSecurityRecordIfNotExists = false)
        {
            if (createUserSecurityRecordIfNotExists)
                EnsureUserExists(user);
            UserSecurity? userSecurity = _userSecurityStorage.GetUserSecurity(user.Id);
            if (userSecurity is not null)
                return permission(userSecurity);
            if (allowForAdmin && user.IsAdmin())
                return true;
            return user.Groups is not null && user.Groups.Any() && _securitySettings.Value.ManageSecurityWithUserGroups && _userGroupSecurityStorage.GetAllUserGroupSecurity([.. user.Groups.Select(x => x.Id)]).Any(x => permission(x));
        }

        public IEnumerable<Guid> GetStartFolderKeysForCurrentUser()
        {
            IUser? currentUser = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            if (currentUser is not null)
                return GetStartFolderKeysForUser(currentUser);
            _logger.LogWarning("Cannot determine the current user");
            return [];
        }

        public IEnumerable<Guid> GetStartFolderKeysForUser(IUser user)
        {
            List<Guid> list1 = [.. _userStartFolderStorage.GetStartFolderKeys(user.Id)];
            if (list1.Count != 0)
                return list1;
            if (user.Groups is null || !user.Groups.Any() || !_securitySettings.Value.ManageSecurityWithUserGroups)
                return [];
            if (_userSecurityStorage.GetUserSecurity(user.Id) is not null)
                return [];
            List<Guid> source = [];
            foreach (IReadOnlyUserGroup group in user.Groups)
            {
                if (group.AllowedSections.Contains("forms"))
                {
                    UserGroupSecurity? userGroupSecurity = _userGroupSecurityStorage.GetUserGroupSecurity(group.Id);
                    if (userGroupSecurity is not null && (userGroupSecurity.ManageForms || userGroupSecurity.ViewEntries))
                    {
                        List<Guid> list2 = [.. _userGroupStartFolderStorage.GetStartFolderKeys(group.Id)];
                        if (list2.Count == 0)
                            return [];
                        source.AddRange(list2);
                    }
                }
            }
            return source.Distinct();
        }
    }
}