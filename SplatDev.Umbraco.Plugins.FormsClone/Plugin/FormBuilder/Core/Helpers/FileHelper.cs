using System.Text.RegularExpressions;

namespace FormBuilder.Core.Helpers
{
    public partial class FileHelper
    {
        public static string SafeUrl(string url) => string.IsNullOrEmpty(url) ? string.Empty : ValidFileName().Replace(url, "_");

        public static string MakeValidFileName(string name)
        {
            string pattern = string.Format("([{0}]*\\.+$)|([{0}]+)", Regex.Escape(new string(Path.GetInvalidFileNameChars())));
            return Regex.Replace(name, pattern, "_");
        }

        [GeneratedRegex("[^a-zA-Z0-9\\-\\.\\/\\:]{1}")]
        public static partial Regex ValidFileName();
    }
}