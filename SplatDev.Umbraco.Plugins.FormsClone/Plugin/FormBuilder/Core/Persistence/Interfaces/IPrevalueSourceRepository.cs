using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Persistence;

namespace FormBuilder.Core.Persistence.Interfaces
{
    public interface IPrevalueSourceRepository :
      IReadWriteQueryRepository<Guid, PrevalueSourceEntity>,
      IReadRepository<Guid, PrevalueSourceEntity>,
      IRepository,
      IWriteRepository<PrevalueSourceEntity>,
      IQueryRepository<PrevalueSourceEntity>
    {
        IEnumerable<PrevalueSourceEntitySlim> GetManySlim(
          params Guid[]? ids);
    }
}