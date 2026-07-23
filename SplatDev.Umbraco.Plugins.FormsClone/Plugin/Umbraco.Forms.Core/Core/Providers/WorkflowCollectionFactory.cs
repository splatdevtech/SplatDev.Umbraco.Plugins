
// Type: Umbraco.Forms.Core.Providers.WorkflowCollectionFactory
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.DependencyInjection;
using System;


#nullable enable
namespace Umbraco.Forms.Core.Providers
{
  public class WorkflowCollectionFactory
  {
    private readonly IServiceProvider _serviceProvider;

    public WorkflowCollectionFactory(IServiceProvider serviceProvider) => this._serviceProvider = serviceProvider;

    public WorkflowCollection GetWorkflowCollection() => ServiceProviderServiceExtensions.GetRequiredService<WorkflowCollection>(this._serviceProvider);
  }
}
