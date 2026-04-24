using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Providers.Collections;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Builders
{
    public class ThemeCollectionBuilder :
      LazyCollectionBuilderBase<ThemeCollectionBuilder, ThemeCollection, ITheme>
    {
        protected override ThemeCollectionBuilder This => this;
    }
}