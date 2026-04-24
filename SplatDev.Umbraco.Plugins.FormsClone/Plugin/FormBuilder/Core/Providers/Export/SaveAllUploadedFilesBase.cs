using FormBuilder.Core.Export;
using FormBuilder.Core.Models;

using Microsoft.Extensions.Hosting;

using System.Globalization;
using System.IO.Compression;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.IO;

namespace FormBuilder.Core.Providers.Export
{
    /// <summary>
    /// Base class providing a an export type for exporting to a zip archive.
    /// </summary>
    public abstract class SaveAllUploadedFilesBase : ExportType
    {
        private readonly MediaFileManager _mediaFileManager;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        protected SaveAllUploadedFilesBase(
          IHostEnvironment hostEnvironment,
          MediaFileManager mediaFileManager)
          : base(hostEnvironment)
        {
            _mediaFileManager = mediaFileManager;
            FileExtension = "zip";
            Icon = "icon-download";
        }

        /// <inheritdoc />
        public override Task<string> ExportRecordsAsync(Guid formId, RecordExportFilter filter) => throw new NotSupportedException();

        /// <inheritdoc />
        public override Task<string> ExportToFileAsync(
          Guid formId,
          RecordExportFilter filter,
          string filepath)
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(5, 1);
            interpolatedStringHandler.AppendLiteral("form_");
            interpolatedStringHandler.AppendFormatted(formId);
            string str1 = "forms/upload/" + interpolatedStringHandler.ToStringAndClear().ToLower(CultureInfo.InvariantCulture);
            string str2 = filepath;
            if (!filepath.Contains('\\'))
                str2 = HostEnvironment.MapPathContentRoot(filepath);
            string path = filepath[..filepath.LastIndexOf('\\')];
            if (!str2.EndsWith("." + FileExtension))
                str2 = str2 + "." + FileExtension;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (System.IO.File.Exists(str2))
                System.IO.File.Delete(str2);
            if (_mediaFileManager.FileSystem.DirectoryExists(str1))
            {
                using FileStream fileStream = new(str2, FileMode.Create);
                using ZipArchive zip = new(fileStream, ZipArchiveMode.Create);
                PopulateZipArchive(_mediaFileManager.FileSystem, formId, filter, str1, zip);
            }
            return Task.FromResult(str2);
        }

        /// <summary>Populates the contents of the zip archive.</summary>
        protected abstract void PopulateZipArchive(
          IFileSystem umbracoMediaFileSystem,
          Guid formId,
          RecordExportFilter filter,
          string folderPath,
          ZipArchive zip);

        /// <summary>Adds a zip entry from the provided file.</summary>
        protected static void AddZipEntryFromFile(
          IFileSystem umbracoMediaFileSystem,
          ZipArchive zip,
          string file,
          string zipEntryName)
        {
            using Stream stream = umbracoMediaFileSystem.OpenFile(file);
            using Stream destination = zip.CreateEntry(zipEntryName).Open();
            stream.CopyTo(destination);
        }
    }
}