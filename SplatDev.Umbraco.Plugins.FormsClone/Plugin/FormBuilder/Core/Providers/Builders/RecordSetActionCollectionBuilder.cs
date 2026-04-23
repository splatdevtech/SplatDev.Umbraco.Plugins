using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Builders
{
    public class RecordSetActionCollectionBuilder :
      LazyCollectionBuilderBase<RecordSetActionCollectionBuilder, RecordSetActionCollection, RecordSetActionType>
    {
        protected override RecordSetActionCollectionBuilder This => this;
    }
}