
// Type: Umbraco.Forms.Core.Extensions.FormFileExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Umbraco.Cms.Core.Extensions;
using Umbraco.Forms.Core.Data.Helpers;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
  public static class FormFileExtensions
  {
    public static bool IsFileTypeAllowed(
      this IFormFile file,
      Field field,
      string[] alwaysDisallowedExtensions,
      string[] onlyAllowedExtensions)
    {
      string fileExtension = ((IEnumerable<string>) file.FileName.Split('.')).Last<string>();
      if (onlyAllowedExtensions.Length != 0 && !FormFileExtensions.ContainsExtension(onlyAllowedExtensions, fileExtension) || FormFileExtensions.ContainsExtension(alwaysDisallowedExtensions, fileExtension))
        return false;
      if (field.AllowedUploadTypes == null)
        return true;
      IEnumerable<AllowedUploadType> source = field.AllowedUploadTypes.Where<AllowedUploadType>((Func<AllowedUploadType, bool>) (p => !string.Equals("false", p.Checked, StringComparison.InvariantCultureIgnoreCase)));
      return source.Any<AllowedUploadType>((Func<AllowedUploadType, bool>) (x => x.Type.ToLowerInvariant() == string.Empty)) || source.Any<AllowedUploadType>((Func<AllowedUploadType, bool>) (x => string.Compare(fileExtension, x.Type, StringComparison.InvariantCultureIgnoreCase) == 0));
    }

    private static bool ContainsExtension(string[] fileExtensionList, string fileExtension) => ((IEnumerable<string>) fileExtensionList).Contains<string>("." + fileExtension.ToLowerInvariant());

    public static string SaveUploadedFileToTempFolder(
      this IFormFile file,
      IHostEnvironment hostEnvironment)
    {
      string fileName = Path.GetFileName(file.FileName);
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
      interpolatedStringHandler.AppendFormatted("~/umbraco/Data/TEMP/FileUploads");
      interpolatedStringHandler.AppendLiteral("/");
      interpolatedStringHandler.AppendFormatted<Guid>(Guid.NewGuid());
      interpolatedStringHandler.AppendLiteral("/");
      interpolatedStringHandler.AppendFormatted(FileHelper.SafeUrl(fileName));
      string stringAndClear = interpolatedStringHandler.ToStringAndClear();
      string path = hostEnvironment.MapPathContentRoot(stringAndClear);
      string directoryName = Path.GetDirectoryName(path);
      if (directoryName != null)
        Directory.CreateDirectory(directoryName);
      using (Stream target = (Stream) new FileStream(path, FileMode.Create))
        file.CopyTo(target);
      return stringAndClear;
    }
  }
}
