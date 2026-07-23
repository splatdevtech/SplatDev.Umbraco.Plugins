
// Type: Umbraco.Forms.Core.Extensions.SupportFileUploadExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
    public static class SupportFileUploadExtensions
    {
        public static void SetFileUploadOptions(this ISupportFileUploads model, Field field)
        {
            model.AllowMultipleFileUploads = field.AllowMultipleFileUploads;
            if (field.AllowedUploadTypes == null)
                return;
            bool flag = field.AllowedUploadTypes.SingleOrDefault<AllowedUploadType>(x => string.IsNullOrEmpty(x.Type) && !string.Equals("false", x.Checked, StringComparison.InvariantCultureIgnoreCase)) != null;
            model.AllowAllUploadExtensions = flag;
            if (flag)
                return;
            model.AllowedUploadExtensions = field.AllowedUploadTypes.Where<AllowedUploadType>(x => !string.IsNullOrEmpty(x.Type) && !string.Equals("false", x.Checked, StringComparison.InvariantCultureIgnoreCase)).Select<AllowedUploadType, string>(x => x.Type.ToLowerInvariant()).ToList<string>();
        }
    }
}
