using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Providers.Collections;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Builders
{
    public class EmailTemplateCollectionBuilder :
      LazyCollectionBuilderBase<EmailTemplateCollectionBuilder, EmailTemplateCollection, IEmailTemplate>
    {
        protected override EmailTemplateCollectionBuilder This => this;
    }
}