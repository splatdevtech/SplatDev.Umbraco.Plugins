
// Type: Umbraco.Forms.Core.Data.PreValueTextFileStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Forms.Data.FileSystem;


#nullable enable
namespace Umbraco.Forms.Core.Data
{
  internal sealed class PreValueTextFileStorage : PreValueTextFileSystemStorage
  {
    public PreValueTextFileStorage(
      FormsFileSystemForSavedData fileSystem,
      IScopeProvider scopeProvider)
      : base((IFileSystem) fileSystem, scopeProvider, "PreValueTextFiles")
    {
    }
  }
}
