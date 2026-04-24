
// Type: Umbraco.Forms.Data.Storage.IUserSecurityStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Data.Storage
{
  public interface IUserSecurityStorage
  {
    List<UserSecurity> GetAllUserSecurity();

    UserSecurity? GetUserSecurity(int userId);

    UserSecurity InsertUserSecurity(UserSecurity usersecurity);

    bool DeleteUserSecurity(UserSecurity userSecurity);

    UserSecurity UpdateUserSecurity(UserSecurity userSecurity);
  }
}
