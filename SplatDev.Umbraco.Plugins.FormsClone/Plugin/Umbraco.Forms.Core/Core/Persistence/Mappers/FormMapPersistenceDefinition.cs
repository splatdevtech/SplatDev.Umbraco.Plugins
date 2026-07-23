
// Type: Umbraco.Forms.Core.Persistence.Mappers.FormMapPersistenceDefinition
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Mappers
{
  [MapperFor(typeof (FormDto))]
  [MapperFor(typeof (FormEntity))]
  internal sealed class FormMapPersistenceDefinition : BaseMapper
  {
    public FormMapPersistenceDefinition(Lazy<ISqlContext> sqlContext, MapperConfigurationStore maps)
      : base(sqlContext, maps)
    {
    }

    [ExcludeFromCodeCoverage]
    protected override void DefineMaps()
    {
      this.DefineMap<FormEntity, FormDto>("CreateDate", "CreateDate");
      this.DefineMap<FormEntity, FormDto>("CreatedBy", "CreatedBy");
      this.DefineMap<FormEntity, FormDto>("Id", "Id");
      this.DefineMap<FormEntity, FormDto>("Key", "Key");
      this.DefineMap<FormEntity, FormDto>("Name", "Name");
      this.DefineMap<FormEntity, FormDto>("UpdateDate", "UpdateDate");
      this.DefineMap<FormEntity, FormDto>("UpdatedBy", "UpdatedBy");
      this.DefineMap<FormEntity, FormDto>("FolderId", "FolderKey");
    }
  }
}
