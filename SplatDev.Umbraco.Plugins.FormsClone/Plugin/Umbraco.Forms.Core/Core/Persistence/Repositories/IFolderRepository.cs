
// Type: Umbraco.Forms.Core.Persistence.Repositories.IFolderRepository
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Persistence;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Repositories
{
  public interface IFolderRepository : 
    IReadWriteQueryRepository<Guid, FolderEntity>,
    IReadRepository<Guid, FolderEntity>,
    IRepository,
    IWriteRepository<FolderEntity>,
    IQueryRepository<FolderEntity>
  {
    IEnumerable<FolderEntity> GetAtRoot();

    IEnumerable<FolderEntity> GetChildren(Guid parentId);

    bool ExistsAndIsEmpty(Guid id);
  }
}
