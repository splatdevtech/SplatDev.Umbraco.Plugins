using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Providers.Collections;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Builders
{
    public class ValidationPatternCollectionBuilder :
      OrderedCollectionBuilderBase<ValidationPatternCollectionBuilder, ValidationPatternCollection, IValidationPattern>
    {
        protected override ValidationPatternCollectionBuilder This => this;
    }
}