using FormBuilder.Core.Export;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

using Umbraco.Cms.Core.Extensions;

namespace FormBuilder.Web.Api.Management.Controllers
{
    public class ExportController(IFormService formService, IHostEnvironment hostEnvironment) : ExportControllerBase(formService)
    {
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

        /// <summary>
        /// This is used to stream a specific file form /umbraco/Data/TEMP/FileUploads/form-exports/formGUID/formName.xls
        /// </summary>
        /// <param name="formId">The form Id</param>
        /// <param name="fileName">The filename to request formname.xls</param>
        [HttpGet]
        [ProducesResponseType(typeof(PhysicalFileResult), 200)]
        public IActionResult GetExport(string formId, string fileName)
        {
            if (!Guid.TryParse(formId, out Guid result))
                return BadRequest("Invalid Id");
            if (FormService.Get(result) is null)
                return NotFound("Form ID not found");
            if (!Directory.Exists(_hostEnvironment.MapPathContentRoot(Path.Combine("~/umbraco/Data/TEMP/FileUploads", "form-exports", formId))))
                return NotFound("Form export not found");
            if (fileName.Any(new Func<char, bool>(Path.GetInvalidFileNameChars().Contains)))
                return BadRequest("File name contains invalid characters");
            string path = Path.Combine("~/umbraco/Data/TEMP/FileUploads", "form-exports", formId, fileName);
            string str1 = Path.Combine("~/umbraco/Data/TEMP/FileUploads", "form-exports", formId);
            if (!path.StartsWith(str1, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Unexpected folder");
            string str2 = _hostEnvironment.MapPathContentRoot(path);
            return PhysicalFile(str2, "application/octet-stream", Path.GetFileName(str2));
        }
    }
}