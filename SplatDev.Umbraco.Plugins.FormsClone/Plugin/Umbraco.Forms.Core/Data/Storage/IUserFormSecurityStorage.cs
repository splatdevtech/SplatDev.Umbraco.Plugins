
// Type: Umbraco.Forms.Data.Storage.IUserFormSecurityStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Data.Storage
{
  public interface IUserFormSecurityStorage
  {
    List<UserFormSecurity> GetAllUserFormSecurity(string userId);

    UserFormSecurity? GetUserFormSecurity(string userId, Guid form);

    UserFormSecurity InsertUserFormSecurity(UserFormSecurity formSecurity);

    List<UserFormSecurity> GetUserFormSecurityForAllUsers(Guid form);

    bool DeleteUserFormSecurity(UserFormSecurity formSecurity);

    void DeleteAllUserFormSecurityForForm(Guid form);

    void DeleteAllUserFormSecurityForUser(int userId);

    UserFormSecurity UpdateUserFormSecurity(UserFormSecurity formSecurity);
  }
}
