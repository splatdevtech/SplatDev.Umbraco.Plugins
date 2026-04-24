using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Persistence;

namespace FormBuilder.Core.Persistence.Interfaces
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