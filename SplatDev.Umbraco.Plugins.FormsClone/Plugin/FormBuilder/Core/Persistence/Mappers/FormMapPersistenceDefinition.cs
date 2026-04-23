using FormBuilder.Core.Dto;
using FormBuilder.Core.Models;

using System.Diagnostics.CodeAnalysis;

using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;

namespace FormBuilder.Core.Persistence.Mappers
{
    [MapperFor(typeof(FormDto))]
    [MapperFor(typeof(FormEntity))]
    internal sealed class FormMapPersistenceDefinition(Lazy<ISqlContext> sqlContext, MapperConfigurationStore maps) : BaseMapper(sqlContext, maps)
    {
        [ExcludeFromCodeCoverage]
        protected override void DefineMaps()
        {
            DefineMap<FormEntity, FormDto>("CreateDate", "CreateDate");
            DefineMap<FormEntity, FormDto>("CreatedBy", "CreatedBy");
            DefineMap<FormEntity, FormDto>("Id", "Id");
            DefineMap<FormEntity, FormDto>("Key", "Key");
            DefineMap<FormEntity, FormDto>("Name", "Name");
            DefineMap<FormEntity, FormDto>("UpdateDate", "UpdateDate");
            DefineMap<FormEntity, FormDto>("UpdatedBy", "UpdatedBy");
            DefineMap<FormEntity, FormDto>("FolderId", "FolderKey");
        }
    }
}