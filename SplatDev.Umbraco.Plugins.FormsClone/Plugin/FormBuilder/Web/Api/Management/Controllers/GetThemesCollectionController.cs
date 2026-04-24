using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.IO;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for retrieving all themes.</summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="T:FormBuilder.Web.Api.ManagementApi.Theme.GetCollectionController" /> class.
    /// </remarks>
    public class GetThemesCollectionController(
      IIOHelper ioHelper,
      IHostEnvironment hostEnvironment,
      Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
      ILoggerFactory loggerFactory,
      ThemeCollection themeCollection) : ThemeControllerBase
    {
        private readonly IIOHelper _ioHelper = ioHelper;
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment = hostingEnvironment;
        private readonly ILoggerFactory _loggerFactory = loggerFactory;
        private readonly ThemeCollection _themeCollection = themeCollection;

        /// <summary>Management API endpoint for retrieving all themes.</summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Theme>), 200)]
        public IActionResult GetAll()
        {
            List<string> list = [.. _themeCollection.Select(x => x.Name)];
            IEnumerable<string> directories = CreateThemesFileSystem().GetDirectories(string.Empty);
            list.AddRange(directories);
            return Ok(list.Distinct().OrderBy(x => x).Select(x => new Theme()
            {
                Name = x
            }).ToList());
        }

        private PhysicalFileSystem CreateThemesFileSystem() => new(_ioHelper, _hostingEnvironment, _loggerFactory.CreateLogger<PhysicalFileSystem>(), _hostEnvironment.MapPathContentRoot("/Views/Partials/Forms/Themes"), "/Views/Partials/Forms/Themes");
    }
}