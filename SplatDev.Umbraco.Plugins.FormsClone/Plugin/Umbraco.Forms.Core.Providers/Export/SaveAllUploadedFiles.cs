
// Type: Umbraco.Forms.Core.Providers.Export.SaveAllUploadedFiles
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Hosting;
using System;
using System.IO.Compression;
using Umbraco.Cms.Core.IO;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Providers.Export
{
  public class SaveAllUploadedFiles : SaveAllUploadedFilesBase
  {
    public SaveAllUploadedFiles(IHostEnvironment hostEnvironment, MediaFileManager mediaFileManager)
      : base(hostEnvironment, mediaFileManager)
    {
      this.Id = new Guid("08479664-4FD9-4C7E-9504-77B764878E86");
      this.Name = "Save All Uploaded Files (in disk structure)";
      this.Alias = "saveAllUploadedFilesInDiskStructure";
      this.Description = "Exports all files uploading in form submissions to a zip archive, organised as the files are stored on disk.";
    }

    protected override void PopulateZipArchive(
      IFileSystem umbracoMediaFileSystem,
      Guid formId,
      RecordExportFilter filter,
      string folderPath,
      ZipArchive zip)
    {
      foreach (string directory in umbracoMediaFileSystem.GetDirectories(folderPath))
      {
        zip.CreateEntry(directory + "/");
        foreach (string file in umbracoMediaFileSystem.GetFiles(directory))
        {
          string fileName = umbracoMediaFileSystem.GetFileName(file);
          string zipEntryName = directory + "/" + fileName;
          SaveAllUploadedFilesBase.AddZipEntryFromFile(umbracoMediaFileSystem, zip, file, zipEntryName);
        }
      }
    }
  }
}
