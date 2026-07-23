
// Type: Umbraco.Forms.Web.Extensions.StringExtensions
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;


#nullable enable
namespace Umbraco.Forms.Web.Extensions
{
  internal static class StringExtensions
  {
    internal static string ModifyFolderPathForStartFolders(
      this string folderPath,
      IList<Guid> startFolderKeys,
      string withPrefixedIds = "")
    {
      if (startFolderKeys == null || startFolderKeys.Count == 0)
        return folderPath;
      string[] strArray = folderPath.Split(',');
      foreach (Guid startFolderKey in (IEnumerable<Guid>) startFolderKeys)
      {
        int pathPartIndex = Array.IndexOf<string>(strArray, withPrefixedIds + startFolderKey.ToString());
        if (pathPartIndex != -1)
          return string.Join(",", ((IEnumerable<string>) strArray).Where<string>((Func<string, int, bool>) ((v, i) =>
          {
            if (i == 0)
              return true;
            return startFolderKeys.Count != 1 ? i >= pathPartIndex : i > pathPartIndex;
          })));
      }
      return folderPath;
    }

    internal static bool StartsWithNormalizedPath(
      this string path,
      string other,
      StringComparison comparisonType = StringComparison.Ordinal)
    {
      if (path.StartsWith(other, comparisonType))
        return true;
      string[] strArray1 = other.Split(Constants.CharArrays.ForwardSlash, StringSplitOptions.RemoveEmptyEntries);
      string[] strArray2 = path.Split(Constants.CharArrays.ForwardSlash, strArray1.Length + 1, StringSplitOptions.RemoveEmptyEntries);
      if (strArray1.Length > strArray2.Length)
        return false;
      for (int index = strArray1.Length - 1; index >= 0; --index)
      {
        if (!string.Equals(strArray1[index], strArray2[index], comparisonType))
          return false;
      }
      return true;
    }
  }
}
