
// Type: Umbraco.Forms.Web.Api.ManagementApi.Export.PostGenerateExportController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using Umbraco.Forms.Api.ManagementApi.Export;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Export
{
    public class PostGenerateExportController(
  IFormService formService,
  IShortStringHelper shortStringHelper,
  ExportCollection exportCollection,
  IHostEnvironment hostEnvironment) : ExportControllerBase(formService)
    {
        private readonly IShortStringHelper _shortStringHelper = shortStringHelper;
        private readonly ExportCollection _exportCollection = exportCollection;
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

        /// <summary>
        /// When selecting an Export Type - we generate the file on disk to be exported
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(GenerateExportResponseModel), 200)]
        public IActionResult PostGenerateExport(Guid formId, RecordExportFilter model)
        {
            string empty1 = string.Empty;
            string empty2 = string.Empty;
            if (string.IsNullOrEmpty(model.ExportType))
                return Ok(new GenerateExportResponseModel()
                {
                    FileName = empty1,
                    FormId = formId.ToString(),
                    Path = empty2
                });
            Guid id = Guid.Parse(model.ExportType);
            model.Take = int.MaxValue;
            model.Skip = 0;
            ExportType export = _exportCollection[id];
            Umbraco.Forms.Core.Models.Form? form = FormService.Get(formId);
            if (form is null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(25, 1);
                interpolatedStringHandler.AppendLiteral("Cannot find form with Id ");
                interpolatedStringHandler.AppendFormatted(formId);
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            string path4 = string.Format("{0}.{1}", form.Name.Replace(" ", "_").ToSafeFileName(_shortStringHelper), export.FileExtension);
            string filepath = _hostEnvironment.MapPathContentRoot(Path.Combine("~/umbraco/Data/TEMP/FileUploads", "form-exports", formId.ToString(), path4));
            Task.FromResult(export.ExportToFileAsync(formId, model, filepath));
            return Ok(new GenerateExportResponseModel()
            {
                FileName = path4,
                FormId = formId.ToString(),
                Path = filepath
            });
        }

        public record GenerateExportResponseModel
        {
            [Required]
            public string FileName { get; init; } = string.Empty;

            [Required]
            public string FormId { get; init; } = string.Empty;

            [Required]
            public string Path { get; init; } = string.Empty;

            public GenerateExportResponseModel()
            {
            }
        }
    }
}
