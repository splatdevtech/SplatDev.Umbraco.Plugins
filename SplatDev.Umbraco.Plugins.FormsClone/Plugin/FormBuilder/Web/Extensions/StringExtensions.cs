using FormBuilder.Core;

namespace FormBuilder.Web.Extensions
{
    internal static class StringExtensions
    {
        internal static string ModifyFolderPathForStartFolders(
          this string folderPath,
          IList<Guid> startFolderKeys,
          string withPrefixedIds = "")
        {
            if (startFolderKeys is null || startFolderKeys.Count == 0)
                return folderPath;
            string[] strArray = folderPath.Split(',');
            foreach (Guid startFolderKey in (IEnumerable<Guid>)startFolderKeys)
            {
                int pathPartIndex = Array.IndexOf(strArray, withPrefixedIds + startFolderKey.ToString());
                if (pathPartIndex != -1)
                    return string.Join(",", strArray.Where((v, i) =>
                    {
                        if (i == 0)
                            return true;
                        return startFolderKeys.Count != 1 ? i >= pathPartIndex : i > pathPartIndex;
                    }));
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