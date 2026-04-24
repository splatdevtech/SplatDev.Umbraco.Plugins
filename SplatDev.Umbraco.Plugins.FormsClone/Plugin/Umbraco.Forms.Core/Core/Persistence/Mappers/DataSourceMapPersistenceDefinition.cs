
// Type: Umbraco.Forms.Core.Persistence.Mappers.DataSourceMapPersistenceDefinition
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
  [MapperFor(typeof (DataSourceDto))]
  [MapperFor(typeof (DataSourceEntity))]
  internal sealed class DataSourceMapPersistenceDefinition : BaseMapper
  {
    public DataSourceMapPersistenceDefinition(
      Lazy<ISqlContext> sqlContext,
      MapperConfigurationStore maps)
      : base(sqlContext, maps)
    {
    }

    [ExcludeFromCodeCoverage]
    protected override void DefineMaps()
    {
      this.DefineMap<DataSourceEntity, DataSourceDto>("CreateDate", "CreateDate");
      this.DefineMap<DataSourceEntity, DataSourceDto>("CreatedBy", "CreatedBy");
      this.DefineMap<DataSourceEntity, DataSourceDto>("Id", "Id");
      this.DefineMap<DataSourceEntity, DataSourceDto>("Key", "Key");
      this.DefineMap<DataSourceEntity, DataSourceDto>("Name", "Name");
      this.DefineMap<DataSourceEntity, DataSourceDto>("UpdateDate", "UpdateDate");
      this.DefineMap<DataSourceEntity, DataSourceDto>("UpdatedBy", "UpdatedBy");
    }
  }
}
