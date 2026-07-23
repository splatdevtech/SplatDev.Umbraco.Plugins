using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Dtos;

using System.Diagnostics.CodeAnalysis;

using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;

namespace FormBuilder.Core.Persistence.Mappers
{
    [MapperFor(typeof(FormDto))]
    [MapperFor(typeof(FolderEntity))]
    internal sealed class FolderMapPersistenceDefinition(
      Lazy<ISqlContext> sqlContext,
      MapperConfigurationStore maps) : BaseMapper(sqlContext, maps)
    {
        [ExcludeFromCodeCoverage]
        protected override void DefineMaps()
        {
            DefineMap<FolderEntity, FolderDto>("CreateDate", "CreateDate");
            DefineMap<FolderEntity, FolderDto>("UpdateDate", "UpdateDate");
            DefineMap<FolderEntity, FolderDto>("Id", "Id");
            DefineMap<FolderEntity, FolderDto>("Key", "Key");
            DefineMap<FolderEntity, FolderDto>("Name", "Name");
            DefineMap<FolderEntity, FolderDto>("ParentKey", "ParentKey");
        }
    }
}