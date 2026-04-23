using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Dtos;

namespace FormBuilder.Core.Persistence.Interfaces
{
    public interface IDataSourceFactory
    {
        DataSourceDto BuildDto(DataSourceEntity entity);

        IEnumerable<DataSourceEntity> BuildEntities(
          IEnumerable<DataSourceDto> dtos);

        DataSourceEntity BuildEntity(DataSourceDto dto);

        DataSourceEntitySlim BuildEntitySlim(DataSourceDto dto);
    }
}