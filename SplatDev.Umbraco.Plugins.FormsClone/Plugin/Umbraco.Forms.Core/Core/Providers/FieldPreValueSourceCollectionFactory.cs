
// Type: Umbraco.Forms.Core.Providers.FieldPreValueSourceCollectionFactory
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.DependencyInjection;
using System;


#nullable enable
namespace Umbraco.Forms.Core.Providers
{
  public class FieldPreValueSourceCollectionFactory
  {
    private readonly IServiceProvider _serviceProvider;

    public FieldPreValueSourceCollectionFactory(IServiceProvider serviceProvider) => this._serviceProvider = serviceProvider;

    public FieldPreValueSourceCollection GetPreValueSourceCollection() => ServiceProviderServiceExtensions.GetRequiredService<FieldPreValueSourceCollection>(this._serviceProvider);
  }
}
