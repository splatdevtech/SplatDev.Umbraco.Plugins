using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Persistence;

namespace FormBuilder.Core.Persistence.Interfaces
{
    public interface IFormRepository :
      IReadWriteQueryRepository<Guid, FormEntity>,
      IReadRepository<Guid, FormEntity>,
      IRepository,
      IWriteRepository<FormEntity>,
      IQueryRepository<FormEntity>
    {
        IEnumerable<FormEntitySlim> GetManySlim();

        IEnumerable<FormEntity> GetAtRoot();

        IEnumerable<FormEntitySlim> GetAtRootSlim();

        FormEntitySlim? GetSlim(Guid id);

        IEnumerable<FormEntity> GetFromFolder(Guid parentFolderId);

        IEnumerable<FormEntitySlim> GetFromFolderSlim(Guid parentFolderId);
    }
}