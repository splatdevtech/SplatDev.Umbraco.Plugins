using FormBuilder.Core.Interfaces;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Collections
{
    public class ValidationPatternCollection(Func<IEnumerable<IValidationPattern>> items) : BuilderCollectionBase<IValidationPattern>(items)
    {
    }
}