
// Type: Umbraco.Forms.Data.Storage.IUserGroupSecurityStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Data.Storage
{
  public interface IUserGroupSecurityStorage
  {
    List<UserGroupSecurity> GetAllUserGroupSecurity(params int[] groupIds);

    UserGroupSecurity? GetUserGroupSecurity(int groupId);

    UserGroupSecurity InsertUserGroupSecurity(UserGroupSecurity usersecurity);

    bool DeleteUserGroupSecurity(UserGroupSecurity userSecurity);

    UserGroupSecurity UpdateUserGroupSecurity(UserGroupSecurity userSecurity);
  }
}
