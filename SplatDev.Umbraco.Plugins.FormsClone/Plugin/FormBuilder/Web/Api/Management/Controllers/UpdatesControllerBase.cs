using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Semver;

namespace FormBuilder.Web.Api.Management.Controllers
{
    [ApiExplorerSettings(GroupName = "Updates")]
    [Authorize(Policy = "SectionAccessForms")]
    [Route("/formBuilder/management/api/v1/updates")]
    public abstract class UpdatesControllerBase : FormsManagementApiControllerBase
    {
        protected const string FeedUrl = "http://nightly.formbuilder.com/check-version/";
        protected const string UpgradeUrl = "http://nightly.formbuilder.com/releases/FormBuilder.Files.{0}.zip";
        protected const string VersionMarker = "/App_Plugins/FormBuilder/version";
        protected const string InstallMarker = "/App_Plugins/FormBuilder/installed";

        /// <summary>Representation of the update status.</summary>
        public class UpdateStatus
        {
            /// <summary>Gets or sets the remote version.</summary>
            public string RemoteVersion { get; set; } = string.Empty;

            /// <summary>Gets or sets the current version.</summary>
            public SemVersion? CurrentVersion { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the update status is to a prerelease.
            /// </summary>
            public bool IsPreRelease { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether an upgrade is available.
            /// </summary>
            public bool UpgradeAvailable { get; set; }

            /// <summary>
            /// Gets or sets the hours since the current version was installed.
            /// </summary>
            public double HoursSinceInstall { get; set; }
        }
    }
}