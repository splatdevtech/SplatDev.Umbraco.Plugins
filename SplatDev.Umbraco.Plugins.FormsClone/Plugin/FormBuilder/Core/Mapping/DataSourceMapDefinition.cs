using FormBuilder.Core.DataSources;
using FormBuilder.Core.Models;

using System.Diagnostics.CodeAnalysis;

using Umbraco.Cms.Core.Mapping;

namespace FormBuilder.Core.Mapping
{
    internal sealed class DataSourceMapDefinition : IMapDefinition
    {
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define((source, context) => new DataSourceEntity(), new Action<FormDataSource, DataSourceEntity, MapperContext>(Map));
            mapper.Define((source, context) => new FormDataSource(), new Action<DataSourceEntity, FormDataSource, MapperContext>(Map));
            mapper.Define((source, context) => new DataSourceEntitySlim(), new Action<FormDataSourceSlim, DataSourceEntitySlim, MapperContext>(Map));
            mapper.Define((source, context) => new FormDataSourceSlim(), new Action<DataSourceEntitySlim, FormDataSourceSlim, MapperContext>(Map));
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