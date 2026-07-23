
// Type: Umbraco.Forms.Data.Storage.IUserStartFolderStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;


#nullable enable
namespace Umbraco.Forms.Data.Storage
{
  public interface IUserStartFolderStorage
  {
    IEnumerable<Guid> GetStartFolderKeys(int userId);

    void UpdateStartFolders(int userId, IEnumerable<Guid> folderKeys);
  }
}
