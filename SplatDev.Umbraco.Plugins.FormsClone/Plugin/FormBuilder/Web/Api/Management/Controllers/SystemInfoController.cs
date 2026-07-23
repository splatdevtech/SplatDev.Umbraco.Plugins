using FormBuilder.Core.Extensions;
using FormBuilder.Web.Attributes;

using Microsoft.AspNetCore.Mvc;

using System.Text.Json.Serialization;

using Umbraco.Cms.Infrastructure.Persistence;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Exposes system information for acceptance tests in order to customize behaviour when running in different environments.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [AuthorizeForAcceptanceTests]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/formBuilder/management/api/v1/systeminfo")]
    public class SystemInfoController(IUmbracoDatabaseFactory databaseFactory) : FormsManagementApiControllerBase
    {
        private readonly IUmbracoDatabaseFactory _databaseFactory = databaseFactory;

        /// <summary>
        /// Retrieves system information used for customization of acceptance test behaviour.
        /// </summary>
        [HttpGet]
        public IActionResult GetSystemInfo()
        {
            bool flag = _databaseFactory.SqlContext.IsSqlite();
            return Ok(new SystemInfo()
            {
                IsWindows = OperatingSystem.IsWindows(),
                IsLinux = OperatingSystem.IsLinux(),
                IsMacOS = OperatingSystem.IsMacOS(),
                IsSqlServer = !flag,
                IsSqlite = flag
            });
        }

        /// <summary>Data object returning system information.</summary>
        public class SystemInfo
        {
            /// <summary>
            /// Gets or sets a value indicating whether the current operating system is Windows.
            /// </summary>
            [JsonPropertyName("isWindows")]
            public bool IsWindows { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the current operating system is Linux.
            /// </summary>
            [JsonPropertyName("isLinux")]
            public bool IsLinux { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the current operating system is MacOS.
            /// </summary>
            [JsonPropertyName("isMacOS")]
            public bool IsMacOS { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether SQL Server is used as the Umbraco database.
            /// </summary>
            [JsonPropertyName("isSqlServer")]
            public bool IsSqlServer { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether SQLite is used as the Umbraco database.
            /// </summary>
            [JsonPropertyName("isSqlite")]
            public bool IsSqlite { get; set; }
        }
    }
}