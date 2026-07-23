
// Type: Umbraco.Extensions.FormsUmbracoBuilderExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.DependencyInjection;
using System;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.IO;
using Umbraco.Forms.Data.FileSystem;


#nullable enable
namespace Umbraco.Extensions
{
  public static class FormsUmbracoBuilderExtensions
  {
    public static void SetFormsSavedDataFileSystem(
      this IUmbracoBuilder builder,
      Func<IServiceProvider, IFileSystem> filesystemFactory)
    {
      builder.Services.AddUnique<FormsFileSystemForSavedData>((Func<IServiceProvider, FormsFileSystemForSavedData>) (provider =>
      {
        IFileSystem filesystem = filesystemFactory(provider);
        IFileSystem shadowWrapper = ServiceProviderServiceExtensions.GetRequiredService<FileSystems>(provider).CreateShadowWrapper(filesystem, "FormsFileSystemForSavedData");
        return provider.CreateInstance<FormsFileSystemForSavedData>((object) shadowWrapper);
      }));
    }
  }
}
