
// Type: Umbraco.Forms.Data.Storage.IUserGroupStartFolderStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;


#nullable enable
namespace Umbraco.Forms.Data.Storage
{
  public interface IUserGroupStartFolderStorage
  {
    IEnumerable<Guid> GetStartFolderKeys(int userGroupId);

    void UpdateStartFolders(int userGroupId, IEnumerable<Guid> folderKeys);
  }
}
