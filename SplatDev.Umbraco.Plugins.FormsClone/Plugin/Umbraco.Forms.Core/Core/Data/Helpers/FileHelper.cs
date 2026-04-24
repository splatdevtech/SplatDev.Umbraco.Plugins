
// Type: Umbraco.Forms.Core.Data.Helpers.FileHelper
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.IO;
using System.Text.RegularExpressions;


#nullable enable
namespace Umbraco.Forms.Core.Data.Helpers
{
  public class FileHelper
  {
    public static string SafeUrl(string url) => string.IsNullOrEmpty(url) ? string.Empty : Regex.Replace(url, "[^a-zA-Z0-9\\-\\.\\/\\:]{1}", "_");

    public static string MakeValidFileName(string name)
    {
      string pattern = string.Format("([{0}]*\\.+$)|([{0}]+)", (object) Regex.Escape(new string(Path.GetInvalidFileNameChars())));
      return Regex.Replace(name, pattern, "_");
    }
  }
}
