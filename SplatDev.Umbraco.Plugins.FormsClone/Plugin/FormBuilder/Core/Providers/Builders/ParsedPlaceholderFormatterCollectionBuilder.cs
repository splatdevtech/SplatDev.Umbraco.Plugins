using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Providers.Collections;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Builders
{
    public class ParsedPlaceholderFormatterCollectionBuilder :
      LazyCollectionBuilderBase<ParsedPlaceholderFormatterCollectionBuilder, ParsedPlaceholderFormatterCollection, IParsedPlaceholderFormatter>
    {
        protected override ParsedPlaceholderFormatterCollectionBuilder This => this;
    }
}