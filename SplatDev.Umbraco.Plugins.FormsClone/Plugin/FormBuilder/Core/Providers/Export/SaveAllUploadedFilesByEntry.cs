using FormBuilder.Core.Models;
using FormBuilder.Core.Searches;
using FormBuilder.Core.Searches.Interfaces;

using Microsoft.Extensions.Hosting;

using System.IO.Compression;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.IO;
using Umbraco.Extensions;

namespace FormBuilder.Core.Providers.Export
{
    /// <summary>
    /// Provides a an export type for exporting to a zip archive.
    /// </summary>
    public class SaveAllUploadedFilesByEntry : SaveAllUploadedFilesBase
    {
        private readonly IFormRecordSearcher _formRecordSearcher;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public SaveAllUploadedFilesByEntry(
          IHostEnvironment hostEnvironment,
          MediaFileManager mediaFileManager,
          IFormRecordSearcher formRecordSearcher)
          : base(hostEnvironment, mediaFileManager)
        {
            _formRecordSearcher = formRecordSearcher;
            Id = new Guid("fa7ae082-5c6a-4fdc-babd-162c9607b343");
            Name = "Save All Uploaded Files (by entry)";
            Alias = "saveAllUploadedFilesByEntry";
            Description = "Exports all files uploading in form submissions to a zip archive, organised in a sub-folder per entry.";
        }

        /// <inheritdoc />
        protected override void PopulateZipArchive(
          IFileSystem umbracoMediaFileSystem,
          Guid formId,
          RecordExportFilter filter,
          string folderPath,
          ZipArchive zip)
        {
            List<EntrySearchResult> list = [.. _formRecordSearcher.QueryDataBase(formId, filter).Results];
            foreach (string directory in umbracoMediaFileSystem.GetDirectories(folderPath))
            {
                if (TryGetMatchingRecordId(list, directory, out int matchingRecordId))
                {
                    foreach (string file in umbracoMediaFileSystem.GetFiles(directory))
                    {
                        string fileName = umbracoMediaFileSystem.GetFileName(file);
                        DefaultInterpolatedStringHandler interpolatedStringHandler = new(1, 2);
                        interpolatedStringHandler.AppendFormatted(matchingRecordId);
                        interpolatedStringHandler.AppendLiteral("/");
                        interpolatedStringHandler.AppendFormatted(fileName);
                        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                        AddZipEntryFromFile(umbracoMediaFileSystem, zip, file, stringAndClear);
                    }
                }
            }
        }

        private static bool TryGetMatchingRecordId(
          List<EntrySearchResult> submissions,
          string entryFolderName,
          out int matchingRecordId)
        {
            foreach (EntrySearchResult submission in submissions)
            {
                foreach (string? str in submission.Fields.Where(x => x.Value is not null).Select(x => x.Value?.ToString()).Where(x => x!.Contains("forms/upload")))
                {
                    if (str is not null && str.Contains(entryFolderName))
                    {
                        matchingRecordId = submission.Id;
                        return true;
                    }
                }
            }
            matchingRecordId = -1;
            return false;
        }
    }
}