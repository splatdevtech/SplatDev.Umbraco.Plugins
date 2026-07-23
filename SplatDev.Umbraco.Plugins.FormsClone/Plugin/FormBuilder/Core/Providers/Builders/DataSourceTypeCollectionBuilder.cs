using FormBuilder.Core.DataSources;
using FormBuilder.Core.Providers.Collections;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Builders
{
    public class DataSourceTypeCollectionBuilder :
      LazyCollectionBuilderBase<DataSourceTypeCollectionBuilder, DataSourceTypeCollection, FormDataSourceType>
    {
        protected override DataSourceTypeCollectionBuilder This => this;
    }
}