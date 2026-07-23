
// Type: Umbraco.Forms.Web.Api.ManagementApi.System.SystemInfoController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;

using System.Text.Json.Serialization;

using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Web.Attributes;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.SystemInformation
{
    [AuthorizeForAcceptanceTests]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/umbraco/forms/management/api/v1/systeminfo")]
    public class SystemInfoController : FormsManagementApiControllerBase
    {
        private readonly IUmbracoDatabaseFactory _databaseFactory;

        public SystemInfoController(IUmbracoDatabaseFactory databaseFactory) => this._databaseFactory = databaseFactory;

        [HttpGet]
        public IActionResult GetSystemInfo()
        {
            bool flag = this._databaseFactory.SqlContext.IsSqlite();
            return this.Ok(new SystemInfoController.SystemInfo()
            {
                IsWindows = OperatingSystem.IsWindows(),
                IsLinux = OperatingSystem.IsLinux(),
                IsMacOS = OperatingSystem.IsMacOS(),
                IsSqlServer = !flag,
                IsSqlite = flag
            });
        }

        public class SystemInfo
        {
            [JsonPropertyName("isWindows")]
            public bool IsWindows { get; set; }

            [JsonPropertyName("isLinux")]
            public bool IsLinux { get; set; }

            [JsonPropertyName("isMacOS")]
            public bool IsMacOS { get; set; }

            [JsonPropertyName("isSqlServer")]
            public bool IsSqlServer { get; set; }

            [JsonPropertyName("isSqlite")]
            public bool IsSqlite { get; set; }
        }
    }
}
