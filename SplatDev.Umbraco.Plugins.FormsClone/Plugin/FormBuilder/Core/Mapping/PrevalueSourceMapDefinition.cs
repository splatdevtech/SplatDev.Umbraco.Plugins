using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;

using System.Diagnostics.CodeAnalysis;

using Umbraco.Cms.Core.Mapping;

namespace FormBuilder.Core.Mapping
{
    internal sealed class PrevalueSourceMapDefinition : IMapDefinition
    {
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define((source, context) => new PrevalueSourceEntity(), new Action<FieldPrevalueSource, PrevalueSourceEntity, MapperContext>(Map));
            mapper.Define((source, context) => new FieldPrevalueSource(), new Action<PrevalueSourceEntity, FieldPrevalueSource, MapperContext>(Map));
            mapper.Define((source, context) => new PrevalueSourceEntitySlim(), new Action<FieldPrevalueSourceSlim, PrevalueSourceEntitySlim, MapperContext>(Map));
            mapper.Define((source, context) => new FieldPrevalueSourceSlim(), new Action<PrevalueSourceEntitySlim, FieldPrevalueSourceSlim, MapperContext>(Map));
        }

        private void Map(
          FieldPrevalueSource source,
          PrevalueSourceEntity target,
          MapperContext context)
        {
            target.Name = source.Name;
            target.CreateDate = source.Created;
            target.CreatedBy = source.CreatedBy;
            target.UpdateDate = source.Updated;
            target.UpdatedBy = source.UpdatedBy;
            target.FieldPrevalueSourceTypeId = source.FieldPrevalueSourceTypeId;
            target.Settings = source.Settings;
            target.Key = source.Id;
            target.CachePrevaluesFor = source.CachePrevaluesFor;
        }

        private void Map(
          PrevalueSourceEntity source,
          FieldPrevalueSource target,
          MapperContext context)
        {
            target.Name = source.Name;
            target.Created = source.CreateDate;
            target.CreatedBy = source.CreatedBy;
            target.Updated = source.UpdateDate;
            target.UpdatedBy = source.UpdatedBy;
            target.FieldPrevalueSourceTypeId = source.FieldPrevalueSourceTypeId;
            target.Settings = source.Settings;
            target.Id = source.Key;
            target.CachePrevaluesFor = source.CachePrevaluesFor;
        }

        [ExcludeFromCodeCoverage]
        internal void Map(
          FieldPrevalueSourceSlim source,
          PrevalueSourceEntitySlim target,
          MapperContext context)
        {
            target.Name = source.Name;
            target.Key = source.Id;
        }

        [ExcludeFromCodeCoverage]
        internal void Map(
          PrevalueSourceEntitySlim source,
          FieldPrevalueSourceSlim target,
          MapperContext context)
        {
            target.Name = source.Name;
            target.Id = source.Key;
        }
    }
}