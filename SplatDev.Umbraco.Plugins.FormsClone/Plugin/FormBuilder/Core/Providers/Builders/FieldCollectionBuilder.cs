using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Providers.Collections;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Builders
{
    public class FieldCollectionBuilder :
      LazyCollectionBuilderBase<FieldCollectionBuilder, FieldCollection, FieldType>
    {
        protected override FieldCollectionBuilder This => this;
    }
}