
// Type: Umbraco.Forms.Web.Api.ManagementApi.AcceptanceTests.GetSystemInfoController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Web.Models.ManagementApi.AcceptanceTests;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.AcceptanceTests
{
  public class GetSystemInfoController : AcceptanceTestsControllerBase
  {
    protected readonly IUmbracoDatabaseFactory _databaseFactory;

    public GetSystemInfoController(IUmbracoDatabaseFactory databaseFactory) => this._databaseFactory = databaseFactory;

    [HttpGet("system-info")]
    [ProducesResponseType(typeof (SystemInfo), 200)]
    public IActionResult GetSystemInfo()
    {
      bool flag = this._databaseFactory.SqlContext.IsSqlite();
      return (IActionResult) this.Ok((object) new SystemInfo()
      {
        IsWindows = OperatingSystem.IsWindows(),
        IsLinux = OperatingSystem.IsLinux(),
        IsMacOS = OperatingSystem.IsMacOS(),
        IsSqlServer = !flag,
        IsSqlite = flag
      });
    }
  }
}
