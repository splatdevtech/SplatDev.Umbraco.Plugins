
// Type: Umbraco.Forms.Data.Storage.IUserGroupFormSecurityStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Data.Storage
{
  public interface IUserGroupFormSecurityStorage
  {
    List<UserGroupFormSecurity> GetAllUserGroupFormSecurity(int groupId);

    List<UserGroupFormSecurity> GetAllUserGroupFormSecurity(
      params int[] groupIds);

    List<UserGroupFormSecurity> GetAllUserGroupFormSecurity(
      Guid form,
      params int[] groupIds);

    UserGroupFormSecurity? GetUserGroupFormSecurity(int groupId, Guid form);

    UserGroupFormSecurity InsertUserGroupFormSecurity(
      UserGroupFormSecurity formSecurity);

    List<UserGroupFormSecurity> GetUserGroupFormSecurityForAllUsers(
      Guid form);

    bool DeleteUserGroupFormSecurity(UserGroupFormSecurity formSecurity);

    void DeleteAllUserGroupFormSecurityForForm(Guid form);

    void DeleteAllUserGroupFormSecurityForUserGroup(int userGroupId);

    UserGroupFormSecurity UpdateUserGroupFormSecurity(
      UserGroupFormSecurity formSecurity);
  }
}
