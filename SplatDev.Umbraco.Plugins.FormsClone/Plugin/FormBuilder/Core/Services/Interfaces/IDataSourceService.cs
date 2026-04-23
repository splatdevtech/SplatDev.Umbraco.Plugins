using FormBuilder.Core.DataSources;
using FormBuilder.Core.Models;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IDataSourceService : IBaseService<FormDataSource, DataSourceEntity>
    {
        IEnumerable<FormDataSourceSlim> GetSlim(params Guid[] ids);
    }
}