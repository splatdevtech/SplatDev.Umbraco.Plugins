using FormBuilder.Core.Helpers;
using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

using Umbraco.Cms.Core.Extensions;

namespace FormBuilder.Core.Extensions
{
    public static class FormFileExtensions
    {
        public static bool IsFileTypeAllowed(
          this IFormFile file,
          Field field,
          string[] alwaysDisallowedExtensions,
          string[] onlyAllowedExtensions)
        {
            string fileExtension = file.FileName.Split('.').Last();
            if (onlyAllowedExtensions.Length != 0 && !ContainsExtension(onlyAllowedExtensions, fileExtension) || ContainsExtension(alwaysDisallowedExtensions, fileExtension))
                return false;
            if (field.AllowedUploadTypes is null)
                return true;
            IEnumerable<AllowedUploadType> source = field.AllowedUploadTypes.Where(p => !string.Equals("false", p.Checked, StringComparison.InvariantCultureIgnoreCase));
            return source.Any(x => x.Type.Equals(string.Empty, StringComparison.InvariantCultureIgnoreCase)) || source.Any(x => string.Compare(fileExtension, x.Type, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        private static bool ContainsExtension(string[] fileExtensionList, string fileExtension) => fileExtensionList.Contains("." + fileExtension.ToLowerInvariant());

        public static string SaveUploadedFileToTempFolder(
          this IFormFile file,
          IHostEnvironment hostEnvironment)
        {
            string fileName = Path.GetFileName(file.FileName);
            string path1 = "~/umbraco/Data/TEMP/FileUploads/" + Guid.NewGuid().ToString();
            string path2 = path1 + "/" + FileHelper.SafeUrl(fileName);
            Directory.CreateDirectory(hostEnvironment.MapPathContentRoot(path1));
            using (Stream target = new FileStream(hostEnvironment.MapPathContentRoot(path2), FileMode.Create))
            {
                file.CopyTo(target);
                target.Close();
            }
            return path2;
        }
    }
}