
// Type: Umbraco.Forms.Core.Models.Mapping.PrevalueSourceMapDefinition
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Mapping;


#nullable enable
namespace Umbraco.Forms.Core.Models.Mapping
{
  internal sealed class PrevalueSourceMapDefinition : IMapDefinition
  {
    public void DefineMaps(IUmbracoMapper mapper)
    {
      mapper.Define<FieldPreValueSource, PrevalueSourceEntity>((Func<FieldPreValueSource, MapperContext, PrevalueSourceEntity>) ((source, context) => new PrevalueSourceEntity()), new Action<FieldPreValueSource, PrevalueSourceEntity, MapperContext>(this.Map));
      mapper.Define<PrevalueSourceEntity, FieldPreValueSource>((Func<PrevalueSourceEntity, MapperContext, FieldPreValueSource>) ((source, context) => new FieldPreValueSource()), new Action<PrevalueSourceEntity, FieldPreValueSource, MapperContext>(this.Map));
      mapper.Define<FieldPreValueSourceSlim, PrevalueSourceEntitySlim>((Func<FieldPreValueSourceSlim, MapperContext, PrevalueSourceEntitySlim>) ((source, context) => new PrevalueSourceEntitySlim()), new Action<FieldPreValueSourceSlim, PrevalueSourceEntitySlim, MapperContext>(this.Map));
      mapper.Define<PrevalueSourceEntitySlim, FieldPreValueSourceSlim>((Func<PrevalueSourceEntitySlim, MapperContext, FieldPreValueSourceSlim>) ((source, context) => new FieldPreValueSourceSlim()), new Action<PrevalueSourceEntitySlim, FieldPreValueSourceSlim, MapperContext>(this.Map));
    }

    private void Map(
      FieldPreValueSource source,
      PrevalueSourceEntity target,
      MapperContext context)
    {
      target.Name = source.Name;
      target.CreateDate = source.Created;
      target.CreatedBy = source.CreatedBy;
      target.UpdateDate = source.Updated;
      target.UpdatedBy = source.UpdatedBy;
      target.FieldPreValueSourceTypeId = source.FieldPreValueSourceTypeId;
      target.Settings = source.Settings;
      target.Key = source.Id;
      target.CachePrevaluesFor = source.CachePrevaluesFor;
    }

    private void Map(
      PrevalueSourceEntity source,
      FieldPreValueSource target,
      MapperContext context)
    {
      target.Name = source.Name;
      target.Created = source.CreateDate;
      target.CreatedBy = source.CreatedBy;
      target.Updated = source.UpdateDate;
      target.UpdatedBy = source.UpdatedBy;
      target.FieldPreValueSourceTypeId = source.FieldPreValueSourceTypeId;
      target.Settings = source.Settings;
      target.Id = source.Key;
      target.CachePrevaluesFor = source.CachePrevaluesFor;
    }

    [ExcludeFromCodeCoverage]
    internal void Map(
      FieldPreValueSourceSlim source,
      PrevalueSourceEntitySlim target,
      MapperContext context)
    {
      target.Name = source.Name;
      target.Key = source.Id;
    }

    [ExcludeFromCodeCoverage]
    internal void Map(
      PrevalueSourceEntitySlim source,
      FieldPreValueSourceSlim target,
      MapperContext context)
    {
      target.Name = source.Name;
      target.Id = source.Key;
    }
  }
}
