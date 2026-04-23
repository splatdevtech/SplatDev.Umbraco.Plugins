using FormBuilder.Core.Models;

using Microsoft.Extensions.Hosting;

using System.IO.Compression;

using Umbraco.Cms.Core.IO;
using Umbraco.Extensions;

namespace FormBuilder.Core.Providers.Export
{
    /// <summary>
    /// Provides a an export type for exporting to a zip archive.
    /// </summary>
    public class SaveAllUploadedFiles : SaveAllUploadedFilesBase
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public SaveAllUploadedFiles(IHostEnvironment hostEnvironment, MediaFileManager mediaFileManager)
          : base(hostEnvironment, mediaFileManager)
        {
            Id = new Guid("08479664-4FD9-4C7E-9504-77B764878E86");
            Name = "Save All Uploaded Files (in disk structure)";
            Alias = "saveAllUploadedFilesInDiskStructure";
            Description = "Exports all files uploading in form submissions to a zip archive, organised as the files are stored on disk.";
        }

        /// <inheritdoc />
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
                    AddZipEntryFromFile(umbracoMediaFileSystem, zip, file, zipEntryName);
                }
            }
        }
    }
}