using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Web.Common.Controllers;

namespace SplatDev.Umbraco.Plugins.PdfCurator.Controllers;

public class UploadApiController : UmbracoApiController
{
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger _logger;
    private readonly string _contentRootPath;
    private string _path;

    public UploadApiController(IWebHostEnvironment webHostEnvironment, IConfiguration config, ILogger<UploadApiController> logger)
    {
        _webHostEnvironment = webHostEnvironment;
        _config = config;
        _path = _config.GetValue(Constants.Imports.IMPORT_APP_SETTING, Constants.Imports.DEFAULT_IMPORT_FOLDER) ?? Constants.Imports.DEFAULT_IMPORT_FOLDER;
        _contentRootPath = _webHostEnvironment.ContentRootPath;
        _logger = logger;
    }

    [HttpPost, DisableRequestSizeLimit]
    public async Task UploadFileAsync(IFormFile file)
    {
        try
        {
            _path = Path.Combine(_contentRootPath, _path);

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            var filename = Path.Combine(_path, file.FileName);
            using Stream fileStream = new FileStream(filename, FileMode.Create);
            if (file is not null) await file.CopyToAsync(fileStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Uploading file {name}", file.Name);
        }
    }

    [HttpPost, DisableRequestSizeLimit]
    public async Task<ActionResult> UploadFiles()
    {
        var request = HttpContext.Request;
        try
        {
            foreach (var file in request.Form.Files)
                await UploadFileAsync(file);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Uploading files");
            return BadRequest();
        }
    }
}
