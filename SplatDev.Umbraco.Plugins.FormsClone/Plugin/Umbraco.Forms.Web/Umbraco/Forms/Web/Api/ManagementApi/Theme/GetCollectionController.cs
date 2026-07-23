
// Type: Umbraco.Forms.Web.Api.ManagementApi.Theme.GetCollectionController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.IO;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Theme
{
  public class GetCollectionController : ThemeControllerBase
  {
    private readonly IIOHelper _ioHelper;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ThemeCollection _themeCollection;

    public GetCollectionController(
      IIOHelper ioHelper,
      IHostEnvironment hostEnvironment,
      Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
      ILoggerFactory loggerFactory,
      ThemeCollection themeCollection)
    {
      this._ioHelper = ioHelper;
      this._hostEnvironment = hostEnvironment;
      this._hostingEnvironment = hostingEnvironment;
      this._loggerFactory = loggerFactory;
      this._themeCollection = themeCollection;
    }

    [HttpGet]
    [ProducesResponseType(typeof (IEnumerable<Umbraco.Forms.Web.Models.Backoffice.Theme>), 200)]
    public IActionResult GetAll()
    {
      List<string> list = this._themeCollection.Select<ITheme, string>((Func<ITheme, string>) (x => x.Name)).ToList<string>();
      IEnumerable<string> directories = this.CreateThemesFileSystem().GetDirectories(string.Empty);
      list.AddRange(directories);
      return (IActionResult) this.Ok((object) list.Distinct<string>().OrderBy<string, string>((Func<string, string>) (x => x)).Select<string, Umbraco.Forms.Web.Models.Backoffice.Theme>((Func<string, Umbraco.Forms.Web.Models.Backoffice.Theme>) (x => new Umbraco.Forms.Web.Models.Backoffice.Theme()
      {
        Name = x
      })).ToList<Umbraco.Forms.Web.Models.Backoffice.Theme>());
    }

    private PhysicalFileSystem CreateThemesFileSystem() => new PhysicalFileSystem(this._ioHelper, this._hostingEnvironment, this._loggerFactory.CreateLogger<PhysicalFileSystem>(), this._hostEnvironment.MapPathContentRoot("/Views/Partials/Forms/Themes"), "/Views/Partials/Forms/Themes");
  }
}
