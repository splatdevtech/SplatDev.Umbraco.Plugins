
// Type: Umbraco.Forms.Data.FileSystem.FormsFileSystemForSavedData
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.IO;


#nullable enable
namespace Umbraco.Forms.Data.FileSystem
{
  public class FormsFileSystemForSavedData : FormsFileSystemWrapper
  {
    public FormsFileSystemForSavedData(IFileSystem inner)
      : base(inner)
    {
    }

    public bool IsDefaultFileSystem => FormsFileSystemForSavedData.IsDefault;

    internal static bool IsDefault { get; set; }
  }
}
