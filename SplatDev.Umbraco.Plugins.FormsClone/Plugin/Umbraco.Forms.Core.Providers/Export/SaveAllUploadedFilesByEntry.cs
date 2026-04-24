
// Type: Umbraco.Forms.Core.Providers.Export.SaveAllUploadedFilesByEntry
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using Umbraco.Cms.Core.IO;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Searchers;


#nullable enable
namespace Umbraco.Forms.Core.Providers.Export
{
  public class SaveAllUploadedFilesByEntry : SaveAllUploadedFilesBase
  {
    private readonly IFormRecordSearcher _formRecordSearcher;

    public SaveAllUploadedFilesByEntry(
      IHostEnvironment hostEnvironment,
      MediaFileManager mediaFileManager,
      IFormRecordSearcher formRecordSearcher)
      : base(hostEnvironment, mediaFileManager)
    {
      this._formRecordSearcher = formRecordSearcher;
      this.Id = new Guid("fa7ae082-5c6a-4fdc-babd-162c9607b343");
      this.Name = "Save All Uploaded Files (by entry)";
      this.Alias = "saveAllUploadedFilesByEntry";
      this.Description = "Exports all files uploading in form submissions to a zip archive, organised in a sub-folder per entry.";
    }

    protected override void PopulateZipArchive(
      IFileSystem umbracoMediaFileSystem,
      Guid formId,
      RecordExportFilter filter,
      string folderPath,
      ZipArchive zip)
    {
      List<EntrySearchResult> list = this._formRecordSearcher.QueryDataBase(formId, (RecordFilter) filter).Results.ToList<EntrySearchResult>();
      foreach (string directory in umbracoMediaFileSystem.GetDirectories(folderPath))
      {
        int matchingRecordId;
        if (SaveAllUploadedFilesByEntry.TryGetMatchingRecordId(list, directory, out matchingRecordId))
        {
          foreach (string file in umbracoMediaFileSystem.GetFiles(directory))
          {
            string fileName = umbracoMediaFileSystem.GetFileName(file);
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
            interpolatedStringHandler.AppendFormatted<int>(matchingRecordId);
            interpolatedStringHandler.AppendLiteral("/");
            interpolatedStringHandler.AppendFormatted(fileName);
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            SaveAllUploadedFilesBase.AddZipEntryFromFile(umbracoMediaFileSystem, zip, file, stringAndClear);
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
        foreach (string str in submission.Fields.Where<EntrySearchResult.FieldData>((Func<EntrySearchResult.FieldData, bool>) (x => x.Value != null)).Select<EntrySearchResult.FieldData, string>((Func<EntrySearchResult.FieldData, string>) (x => x.Value.ToString())).Where<string>((Func<string, bool>) (x => x.Contains("forms/upload"))))
        {
          if (str != null && str.Contains(entryFolderName))
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
