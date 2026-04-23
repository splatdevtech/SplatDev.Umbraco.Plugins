using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace UmbracoCms.Plugins.CharLimit.Composers;

public class CharLimitComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // CharLimitDataEditor is auto-discovered by Umbraco via assembly scanning.
        // No explicit registration required.
    }
}
