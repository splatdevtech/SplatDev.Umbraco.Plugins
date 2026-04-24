using System.Reflection;

using Umbraco.Cms.Core.Semver;

namespace FormBuilder.Core.Extensions
{
    internal static class AssemblyExtensions
    {
        public static SemVersion? GetInformationalVersion(this Assembly assembly)
        {
            AssemblyInformationalVersionAttribute? customAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return customAttribute is null || !SemVersion.TryParse(customAttribute.InformationalVersion, out SemVersion? semver) ? null : semver;
        }
    }
}