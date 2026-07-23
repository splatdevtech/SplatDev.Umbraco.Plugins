
// Type: Umbraco.Forms.Core.Providers.EmailTemplateCollectionBuilder
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.Composing;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Providers
{
  public class EmailTemplateCollectionBuilder : 
    LazyCollectionBuilderBase<EmailTemplateCollectionBuilder, EmailTemplateCollection, IEmailTemplate>
  {
    protected override EmailTemplateCollectionBuilder This => this;
  }
}
