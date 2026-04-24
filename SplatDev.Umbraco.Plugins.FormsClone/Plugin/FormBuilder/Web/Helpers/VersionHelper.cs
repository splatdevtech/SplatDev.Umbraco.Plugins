using FormBuilder.Core.Extensions;

using System.Reflection;

using Umbraco.Cms.Core.Semver;
using Umbraco.Extensions;

namespace FormBuilder.Web.Helpers
{
    internal static class VersionHelper
    {
        public static string GetInstalledPackageVersion()
        {
            Assembly assembly = Assembly.Load("FormBuilderCore");
            SemVersion? semVersion = assembly as object is not null ? assembly.GetInformationalVersion() : null;
            return semVersion as object is null ? string.Empty : semVersion.ToSemanticStringWithoutBuild();
        }
    }
}