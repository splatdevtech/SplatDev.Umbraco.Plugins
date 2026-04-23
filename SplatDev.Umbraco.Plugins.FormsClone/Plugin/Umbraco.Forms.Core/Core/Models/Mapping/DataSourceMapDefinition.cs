
// Type: Umbraco.Forms.Core.Models.Mapping.DataSourceMapDefinition
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Mapping;


#nullable enable
namespace Umbraco.Forms.Core.Models.Mapping
{
  internal sealed class DataSourceMapDefinition : IMapDefinition
  {
    public void DefineMaps(IUmbracoMapper mapper)
    {
      mapper.Define<FormDataSource, DataSourceEntity>((Func<FormDataSource, MapperContext, DataSourceEntity>) ((source, context) => new DataSourceEntity()), new Action<FormDataSource, DataSourceEntity, MapperContext>(this.Map));
      mapper.Define<DataSourceEntity, FormDataSource>((Func<DataSourceEntity, MapperContext, FormDataSource>) ((source, context) => new FormDataSource()), new Action<DataSourceEntity, FormDataSource, MapperContext>(this.Map));
      mapper.Define<FormDataSourceSlim, DataSourceEntitySlim>((Func<FormDataSourceSlim, MapperContext, DataSourceEntitySlim>) ((source, context) => new DataSourceEntitySlim()), new Action<FormDataSourceSlim, DataSourceEntitySlim, MapperContext>(this.Map));
      mapper.Define<DataSourceEntitySlim, FormDataSourceSlim>((Func<DataSourceEntitySlim, MapperContext, FormDataSourceSlim>) ((source, context) => new FormDataSourceSlim()), new Action<DataSourceEntitySlim, FormDataSourceSlim, MapperContext>(this.Map));
    }

    private void Map(FormDataSource source, DataSourceEntity target, MapperContext context)
    {
      target.Name = source.Name;
      target.CreateDate = source.Created;
      target.CreatedBy = source.CreatedBy;
      target.UpdateDate = source.Updated;
      target.UpdatedBy = source.UpdatedBy;
      target.FormDataSourceTypeId = source.FormDataSourceTypeId;
      target.Settings = source.Settings;
      target.Key = source.Id;
    }

    private void Map(DataSourceEntity source, FormDataSource target, MapperContext context)
    {
      target.Name = source.Name;
      target.Created = source.CreateDate;
      target.CreatedBy = source.CreatedBy;
      target.Updated = source.UpdateDate;
      target.UpdatedBy = source.UpdatedBy;
      target.FormDataSourceTypeId = source.FormDataSourceTypeId;
      target.Settings = source.Settings;
      target.Id = source.Key;
    }

    [ExcludeFromCodeCoverage]
    internal void Map(
      FormDataSourceSlim source,
      DataSourceEntitySlim target,
      MapperContext context)
    {
      target.Name = source.Name;
      target.Key = source.Id;
    }

    [ExcludeFromCodeCoverage]
    internal void Map(
      DataSourceEntitySlim source,
      FormDataSourceSlim target,
      MapperContext context)
    {
      target.Name = source.Name;
      target.Id = source.Key;
    }
  }
}
