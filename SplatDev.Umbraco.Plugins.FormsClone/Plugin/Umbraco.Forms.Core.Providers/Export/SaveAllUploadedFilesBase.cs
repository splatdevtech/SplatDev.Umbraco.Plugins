
// Type: Umbraco.Forms.Core.Providers.Export.SaveAllUploadedFilesBase
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Umbraco.Cms.Core.IO;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Providers.Export
{
  public abstract class SaveAllUploadedFilesBase : ExportType
  {
    private readonly MediaFileManager _mediaFileManager;

    protected SaveAllUploadedFilesBase(
      IHostEnvironment hostEnvironment,
      MediaFileManager mediaFileManager)
      : base(hostEnvironment)
    {
      this._mediaFileManager = mediaFileManager;
      this.FileExtension = "zip";
      this.Icon = "icon-download";
    }

    public override Task<string> ExportRecordsAsync(Guid formId, RecordExportFilter filter) => throw new NotSupportedException();

    protected override Task ExportToSteamAsync(
      Guid formId,
      RecordExportFilter filter,
      Stream stream)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 1);
      interpolatedStringHandler.AppendLiteral("form_");
      interpolatedStringHandler.AppendFormatted<Guid>(formId);
      string str = "forms/upload/" + interpolatedStringHandler.ToStringAndClear().ToLowerInvariant();
      if (this._mediaFileManager.FileSystem.DirectoryExists(str))
      {
        using (ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Create))
          this.PopulateZipArchive(this._mediaFileManager.FileSystem, formId, filter, str, zip);
      }
      return Task.CompletedTask;
    }

    protected abstract void PopulateZipArchive(
      IFileSystem umbracoMediaFileSystem,
      Guid formId,
      RecordExportFilter filter,
      string folderPath,
      ZipArchive zip);

    protected static void AddZipEntryFromFile(
      IFileSystem umbracoMediaFileSystem,
      ZipArchive zip,
      string file,
      string zipEntryName)
    {
      using (Stream stream = umbracoMediaFileSystem.OpenFile(file))
      {
        using (Stream destination = zip.CreateEntry(zipEntryName).Open())
          stream.CopyTo(destination);
      }
    }
  }
}
