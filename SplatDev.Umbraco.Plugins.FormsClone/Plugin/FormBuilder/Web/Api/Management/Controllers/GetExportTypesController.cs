using FormBuilder.Core.Export;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

using System.Globalization;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.IO;

namespace FormBuilder.Web.Api.Management.Controllers
{
    public class GetExportTypesController(
      ExportCollection exportCollection,
      MediaFileManager mediaFileManager,
      IFormService formService) : ExportControllerBase(formService)
    {
        private readonly ExportCollection _exportCollection = exportCollection;
        private readonly MediaFileManager _mediaFileManager = mediaFileManager;

        /// <summary>
        /// Retrieves all available         /// </summary>
        [HttpGet("types")]
        [ProducesResponseType(typeof(IEnumerable<ExportType>), 200)]
        public IActionResult GetExportTypes(Guid formId)
        {
            List<ExportType> list = [.. _exportCollection];
            ExportType? excelExport1 = list.SingleOrDefault(x => x.Id == Guid.Parse("94ED105A-87B3-4e1f-97CB-9A320AEE2745"));
            ExportType? excelExport2 = list.SingleOrDefault(x => x.Id == Guid.Parse("688711A2-DC6F-4B51-B8D2-0BB177BB0499"));
            ExportType? downloadAllFilesInDiskStructureExport = list.SingleOrDefault(x => x.Id == Guid.Parse("08479664-4FD9-4C7E-9504-77B764878E86"));
            ExportType? downloadAllFilesByEntryExport = list.SingleOrDefault(x => x.Id == Guid.Parse("fa7ae082-5c6a-4fdc-babd-162c9607b343"));
            if (HttpContext.Request.Headers.UserAgent.ToString().Contains("Intel Mac OS X"))
            {
                AdjustExportTypeDetailsForOsxRequest(excelExport1);
                AdjustExportTypeDetailsForOsxRequest(excelExport2);
            }
            if (downloadAllFilesInDiskStructureExport is null && downloadAllFilesByEntryExport is null)
                return Ok(list);
            Form? form = FormService.Get(formId);
            if (form is null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(25, 1);
                interpolatedStringHandler.AppendLiteral("Cannot find form with Id ");
                interpolatedStringHandler.AppendFormatted(formId);
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            if (!form.AllFields.Any(x => x.FieldTypeId == Guid.Parse("84A17CF8-B711-46A6-9840-0E4A072AD000")))
            {
                RemoveDownloadFilesExportProviders(list, downloadAllFilesInDiskStructureExport, downloadAllFilesByEntryExport);
                return Ok(list);
            }
            string path1 = string.Format("{0}/{1}", "forms/upload", string.Format("form_{0}", formId).ToLower(CultureInfo.InvariantCulture));
            if (!_mediaFileManager.FileSystem.DirectoryExists(path1))
            {
                RemoveDownloadFilesExportProviders(list, downloadAllFilesInDiskStructureExport, downloadAllFilesByEntryExport);
                return Ok(list);
            }
            IEnumerable<string> directories = _mediaFileManager.FileSystem.GetDirectories(path1);
            if (!directories.Any())
            {
                RemoveDownloadFilesExportProviders(list, downloadAllFilesInDiskStructureExport, downloadAllFilesByEntryExport);
                return Ok(list);
            }
            bool flag = false;
            foreach (string path2 in directories)
            {
                if (_mediaFileManager.FileSystem.GetFiles(path2).Any())
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
                return Ok(list);
            RemoveDownloadFilesExportProviders(list, downloadAllFilesInDiskStructureExport, downloadAllFilesByEntryExport);
            return Ok(list);
        }

        /// <summary>
        /// Set the indicator for OSx to allow client-side localization
        /// </summary>
        /// <param name="excelExport"></param>
        private static void AdjustExportTypeDetailsForOsxRequest(ExportType? excelExport)
        {
            if (excelExport is null)
                return;
            excelExport.IsOsx = true;
        }

        private static void RemoveDownloadFilesExportProviders(
          List<ExportType> allExportProviders,
          ExportType? downloadAllFilesInDiskStructureExport,
          ExportType? downloadAllFilesByEntryExport)
        {
            if (downloadAllFilesInDiskStructureExport is not null)
                allExportProviders.Remove(downloadAllFilesInDiskStructureExport);
            if (downloadAllFilesByEntryExport is null)
                return;
            allExportProviders.Remove(downloadAllFilesByEntryExport);
        }
    }
}