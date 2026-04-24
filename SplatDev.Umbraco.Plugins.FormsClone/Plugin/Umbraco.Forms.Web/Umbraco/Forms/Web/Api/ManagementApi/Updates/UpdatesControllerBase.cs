
// Type: Umbraco.Forms.Web.Api.ManagementApi.Updates.UpdatesControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Semver;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Updates
{
  [ApiExplorerSettings(GroupName = "Updates")]
  [Authorize(Policy = "SectionAccessForms")]
  [Route("/umbraco/forms/management/api/v1/updates")]
  public abstract class UpdatesControllerBase : FormsManagementApiControllerBase
  {
    protected const string FeedUrl = "http://nightly.umbraco.org/latest-form-version-v6/";
    protected const string UpgradeUrl = "http://nightly.umbraco.org/umbraco-forms-release/UmbracoForms.Files.{0}.zip";
    protected const string VersionMarker = "/App_Plugins/UmbracoForms/version";
    protected const string InstallMarker = "/App_Plugins/UmbracoForms/installed";

    public class UpdateStatus
    {
      public string RemoteVersion { get; set; } = string.Empty;

      public SemVersion? CurrentVersion { get; set; }

      public bool IsPreRelease { get; set; }

      public bool UpgradeAvailable { get; set; }

      public double HoursSinceInstall { get; set; }
    }
  }
}
