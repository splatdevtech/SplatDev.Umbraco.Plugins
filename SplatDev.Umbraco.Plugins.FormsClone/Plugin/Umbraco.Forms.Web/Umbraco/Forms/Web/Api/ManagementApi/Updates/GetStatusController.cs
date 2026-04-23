
// Type: Umbraco.Forms.Web.Api.ManagementApi.Updates.GetStatusController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

using System.Text.Json.Nodes;

using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.Semver;

#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Updates
{
    public class GetStatusController : UpdatesControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;

        public GetStatusController(
          IHttpContextAccessor httpContextAccessor,
          IHostEnvironment hostEnvironment)
        {
            this._hostEnvironment = hostEnvironment;
        }

        [HttpGet("status")]
        [ProducesResponseType(typeof(UpdatesControllerBase.UpdateStatus), 200)]
        public async Task<IActionResult> GetUpdateStatus()
        {
            GetStatusController statusController = this;
            if (!System.IO.File.Exists(statusController._hostEnvironment.MapPathContentRoot("/App_Plugins/UmbracoForms/installed")))
                return statusController.Ok();
            TimeSpan timeSinceInstall = DateTime.Now.Subtract(System.IO.File.GetCreationTime(statusController._hostEnvironment.MapPathContentRoot("/App_Plugins/UmbracoForms/installed")));
            if (timeSinceInstall.TotalHours < 24.0)
                return statusController.Ok();
            CancellationToken cancellationToken = statusController.HttpContext.RequestAborted;
            SemVersion localVersion = new SemVersion(0);
            string remoteVersion = string.Empty;
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage updateCheckerResponse = await httpClient.GetAsync("http://nightly.umbraco.org/latest-form-version-v6/", cancellationToken))
                {
                    if (updateCheckerResponse.IsSuccessStatusCode)
                        remoteVersion = JsonNode.Parse(await updateCheckerResponse.Content.ReadAsStringAsync(cancellationToken))?["Version"]?.GetValue<string>() ?? string.Empty;
                }
            }
            SemVersion remote;
            SemVersion.TryParse(remoteVersion, out remote);
            if (System.IO.File.Exists(statusController._hostEnvironment.MapPathContentRoot("/App_Plugins/UmbracoForms/version")))
            {
                if (!SemVersion.TryParse((await System.IO.File.ReadAllTextAsync(statusController._hostEnvironment.MapPathContentRoot("/App_Plugins/UmbracoForms/version"), cancellationToken)).Trim().ToLower(), out localVersion))
                    localVersion = remote;
            }
            UpdatesControllerBase.UpdateStatus updateStatus = new UpdatesControllerBase.UpdateStatus()
            {
                RemoteVersion = remoteVersion,
                CurrentVersion = localVersion,
                UpgradeAvailable = (object)remote != null && (object)localVersion != null && remote > localVersion,
                HoursSinceInstall = timeSinceInstall.TotalHours
            };
            return statusController.Ok(updateStatus);
        }
    }
}
