using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Dtos;

using System.Diagnostics.CodeAnalysis;

using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;

namespace FormBuilder.Core.Persistence.Mappers
{
    [MapperFor(typeof(PrevalueSourceDto))]
    [MapperFor(typeof(PrevalueSourceEntity))]
    internal sealed class PrevalueSourceMapPersistenceDefinition(
      Lazy<ISqlContext> sqlContext,
      MapperConfigurationStore maps) : BaseMapper(sqlContext, maps)
    {
        [ExcludeFromCodeCoverage]
        protected override void DefineMaps()
        {
            DefineMap<PrevalueSourceEntity, PrevalueSourceDto>("Id", "Id");
            DefineMap<PrevalueSourceEntity, PrevalueSourceDto>("Key", "Key");
            DefineMap<PrevalueSourceEntity, PrevalueSourceDto>("Name", "Name");
            DefineMap<PrevalueSourceEntity, PrevalueSourceDto>("CreateDate", "CreateDate");
            DefineMap<PrevalueSourceEntity, PrevalueSourceDto>("CreatedBy", "CreatedBy");
            DefineMap<PrevalueSourceEntity, PrevalueSourceDto>("UpdateDate", "UpdateDate");
            DefineMap<PrevalueSourceEntity, PrevalueSourceDto>("UpdatedBy", "UpdatedBy");
        }
    }
}