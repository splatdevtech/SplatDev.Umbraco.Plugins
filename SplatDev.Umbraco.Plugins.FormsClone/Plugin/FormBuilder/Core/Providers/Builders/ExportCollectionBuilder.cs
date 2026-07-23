using FormBuilder.Core.Export;
using FormBuilder.Core.Providers.Collections;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Builders
{
    public class ExportCollectionBuilder :
      LazyCollectionBuilderBase<ExportCollectionBuilder, ExportCollection, ExportType>
    {
        protected override ExportCollectionBuilder This => this;
    }
}