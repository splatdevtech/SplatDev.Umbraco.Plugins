using FormBuilder.Core.Interfaces;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Collections
{
    public class ParsedPlaceholderFormatterCollection(
      Func<IEnumerable<IParsedPlaceholderFormatter>> items) :
      BuilderCollectionBase<IParsedPlaceholderFormatter>(items)
    {
    }
}