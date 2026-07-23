using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

using System.Text.Json.Nodes;

using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.Semver;

namespace FormBuilder.Web.Api.Management.Controllers
{
    public class GetStatusController(
      IHostEnvironment hostEnvironment) : UpdatesControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

        /// <summary>Retrieves the status of updates.</summary>
        [HttpGet("status")]
        [ProducesResponseType(typeof(UpdateStatus), 200)]
        public async Task<IActionResult> GetUpdateStatus()
        {
            GetStatusController statusController = this;
            if (!System.IO.File.Exists(statusController._hostEnvironment.MapPathContentRoot("/App_Plugins/FormBuilder/installed")))
                return statusController.Ok();
            TimeSpan timeSinceInstall = DateTime.Now.Subtract(System.IO.File.GetCreationTime(statusController._hostEnvironment.MapPathContentRoot("/App_Plugins/FormBuilder/installed")));
            if (timeSinceInstall.TotalHours < 24.0)
                return statusController.Ok();
            CancellationToken cancellationToken = statusController.HttpContext.RequestAborted;
            SemVersion? localVersion = new(0);
            string remoteVersion = string.Empty;
            using (HttpClient httpClient = new())
            {
                using HttpResponseMessage updateCheckerResponse = await httpClient.GetAsync("http://formbuilder.com/version-check", cancellationToken);
                if (updateCheckerResponse.IsSuccessStatusCode)
                    remoteVersion = JsonNode.Parse(await updateCheckerResponse.Content.ReadAsStringAsync(cancellationToken))?["Version"]?.GetValue<string>() ?? string.Empty;
            }
            _ = SemVersion.TryParse(remoteVersion, out var remote);
            if (System.IO.File.Exists(statusController._hostEnvironment.MapPathContentRoot("/App_Plugins/FormBuilder/version")))
            {
                if (!SemVersion.TryParse((await System.IO.File.ReadAllTextAsync(statusController._hostEnvironment.MapPathContentRoot("/App_Plugins/FormBuilder/version"), cancellationToken)).Trim().ToLower(), out localVersion))
                    localVersion = remote;
            }
            UpdateStatus updateStatus = new()
            {
                RemoteVersion = remoteVersion,
                CurrentVersion = localVersion,
                UpgradeAvailable = remote is not null && localVersion is not null && remote > localVersion,
                HoursSinceInstall = timeSinceInstall.TotalHours
            };
            return statusController.Ok(updateStatus);
        }
    }
}