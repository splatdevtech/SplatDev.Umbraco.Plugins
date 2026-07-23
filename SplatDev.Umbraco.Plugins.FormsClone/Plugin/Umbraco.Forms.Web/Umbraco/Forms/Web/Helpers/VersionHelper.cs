
// Type: Umbraco.Forms.Web.Helpers.VersionHelper
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System.Reflection;

using Umbraco.Cms.Core.Semver;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Extensions;


#nullable enable
namespace Umbraco.Forms.Web.Helpers
{
    internal static class VersionHelper
    {
        public static string GetInstalledPackageVersion()
        {
            Assembly assembly = Assembly.Load("Umbraco.Forms.Core");
            SemVersion semVersion = (object)assembly != null ? assembly.GetInformationalVersion() : null;
            return (object)semVersion == null ? string.Empty : semVersion.ToSemanticStringWithoutBuild();
        }
    }
}
