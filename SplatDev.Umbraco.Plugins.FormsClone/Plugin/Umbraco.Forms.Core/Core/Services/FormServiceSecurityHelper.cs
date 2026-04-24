
// Type: Umbraco.Forms.Core.Services.FormServiceSecurityHelper
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Data.Storage;


#nullable enable
namespace Umbraco.Forms.Core.Services
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
        UserFormSecurity userFormSecurity = new UserFormSecurity();
        userFormSecurity.Form = formId;
        userFormSecurity.HasAccess = true;
        userFormSecurity.User = currentUser.Id.ToString();
        userFormSecurity.SecurityType = FormSecurityType.Full;
        userFormSecurity.AllowInEditor = true;
        UserFormSecurity formSecurity = userFormSecurity;
        userFormSecurityStorage.InsertUserFormSecurity(formSecurity);
      }
      else
      {
        if (string.IsNullOrEmpty(userGroupsWithAccessToNewForms))
          return;
        List<string> list = ((IEnumerable<string>) userGroupsWithAccessToNewForms.Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
        if (list.InvariantContains("all"))
        {
          foreach (IUserGroup allUserGroup in userService.GetAllUserGroups())
          {
            if (allUserGroup.AllowedSections.Contains<string>("forms"))
              FormServiceSecurityHelper.AddUserGroupFormSecurity(userGroupFormSecurityStorage, allUserGroup, formId);
          }
        }
        else
        {
          if (list.InvariantContains("form-creator"))
          {
            IEnumerable<string> second = currentUser.Groups.Where<IReadOnlyUserGroup>((Func<IReadOnlyUserGroup, bool>) (x => x.AllowedSections.Contains<string>("forms"))).Select<IReadOnlyUserGroup, string>((Func<IReadOnlyUserGroup, string>) (x => x.Alias));
            list = list.Where<string>((Func<string, bool>) (x => !string.Equals("form-creator", x, StringComparison.InvariantCultureIgnoreCase))).Union<string>(second).Distinct<string>().ToList<string>();
          }
          foreach (string name in list)
          {
            IUserGroup userGroupByAlias = userService.GetUserGroupByAlias(name);
            if (userGroupByAlias != null)
              FormServiceSecurityHelper.AddUserGroupFormSecurity(userGroupFormSecurityStorage, userGroupByAlias, formId);
          }
        }
      }
    }

    private static void AddUserGroupFormSecurity(
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IUserGroup group,
      Guid formId)
    {
      UserGroupFormSecurity groupFormSecurity = new UserGroupFormSecurity();
      groupFormSecurity.Form = formId;
      groupFormSecurity.HasAccess = true;
      groupFormSecurity.UserGroupId = group.Id;
      groupFormSecurity.SecurityType = FormSecurityType.Full;
      groupFormSecurity.AllowInEditor = true;
      UserGroupFormSecurity formSecurity = groupFormSecurity;
      userGroupFormSecurityStorage.InsertUserGroupFormSecurity(formSecurity);
    }
  }
}
