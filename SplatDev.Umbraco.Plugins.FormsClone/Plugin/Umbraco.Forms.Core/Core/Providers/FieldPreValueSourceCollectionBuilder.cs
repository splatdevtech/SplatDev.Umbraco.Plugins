
// Type: Umbraco.Forms.Core.Providers.FieldPreValueSourceCollectionBuilder
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;


#nullable enable
namespace Umbraco.Forms.Core.Providers
{
  public class FieldPreValueSourceCollectionBuilder : 
    LazyCollectionBuilderBase<FieldPreValueSourceCollectionBuilder, FieldPreValueSourceCollection, FieldPreValueSourceType>
  {
    protected override ServiceLifetime CollectionLifetime => ServiceLifetime.Transient;

    protected override FieldPreValueSourceCollectionBuilder This => this;
  }
}
