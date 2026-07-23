using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers.Collections;

using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Builders
{
    public class FieldPrevalueSourceCollectionBuilder :
      LazyCollectionBuilderBase<FieldPrevalueSourceCollectionBuilder, FieldPrevalueSourceCollection, FieldPrevalueSourceType>
    {
        protected override ServiceLifetime CollectionLifetime => ServiceLifetime.Transient;

        protected override FieldPrevalueSourceCollectionBuilder This => this;
    }
}