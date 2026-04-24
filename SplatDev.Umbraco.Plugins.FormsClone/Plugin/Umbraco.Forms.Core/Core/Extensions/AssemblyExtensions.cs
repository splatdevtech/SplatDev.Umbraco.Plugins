
// Type: Umbraco.Forms.Core.Extensions.AssemblyExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Reflection;

using Umbraco.Cms.Core.Semver;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
    public static class AssemblyExtensions
    {
        public static SemVersion? GetInformationalVersion(this Assembly assembly)
        {
            AssemblyInformationalVersionAttribute customAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            SemVersion semver;
            return customAttribute == null || !SemVersion.TryParse(customAttribute.InformationalVersion, out semver) ? null : semver;
        }
    }
}
