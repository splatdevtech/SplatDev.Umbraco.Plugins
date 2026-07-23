using FormBuilder.Core.Enums;
using FormBuilder.Core.Persistence.Security;
using FormBuilder.Core.Storage.Interfaces;

using Umbraco.Cms.Core.Models.Membership;

using Umbraco.Cms.Core.Services;

using Umbraco.Extensions;

namespace FormBuilder.Core.Services
{
    internal static class FormServiceSecurityHelper
    {
        private const string GrantAccessToNewFormsForUserGroupsValueForAll = "all";
        private const string GrantAccessToNewFormsForUserGroupsValueForFormCreator = "form-creator";

        internal static void SetPermissionsForNewForm(
          Guid formId,
          IUser currentUser,
          IUserService userService,
          IUserFormSecurityStorage userFormSecurityStorage,
          IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
          string userGroupsWithAccessToNewForms,
          bool manageSecurityWithUserGroups)
        {
            if (!manageSecurityWithUserGroups)
            {
                UserFormSecurity userFormSecurity = new()
                {
                    Form = formId,
                    HasAccess = true,
                    User = currentUser.Id.ToString(),
                    SecurityType = FormSecurityType.Full,
                    AllowInEditor = true
                };
                UserFormSecurity formSecurity = userFormSecurity;
                userFormSecurityStorage.InsertUserFormSecurity(formSecurity);
            }
            else
            {
                if (string.IsNullOrEmpty(userGroupsWithAccessToNewForms))
                    return;
                List<string> list = [.. userGroupsWithAccessToNewForms.Split(
                [
                    ','
                ], StringSplitOptions.RemoveEmptyEntries)];
                if (list.InvariantContains("all"))
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    foreach (IUserGroup allUserGroup in userService.GetAllUserGroups())
                    {
                        if (allUserGroup.AllowedSections.Contains("forms"))
                            AddUserGroupFormSecurity(userGroupFormSecurityStorage, allUserGroup, formId);
                    }
#pragma warning restore CS0618 // Type or member is obsolete
                }
                else
                {
                    if (list.InvariantContains("form-creator"))
                    {
                        IEnumerable<string> second = currentUser.Groups.Where(x => x.AllowedSections.Contains("forms")).Select(x => x.Alias);
                        list = [.. list.Where(x => !string.Equals("form-creator", x, StringComparison.OrdinalIgnoreCase)).Union(second).Distinct()];
                    }
                    foreach (string name in list)
                    {
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                        IUserGroup userGroupByAlias = userService.GetUserGroupByAlias(name);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS0618 // Type or member is obsolete
                        if (userGroupByAlias is not null)
                            AddUserGroupFormSecurity(userGroupFormSecurityStorage, userGroupByAlias, formId);
                    }
                }
            }
        }

        private static void AddUserGroupFormSecurity(
          IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
          IUserGroup group,
          Guid formId)
        {
            UserGroupFormSecurity groupFormSecurity = new()
            {
                Form = formId,
                HasAccess = true,
                UserGroupId = group.Id,
                SecurityType = FormSecurityType.Full,
                AllowInEditor = true
            };
            UserGroupFormSecurity formSecurity = groupFormSecurity;
            userGroupFormSecurityStorage.InsertUserGroupFormSecurity(formSecurity);
        }
    }
}