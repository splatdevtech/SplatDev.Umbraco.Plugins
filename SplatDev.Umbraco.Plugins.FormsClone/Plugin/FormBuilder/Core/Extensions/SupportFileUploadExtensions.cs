using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;

namespace FormBuilder.Core.Extensions
{
    internal static class SupportFileUploadExtensions
    {
        public static void SetFileUploadOptions(this ISupportFileUploads model, Field field)
        {
            model.AllowMultipleFileUploads = field.AllowMultipleFileUploads;
            if (field.AllowedUploadTypes is null)
                return;
            bool flag = field.AllowedUploadTypes.SingleOrDefault(x => string.IsNullOrEmpty(x.Type) && !string.Equals("false", x.Checked, StringComparison.InvariantCultureIgnoreCase)) is not null;
            model.AllowAllUploadExtensions = flag;
            if (flag)
                return;
            model.AllowedUploadExtensions = [.. field.AllowedUploadTypes.Where(x => !string.IsNullOrEmpty(x.Type) && !string.Equals("false", x.Checked, StringComparison.OrdinalIgnoreCase)).Select(x => x.Type.ToLowerInvariant())];
        }
    }
}