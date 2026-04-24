
// Type: Umbraco.Forms.Core.Persistence.Mappers.FolderMapPersistenceDefinition
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Diagnostics.CodeAnalysis;

using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Mappers
{
    [MapperFor(typeof(FormDto))]
    [MapperFor(typeof(FolderEntity))]
    internal sealed class FolderMapPersistenceDefinition : BaseMapper
    {
        public FolderMapPersistenceDefinition(
          Lazy<ISqlContext> sqlContext,
          MapperConfigurationStore maps)
          : base(sqlContext, maps)
        {
        }

        [ExcludeFromCodeCoverage]
        protected override void DefineMaps()
        {
            this.DefineMap<FolderEntity, FolderDto>("CreateDate", "CreateDate");
            this.DefineMap<FolderEntity, FolderDto>("UpdateDate", "UpdateDate");
            this.DefineMap<FolderEntity, FolderDto>("Id", "Id");
            this.DefineMap<FolderEntity, FolderDto>("Key", "Key");
            this.DefineMap<FolderEntity, FolderDto>("Name", "Name");
            this.DefineMap<FolderEntity, FolderDto>("ParentKey", "ParentKey");
        }
    }
}
