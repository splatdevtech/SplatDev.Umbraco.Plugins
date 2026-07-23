using Umbraco.Cms.Core.IO;

namespace FormBuilder.Core.FileSystem
{
    public class FormsFileSystemForSavedData(IFileSystem inner) : FormsFileSystemWrapper(inner)
    {
        public static bool IsDefaultFileSystem => IsDefault;

        internal static bool IsDefault { get; set; }
    }
}