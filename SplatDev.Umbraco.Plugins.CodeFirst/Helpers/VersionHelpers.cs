namespace SplatDev.Umbraco.Plugins.CodeFirst.Helpers
{
    using System;
    using System.Reflection;

    // NOTE: ConfigurationManager (System.Configuration) is not available in .NET 8+.
    // TODO: Inject IConfiguration from Microsoft.Extensions.Configuration to read app settings.
    public static class VersionHelpers
    {
        /// <summary>
        /// Checks if installation is required based on a version stored in configuration.
        /// </summary>
        /// <param name="pluginName">The configuration key for the plugin version.</param>
        /// <param name="assembly">The assembly whose version is checked.</param>
        /// <param name="configuredVersion">The version string from IConfiguration (inject and pass from caller).</param>
        public static bool RequiresInstallation(string pluginName, Assembly assembly, string? configuredVersion = null)
        {
            return configuredVersion == null || assembly.GetName().Version!.CompareTo(Version.Parse(configuredVersion)) > 0;
        }

        /// <summary>
        /// Checks if the specified version is less than or equal to the configured version.
        /// </summary>
        /// <param name="version">The version to compare.</param>
        /// <param name="pluginName">The configuration key (informational only).</param>
        /// <param name="configuredVersion">The version string from IConfiguration (inject and pass from caller).</param>
        public static bool VersionIsLessOrEqual(string version, string pluginName, string? configuredVersion = null)
        {
            return configuredVersion == null || Version.Parse(version).CompareTo(Version.Parse(configuredVersion)) > 0;
        }
    }
}
