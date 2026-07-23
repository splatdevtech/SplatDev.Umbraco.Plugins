using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Persistence;

namespace FormBuilder.Core.Persistence.Interfaces
{
    public interface IDataSourceRepository :
      IReadWriteQueryRepository<Guid, DataSourceEntity>,
      IReadRepository<Guid, DataSourceEntity>,
      IRepository,
      IWriteRepository<DataSourceEntity>,
      IQueryRepository<DataSourceEntity>
    {
        IEnumerable<DataSourceEntitySlim> GetManySlim(params Guid[]? ids);
    }
}