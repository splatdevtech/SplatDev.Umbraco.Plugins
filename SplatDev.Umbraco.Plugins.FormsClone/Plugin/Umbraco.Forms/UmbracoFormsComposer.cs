
// Type: Umbraco.Forms.UmbracoFormsComposer
// Assembly: Umbraco.Forms, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 35E95609-BA13-4414-B9E2-6820FE57987F

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;


#nullable enable
namespace Umbraco.Forms
{
  public class UmbracoFormsComposer : IComposer, IDiscoverable
  {
    public void Compose(IUmbracoBuilder builder) => builder.AddUmbracoForms();
  }
}
