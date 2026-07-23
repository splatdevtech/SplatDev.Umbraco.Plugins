
// Type: Umbraco.Forms.Core.Security.FormsSecurity
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;


#nullable enable
namespace Umbraco.Forms.Core.Security
{
  internal sealed class FormsSecurity : IFormsSecurity
  {
    private readonly IUserFormSecurityStorage _userFormSecurityStorage;
    private readonly IUserSecurityStorage _userSecurityStorage;
    private readonly IUserStartFolderStorage _userStartFolderStorage;
    private readonly IUserGroupSecurityStorage _userGroupSecurityStorage;
    private readonly IUserGroupFormSecurityStorage _userGroupFormSecurityStorage;
    private readonly IUserGroupStartFolderStorage _userGroupStartFolderStorage;
    private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
    private readonly ILogger<FormsSecurity> _logger;
    private readonly IFormService _formService;
    private readonly IFolderService _folderService;
    private readonly IOptions<SecuritySettings> _securitySettings;

    public FormsSecurity(
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
      IOptions<SecuritySettings> securitySettings)
    {
      this._userFormSecurityStorage = userFormSecurityStorage;
      this._userSecurityStorage = userSecurityStorage;
      this._userStartFolderStorage = userStartFolderStorage;
      this._userGroupSecurityStorage = userGroupSecurityStorage;
      this._userGroupFormSecurityStorage = userGroupFormSecurityStorage;
      this._userGroupStartFolderStorage = userGroupStartFolderStorage;
      this._backOfficeSecurityAccessor = backOfficeSecurityAccessor;
      this._logger = logger;
      this._formService = formService;
      this._folderService = folderService;
      this._securitySettings = securitySettings;
    }

    public bool HasAccessToForm(Guid form) => this.DoesCurrentUserHavePermissionForForm(form, (Func<FormSecurityBaseDto, bool>) (x => x.HasAccess), true);

    public bool HasAccessToForm(Guid form, IUser user) => this.DoesUserHavePermissionForForm(user, form, (Func<FormSecurityBaseDto, bool>) (x => x.HasAccess), true);

    private bool DoesCurrentUserHavePermissionForForm(
      Guid form,
      Func<FormSecurityBaseDto, bool> permission,
      bool allowForAdmin = false)
    {
      IUser currentUser = this._backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
      if (currentUser != null)
        return this.DoesUserHavePermissionForForm(currentUser, form, permission, allowForAdmin);
      this._logger.LogWarning("Cannot determine the current user");
      return false;
    }

    private bool DoesUserHavePermissionForForm(
      IUser user,
      Guid form,
      Func<FormSecurityBaseDto, bool> permission,
      bool allowForAdmin = false)
    {
      UserFormSecurity userFormSecurity = this._userFormSecurityStorage.GetUserFormSecurity(user.Id.ToString(), form);
      if (userFormSecurity != null)
        return permission((FormSecurityBaseDto) userFormSecurity);
      if (allowForAdmin && user.IsAdmin())
        return true;
      if (this._userSecurityStorage.GetUserSecurity(user.Id) != null)
        return this._securitySettings.Value.DefaultUserAccessToNewForms == FormAccess.Grant;
      return user.Groups != null && user.Groups.Any<IReadOnlyUserGroup>() && this._securitySettings.Value.ManageSecurityWithUserGroups && this._userGroupFormSecurityStorage.GetAllUserGroupFormSecurity(form, user.Groups.Select<IReadOnlyUserGroup, int>((Func<IReadOnlyUserGroup, int>) (x => x.Id)).ToArray<int>()).Any<UserGroupFormSecurity>((Func<UserGroupFormSecurity, bool>) (x => permission((FormSecurityBaseDto) x)));
    }

    public IEnumerable<Guid> FilterFormIdsForCurrentUser(IEnumerable<Guid> formIds)
    {
      IUser currentUser = this._backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
      if (currentUser != null)
        return this.FilterFormIdsForUser(formIds, currentUser);
      this._logger.LogWarning("Cannot determine the current user");
      return Enumerable.Empty<Guid>();
    }

