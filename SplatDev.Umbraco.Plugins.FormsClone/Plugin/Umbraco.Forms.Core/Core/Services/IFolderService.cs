
// Type: Umbraco.Forms.Core.Services.IFolderService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  public interface IFolderService : IBaseService<Folder, FolderEntity>
  {
    IEnumerable<Folder> GetAtRoot();

    IEnumerable<Folder> GetChildren(Guid parentId);

    bool ExistsAndIsEmpty(Guid id);

    string GetPath(Guid id, string prefixIds = "");
  }
}
