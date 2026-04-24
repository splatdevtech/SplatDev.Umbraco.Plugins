using FormBuilder.Core.Interfaces;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Collections
{
    public class ThemeCollection(Func<IEnumerable<ITheme>> items) : BuilderCollectionBase<ITheme>(items)
    {
    }
}