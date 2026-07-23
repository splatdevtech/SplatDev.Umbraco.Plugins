
// Type: Umbraco.Forms.Web.Api.ManagementApi.Export.GetExportTypesController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Umbraco.Cms.Core.IO;
using Umbraco.Forms.Api.ManagementApi.Export;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Export
{
  public class GetExportTypesController : ExportControllerBase
  {
    private readonly ExportCollection _exportCollection;
    private readonly MediaFileManager _mediaFileManager;

    public GetExportTypesController(
      ExportCollection exportCollection,
      MediaFileManager mediaFileManager,
      IFormService formService)
      : base(formService)
    {
      this._exportCollection = exportCollection;
      this._mediaFileManager = mediaFileManager;
    }

    [HttpGet("types")]
    [ProducesResponseType(typeof (IEnumerable<ExportType>), 200)]
    public IActionResult GetExportTypes(Guid formId)
    {
      List<ExportType> list = this._exportCollection.ToList<ExportType>();
      ExportType excelExport1 = list.SingleOrDefault<ExportType>((Func<ExportType, bool>) (x => x.Id == Guid.Parse("94ED105A-87B3-4e1f-97CB-9A320AEE2745")));
      ExportType excelExport2 = list.SingleOrDefault<ExportType>((Func<ExportType, bool>) (x => x.Id == Guid.Parse("688711A2-DC6F-4B51-B8D2-0BB177BB0499")));
      ExportType downloadAllFilesInDiskStructureExport = list.SingleOrDefault<ExportType>((Func<ExportType, bool>) (x => x.Id == Guid.Parse("08479664-4FD9-4C7E-9504-77B764878E86")));
      ExportType downloadAllFilesByEntryExport = list.SingleOrDefault<ExportType>((Func<ExportType, bool>) (x => x.Id == Guid.Parse("fa7ae082-5c6a-4fdc-babd-162c9607b343")));
      if (this.HttpContext.Request.Headers.UserAgent.ToString().Contains("Intel Mac OS X"))
      {
        this.AdjustExportTypeDetailsForOsxRequest(excelExport1);
        this.AdjustExportTypeDetailsForOsxRequest(excelExport2);
      }
      if (downloadAllFilesInDiskStructureExport == null && downloadAllFilesByEntryExport == null)
        return (IActionResult) this.Ok((object) list);
            Umbraco.Forms.Core.Models.Form form = this.FormService.Get(formId);
      if (form == null)
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
        interpolatedStringHandler.AppendLiteral("Cannot find form with Id ");
        interpolatedStringHandler.AppendFormatted<Guid>(formId);
        throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
      }
      if (!form.AllFields.Any<Field>((Func<Field, bool>) (x => x.FieldTypeId == Guid.Parse("84A17CF8-B711-46A6-9840-0E4A072AD000"))))
      {
        GetExportTypesController.RemoveDownloadFilesExportProviders(list, downloadAllFilesInDiskStructureExport, downloadAllFilesByEntryExport);
        return (IActionResult) this.Ok((object) list);
      }
      string path1 = string.Format("{0}/{1}", (object) "forms/upload", (object) string.Format("form_{0}", (object) formId).ToLower(CultureInfo.InvariantCulture));
      if (!this._mediaFileManager.FileSystem.DirectoryExists(path1))
      {
        GetExportTypesController.RemoveDownloadFilesExportProviders(list, downloadAllFilesInDiskStructureExport, downloadAllFilesByEntryExport);
        return (IActionResult) this.Ok((object) list);
      }
      IEnumerable<string> directories = this._mediaFileManager.FileSystem.GetDirectories(path1);
      if (!directories.Any<string>())
      {
        GetExportTypesController.RemoveDownloadFilesExportProviders(list, downloadAllFilesInDiskStructureExport, downloadAllFilesByEntryExport);
        return (IActionResult) this.Ok((object) list);
      }
      bool flag = false;
      foreach (string path2 in directories)
      {
        if (this._mediaFileManager.FileSystem.GetFiles(path2).Any<string>())
        {
          flag = true;
          break;
        }
      }
      if (flag)
        return (IActionResult) this.Ok((object) list);
      GetExportTypesController.RemoveDownloadFilesExportProviders(list, downloadAllFilesInDiskStructureExport, downloadAllFilesByEntryExport);
      return (IActionResult) this.Ok((object) list);
    }

    private void AdjustExportTypeDetailsForOsxRequest(ExportType? excelExport)
    {
      if (excelExport == null)
        return;
      excelExport.IsOsx = true;
    }

    private static void RemoveDownloadFilesExportProviders(
      List<ExportType> allExportProviders,
      ExportType? downloadAllFilesInDiskStructureExport,
      ExportType? downloadAllFilesByEntryExport)
    {
      if (downloadAllFilesInDiskStructureExport != null)
        allExportProviders.Remove(downloadAllFilesInDiskStructureExport);
      if (downloadAllFilesByEntryExport == null)
        return;
      allExportProviders.Remove(downloadAllFilesByEntryExport);
    }
  }
}
