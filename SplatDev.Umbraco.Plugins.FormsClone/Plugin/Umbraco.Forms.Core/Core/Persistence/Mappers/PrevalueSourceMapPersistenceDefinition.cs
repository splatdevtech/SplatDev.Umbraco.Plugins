
// Type: Umbraco.Forms.Core.Persistence.Mappers.PrevalueSourceMapPersistenceDefinition
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
  [MapperFor(typeof (PrevalueSourceDto))]
  [MapperFor(typeof (PrevalueSourceEntity))]
  internal sealed class PrevalueSourceMapPersistenceDefinition : BaseMapper
  {
    public PrevalueSourceMapPersistenceDefinition(
      Lazy<ISqlContext> sqlContext,
      MapperConfigurationStore maps)
      : base(sqlContext, maps)
    {
    }

    [ExcludeFromCodeCoverage]
    protected override void DefineMaps()
    {
      this.DefineMap<PrevalueSourceEntity, PrevalueSourceDto>("Id", "Id");
      this.DefineMap<PrevalueSourceEntity, PrevalueSourceDto>("Key", "Key");
      this.DefineMap<PrevalueSourceEntity, PrevalueSourceDto>("Name", "Name");
      this.DefineMap<PrevalueSourceEntity, PrevalueSourceDto>("CreateDate", "CreateDate");
      this.DefineMap<PrevalueSourceEntity, PrevalueSourceDto>("CreatedBy", "CreatedBy");
      this.DefineMap<PrevalueSourceEntity, PrevalueSourceDto>("UpdateDate", "UpdateDate");
      this.DefineMap<PrevalueSourceEntity, PrevalueSourceDto>("UpdatedBy", "UpdatedBy");
    }
  }
}
