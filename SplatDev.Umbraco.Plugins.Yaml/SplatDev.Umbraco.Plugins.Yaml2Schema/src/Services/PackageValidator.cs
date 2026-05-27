using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    /// <summary>
    /// Validates that NuGet packages declared in the YAML <c>packages:</c> section are loaded in
    /// the current AppDomain. Missing required packages are logged as errors; missing optional
    /// packages are logged as warnings. No installation is performed — add missing packages to
    /// your <c>.csproj</c> manually.
    /// </summary>
    public class PackageValidator
    {
        private readonly ILogger<PackageValidator>? _logger;

        public PackageValidator(ILogger<PackageValidator>? logger = null)
        {
            _logger = logger;
        }

        public void ValidatePackages(List<YamlPackage> packages)
        {
            if (packages == null || packages.Count == 0) return;

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var pkg in packages)
            {
                if (string.IsNullOrWhiteSpace(pkg.Id))
                {
                    _logger?.LogWarning("A package entry is missing an 'id'. Skipping.");
                    continue;
                }

                var assemblyName = pkg.AssemblyName ?? pkg.Id;
                var assembly = loadedAssemblies.FirstOrDefault(a =>
                    string.Equals(a.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase));

                if (assembly == null)
                {
                    if (pkg.Required)
                        _logger?.LogError(
                            "Required NuGet package '{Id}' (assembly '{Assembly}') is not loaded. " +
                            "Add it to your .csproj: <PackageReference Include=\"{Id}\" Version=\"{Version}\" />",
                            pkg.Id, assemblyName, pkg.Id, pkg.Version ?? "*");
                    else
                        _logger?.LogWarning(
                            "Optional NuGet package '{Id}' (assembly '{Assembly}') is not loaded.",
                            pkg.Id, assemblyName);
                }
                else
                {
                    var loadedVersion = assembly.GetName().Version?.ToString();

                    if (!string.IsNullOrEmpty(pkg.Version) && loadedVersion != null)
                    {
                        // Normalise both to major.minor.patch for a lenient prefix comparison
                        var expected = pkg.Version.TrimEnd('*').TrimEnd('.');
                        if (!loadedVersion.StartsWith(expected, StringComparison.Ordinal))
                        {
                            _logger?.LogWarning(
                                "Package '{Id}' is loaded with version {Loaded}, but expected {Expected}.",
                                pkg.Id, loadedVersion, pkg.Version);
                            continue;
                        }
                    }

                    _logger?.LogInformation(
                        "Package '{Id}' v{Version} is present.", pkg.Id, loadedVersion ?? "?");
                }
            }
        }
    }
}