    public IEnumerable<Guid> FilterFormIdsForUser(
      IEnumerable<Guid> formIds,
      IUser user)
    {
      if (formIds == null)
        return Enumerable.Empty<Guid>();
      List<Guid> list1 = formIds.ToList<Guid>();
      if (list1.Count == 0)
        return Enumerable.Empty<Guid>();
      List<Guid> source = !this._securitySettings.Value.ManageSecurityWithUserGroups ? this.GetFilteredFormIdsUsingUserSecurity(user, list1) : (this._userSecurityStorage.GetUserSecurity(user.Id) == null ? this.GetFilteredFormIdsUsingUserGroupSecurity(user, list1) : this.GetFilteredFormIdsUsingUserSecurity(user, list1));
      if (source.Count == 0)
        return Enumerable.Empty<Guid>();
      List<Guid> list2 = this.GetStartFolderKeysForUser(user).ToList<Guid>();
      if (list2.Count == 0)
        return (IEnumerable<Guid>) source;
      List<Form> list3 = this._formService.Get(source.ToArray()).ToList<Form>();
      List<Guid> rootFormIds = list3.Where<Form>((Func<Form, bool>) (x => !x.FolderId.HasValue)).Select<Form, Guid>((Func<Form, Guid>) (x => x.Id)).ToList<Guid>();
      List<Guid> list4 = source.Where<Guid>((Func<Guid, bool>) (x => !rootFormIds.Contains(x))).ToList<Guid>();
      if (list4.Count == 0)
        return Enumerable.Empty<Guid>();
      List<Guid> formIdsOutsideStartFolderPaths = new List<Guid>();
      foreach (Form form in list3.Where<Form>((Func<Form, bool>) (x => x.FolderId.HasValue)))
      {
        List<string> formFolderPath = ((IEnumerable<string>) this._folderService.GetPath(form.FolderId.Value).Split(',')).ToList<string>();
        if (!list2.Any<Guid>((Func<Guid, bool>) (x => formFolderPath.Contains(x.ToString()))))
          formIdsOutsideStartFolderPaths.Add(form.Id);
      }
      return (IEnumerable<Guid>) list4.Where<Guid>((Func<Guid, bool>) (x => !formIdsOutsideStartFolderPaths.Contains(x))).ToList<Guid>();
    }

    private List<Guid> GetFilteredFormIdsUsingUserSecurity(IUser user, List<Guid> formIds)
    {
      List<UserFormSecurity> userFormSecurity = this._userFormSecurityStorage.GetAllUserFormSecurity(user.Id.ToString());
      List<Guid> formIdsWithExplicitAccess = userFormSecurity.Where<UserFormSecurity>((Func<UserFormSecurity, bool>) (x => x.HasAccess)).Select<UserFormSecurity, Guid>((Func<UserFormSecurity, Guid>) (x => x.Form)).ToList<Guid>();
      List<Guid> formIdsWithImplicitAccess = new List<Guid>();
      if (this._securitySettings.Value.DefaultUserAccessToNewForms == FormAccess.Grant)
        formIdsWithImplicitAccess = formIds.Where<Guid>((Func<Guid, bool>) (x => !userFormSecurity.Select<UserFormSecurity, Guid>((Func<UserFormSecurity, Guid>) (y => y.Form)).Contains<Guid>(x))).ToList<Guid>();
      return formIds.Where<Guid>((Func<Guid, bool>) (x => formIdsWithExplicitAccess.Contains(x) || formIdsWithImplicitAccess.Contains(x))).ToList<Guid>();
    }

    private List<Guid> GetFilteredFormIdsUsingUserGroupSecurity(
      IUser user,
      List<Guid> formIds)
    {
      List<Guid> formIdsAccessibleToUser = new List<Guid>();
      foreach (IReadOnlyUserGroup group in user.Groups)
      {
        List<Guid> list = this._userGroupFormSecurityStorage.GetAllUserGroupFormSecurity(group.Id).Where<UserGroupFormSecurity>((Func<UserGroupFormSecurity, bool>) (x => x.HasAccess)).Select<UserGroupFormSecurity, Guid>((Func<UserGroupFormSecurity, Guid>) (x => x.Form)).ToList<Guid>();
        formIdsAccessibleToUser.AddRange((IEnumerable<Guid>) list);
      }
      return formIds.Where<Guid>((Func<Guid, bool>) (x => formIdsAccessibleToUser.Contains(x))).ToList<Guid>();
    }

    private void EnsureUserExists(IUser user)
    {
      if (this._userSecurityStorage.GetUserSecurity(user.Id) != null)
        return;
      UserSecurity usersecurity = UserSecurity.Create();
      usersecurity.ManageForms = true;
      usersecurity.ViewEntries = true;
      int num = user.IsAdmin() ? 1 : 0;
      usersecurity.User = user.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
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
      this._userSecurityStorage.InsertUserSecurity(usersecurity);
    }

    public bool CanCurrentUserManageWorkflows() => this.DoesCurrentUserHavePermission((Func<SecurityBaseDto, bool>) (x => x.ManageWorkflows), true);

    public bool CanCurrentUserManageForms() => this.DoesCurrentUserHavePermission((Func<SecurityBaseDto, bool>) (x => x.ManageForms), true, !this._securitySettings.Value.ManageSecurityWithUserGroups);

