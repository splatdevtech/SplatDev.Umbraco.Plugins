
// Type: Umbraco.Forms.Web.Api.ManagementApi.Export.ExportController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using Umbraco.Cms.Core.Extensions;
using Umbraco.Forms.Api.ManagementApi.Export;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Export
{
  public class ExportController : ExportControllerBase
  {
    private readonly IHostEnvironment _hostEnvironment;

    public ExportController(IFormService formService, IHostEnvironment hostEnvironment)
      : base(formService)
    {
      this._hostEnvironment = hostEnvironment;
    }

    [HttpGet]
    [ProducesResponseType(typeof (PhysicalFileResult), 200)]
    public IActionResult GetExport(string formId, string fileName)
    {
      Guid result;
      if (!Guid.TryParse(formId, out result))
        return (IActionResult) this.BadRequest((object) "Invalid Id");
      if (this.FormService.Get(result) == null)
        return (IActionResult) this.NotFound((object) "Form ID not found");
      if (!Directory.Exists(this._hostEnvironment.MapPathContentRoot(Path.Combine("~/umbraco/Data/TEMP/FileUploads", "form-exports", formId))))
        return (IActionResult) this.NotFound((object) "Form export not found");
            if (fileName.Any(new Func<char, bool>((Path.GetInvalidFileNameChars()).Contains<char>)))
        return (IActionResult) this.BadRequest((object) "File name contains invalid characters");
      string path = Path.Combine("~/umbraco/Data/TEMP/FileUploads", "form-exports", formId, fileName);
      string str1 = Path.Combine("~/umbraco/Data/TEMP/FileUploads", "form-exports", formId);
      if (!path.StartsWith(str1, StringComparison.OrdinalIgnoreCase))
        return (IActionResult) this.BadRequest((object) "Unexpected folder");
      string str2 = this._hostEnvironment.MapPathContentRoot(path);
      return (IActionResult) this.PhysicalFile(str2, "application/octet-stream", Path.GetFileName(str2));
    }
  }
}
