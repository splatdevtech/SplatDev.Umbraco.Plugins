using FormBuilder.Core.Extensions;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace FormBuilder
{
    public class FormBuilderFormsComposer : IComposer, IDiscoverable
    {
        public void Compose(IUmbracoBuilder builder) => builder.AddFormBuilder();
    }
}