    public bool CanUserManageForms(IUser user) => this.DoesUserHavePermission(user, (Func<SecurityBaseDto, bool>) (x => x.ManageForms), true, !this._securitySettings.Value.ManageSecurityWithUserGroups);

    public bool CanCurrentUserManageDataSources() => this.DoesCurrentUserHavePermission((Func<SecurityBaseDto, bool>) (x => x.ManageDataSources), true);

    public bool CanCurrentUserManagePreValues() => this.DoesCurrentUserHavePermission((Func<SecurityBaseDto, bool>) (x => x.ManagePreValueSources), true);

    public bool CanCurrentUserViewEntries() => this.DoesCurrentUserHavePermission((Func<SecurityBaseDto, bool>) (x => x.ViewEntries), true);

    public bool CanUserViewEntries(IUser user) => this.DoesUserHavePermission(user, (Func<SecurityBaseDto, bool>) (x => x.ViewEntries), true, !this._securitySettings.Value.ManageSecurityWithUserGroups);

    public bool CanCurrentUserEditEntries() => this.DoesCurrentUserHavePermission((Func<SecurityBaseDto, bool>) (x => x.EditEntries));

    public bool CanCurrentUserDeleteEntries() => this.DoesCurrentUserHavePermission((Func<SecurityBaseDto, bool>) (x => x.DeleteEntries));

    private bool DoesCurrentUserHavePermission(
      Func<SecurityBaseDto, bool> permission,
      bool allowForAdmin = false,
      bool createUserSecurityRecordIfNotExists = false)
    {
      IUser currentUser = this._backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
      if (currentUser != null)
        return this.DoesUserHavePermission(currentUser, permission, allowForAdmin, createUserSecurityRecordIfNotExists);
      this._logger.LogWarning("Cannot determine the current user");
      return false;
    }

    private bool DoesUserHavePermission(
      IUser user,
      Func<SecurityBaseDto, bool> permission,
      bool allowForAdmin = false,
      bool createUserSecurityRecordIfNotExists = false)
    {
      if (createUserSecurityRecordIfNotExists)
        this.EnsureUserExists(user);
      UserSecurity userSecurity = this._userSecurityStorage.GetUserSecurity(user.Id);
      if (userSecurity != null)
        return permission((SecurityBaseDto) userSecurity);
      if (allowForAdmin && user.IsAdmin())
        return true;
      return user.Groups != null && user.Groups.Any<IReadOnlyUserGroup>() && this._securitySettings.Value.ManageSecurityWithUserGroups && this._userGroupSecurityStorage.GetAllUserGroupSecurity(user.Groups.Select<IReadOnlyUserGroup, int>((Func<IReadOnlyUserGroup, int>) (x => x.Id)).ToArray<int>()).Any<UserGroupSecurity>((Func<UserGroupSecurity, bool>) (x => permission((SecurityBaseDto) x)));
    }

    public IEnumerable<Guid> GetStartFolderKeysForCurrentUser()
    {
      IUser currentUser = this._backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
      if (currentUser != null)
        return this.GetStartFolderKeysForUser(currentUser);
      this._logger.LogWarning("Cannot determine the current user");
      return Enumerable.Empty<Guid>();
    }

    public IEnumerable<Guid> GetStartFolderKeysForUser(IUser user)
    {
      List<Guid> list1 = this._userStartFolderStorage.GetStartFolderKeys(user.Id).ToList<Guid>();
      if (list1.Any<Guid>())
        return (IEnumerable<Guid>) list1;
      if (user.Groups == null || !user.Groups.Any<IReadOnlyUserGroup>() || !this._securitySettings.Value.ManageSecurityWithUserGroups)
        return Enumerable.Empty<Guid>();
      if (this._userSecurityStorage.GetUserSecurity(user.Id) != null)
        return Enumerable.Empty<Guid>();
      List<Guid> source = new List<Guid>();
      foreach (IReadOnlyUserGroup group in user.Groups)
      {
        if (group.AllowedSections.Contains<string>("forms"))
        {
          UserGroupSecurity userGroupSecurity = this._userGroupSecurityStorage.GetUserGroupSecurity(group.Id);
          if (userGroupSecurity != null && (userGroupSecurity.ManageForms || userGroupSecurity.ViewEntries))
          {
            List<Guid> list2 = this._userGroupStartFolderStorage.GetStartFolderKeys(group.Id).ToList<Guid>();
            if (!list2.Any<Guid>())
              return Enumerable.Empty<Guid>();
            source.AddRange((IEnumerable<Guid>) list2);
          }
        }
      }
      return source.Distinct<Guid>();
    }
  }
}
