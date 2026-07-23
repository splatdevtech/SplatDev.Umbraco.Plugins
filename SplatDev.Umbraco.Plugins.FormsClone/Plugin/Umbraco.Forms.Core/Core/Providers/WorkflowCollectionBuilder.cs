
// Type: Umbraco.Forms.Core.Providers.WorkflowCollectionBuilder
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;


#nullable enable
namespace Umbraco.Forms.Core.Providers
{
  public class WorkflowCollectionBuilder : 
    LazyCollectionBuilderBase<WorkflowCollectionBuilder, WorkflowCollection, WorkflowType>
  {
    protected override ServiceLifetime CollectionLifetime => ServiceLifetime.Transient;

    protected override WorkflowCollectionBuilder This => this;
  }
}
