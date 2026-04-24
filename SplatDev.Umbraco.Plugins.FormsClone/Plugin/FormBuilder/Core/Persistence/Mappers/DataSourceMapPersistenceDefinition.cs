using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Dtos;

using System.Diagnostics.CodeAnalysis;

using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;

namespace FormBuilder.Core.Persistence.Mappers
{
    [MapperFor(typeof(DataSourceDto))]
    [MapperFor(typeof(DataSourceEntity))]
    internal sealed class DataSourceMapPersistenceDefinition(
      Lazy<ISqlContext> sqlContext,
      MapperConfigurationStore maps) : BaseMapper(sqlContext, maps)
    {
        [ExcludeFromCodeCoverage]
        protected override void DefineMaps()
        {
            DefineMap<DataSourceEntity, DataSourceDto>("CreateDate", "CreateDate");
            DefineMap<DataSourceEntity, DataSourceDto>("CreatedBy", "CreatedBy");
            DefineMap<DataSourceEntity, DataSourceDto>("Id", "Id");
            DefineMap<DataSourceEntity, DataSourceDto>("Key", "Key");
            DefineMap<DataSourceEntity, DataSourceDto>("Name", "Name");
            DefineMap<DataSourceEntity, DataSourceDto>("UpdateDate", "UpdateDate");
            DefineMap<DataSourceEntity, DataSourceDto>("UpdatedBy", "UpdatedBy");
        }
    }
}