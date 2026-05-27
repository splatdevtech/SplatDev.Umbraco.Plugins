using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    /// <summary>
    /// Downloads and loads NuGet packages declared with <c>install: true</c> in the YAML
    /// <c>packages:</c> section.
    ///
    /// Uses the NuGet v3 flat-container API — no NuGet client library dependency required.
    /// Packages are cached as extracted DLLs under <c>packages/</c> in the content root.
    /// Assemblies are loaded into the current AppDomain via <see cref="Assembly.LoadFrom"/>.
    ///
    /// <para>
    /// Limitation: assemblies loaded at runtime will not have their DI / <c>IComposer</c>
    /// registrations executed. A full Umbraco package install (with DI wiring) requires a
    /// restart after the first run.
    /// </para>
    /// </summary>
    public class PackageInstaller
    {
        // NuGet v3 flat-container endpoint derived from the NuGet.org service index.
        private const string DefaultFlatContainerBase = "https://api.nuget.org/v3-flatcontainer";

        // TFM candidates in descending preference order for net10.0 host projects.
        private static readonly string[] TfmPreference =
        [
            "net10.0", "net9.0", "net8.0", "net7.0", "net6.0",
            "netstandard2.1", "netstandard2.0", "netstandard1.6"
        ];

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<PackageInstaller>? _logger;

        public PackageInstaller(
            IHttpClientFactory httpClientFactory,
            IHostEnvironment hostEnvironment,
            ILogger<PackageInstaller>? logger = null)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _logger = logger;
        }

        /// <summary>
        /// Downloads and loads all packages marked with <c>install: true</c>.
        /// Returns the number of packages that were newly installed (i.e. not already in the AppDomain).
        /// A non-zero return value means the caller should restart the application so that
        /// DI / <c>IComposer</c> registrations from the loaded assemblies are executed.
        /// </summary>
        public int InstallPackages(List<YamlPackage> packages)
        {
            if (packages == null || packages.Count == 0) return 0;

            var toInstall = packages.Where(p => p.Install && !string.IsNullOrWhiteSpace(p.Id)).ToList();
            if (toInstall.Count == 0) return 0;

            var packagesDir = Path.Combine(_hostEnvironment.ContentRootPath, "packages");
            Directory.CreateDirectory(packagesDir);

            var installedCount = 0;
            foreach (var pkg in toInstall)
            {
                try
                {
                    if (InstallOne(pkg, packagesDir))
                        installedCount++;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex,
                        "PackageInstaller: Failed to install '{Id}'. Continuing with remaining packages.", pkg.Id);
                }
            }
            return installedCount;
        }

        /// <returns><c>true</c> if the package was newly downloaded/loaded; <c>false</c> if it was already present.</returns>
        private bool InstallOne(YamlPackage pkg, string packagesDir)
        {
            var assemblyName = pkg.AssemblyName ?? pkg.Id!;

            // Skip if the primary assembly is already loaded.
            if (IsLoaded(assemblyName))
            {
                _logger?.LogInformation(
                    "PackageInstaller: '{Id}' already loaded — skipping download.", pkg.Id);
                return false;
            }

            // Resolve version (download index if not pinned).
            var version = ResolveVersion(pkg);
            if (string.IsNullOrWhiteSpace(version))
            {
                _logger?.LogWarning(
                    "PackageInstaller: Could not resolve a version for '{Id}'. Skipping.", pkg.Id);
                return false;
            }

            // Check local cache first.
            var pkgDir = Path.Combine(packagesDir, $"{pkg.Id!.ToLowerInvariant()}.{version}");
            if (!Directory.Exists(pkgDir) || !Directory.EnumerateFiles(pkgDir, "*.dll").Any())
            {
                DownloadAndExtract(pkg, version, pkgDir);
            }
            else
            {
                _logger?.LogInformation(
                    "PackageInstaller: '{Id}' v{Version} already cached at '{Dir}'.", pkg.Id, version, pkgDir);
            }

            // Load all DLLs in the extracted cache directory.
            var loaded = LoadAssemblies(pkgDir, pkg.Id!, version);
            return loaded > 0;
        }

        /// <summary>
        /// Resolves the concrete version string. If <c>pkg.Version</c> is set, returns it as-is.
        /// Otherwise, queries the NuGet v3 flat-container index for the latest stable version.
        /// </summary>
        private string? ResolveVersion(YamlPackage pkg)
        {
            if (!string.IsNullOrWhiteSpace(pkg.Version))
                return pkg.Version;

            try
            {
                var flatBase = GetFlatContainerBase(pkg.Source);
                var indexUrl = $"{flatBase}/{pkg.Id!.ToLowerInvariant()}/index.json";
                _logger?.LogInformation(
                    "PackageInstaller: No version pinned for '{Id}'; querying {Url}.", pkg.Id, indexUrl);

                var client = _httpClientFactory.CreateClient();
                var json = client.GetStringAsync(indexUrl).GetAwaiter().GetResult();
                var doc = JsonNode.Parse(json);
                var versions = doc?["versions"]?.AsArray()
                    .Select(v => v?.GetValue<string>())
                    .Where(v => v != null && !v.Contains('-')) // exclude pre-release
                    .ToList();

                return versions?.LastOrDefault();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex,
                    "PackageInstaller: Failed to query version index for '{Id}'.", pkg.Id);
                return null;
            }
        }

        private void DownloadAndExtract(YamlPackage pkg, string version, string pkgDir)
        {
            var flatBase = GetFlatContainerBase(pkg.Source);
            var idLower = pkg.Id!.ToLowerInvariant();
            var versionLower = version.ToLowerInvariant();
            var nupkgUrl = $"{flatBase}/{idLower}/{versionLower}/{idLower}.{versionLower}.nupkg";

            _logger?.LogInformation(
                "PackageInstaller: Downloading '{Id}' v{Version} from {Url}.", pkg.Id, version, nupkgUrl);

            var client = _httpClientFactory.CreateClient();
            var nupkgBytes = client.GetByteArrayAsync(nupkgUrl).GetAwaiter().GetResult();

            Directory.CreateDirectory(pkgDir);
            ExtractDlls(nupkgBytes, pkgDir, pkg.Id!, version);

            _logger?.LogInformation(
                "PackageInstaller: Extracted '{Id}' v{Version} to '{Dir}'.", pkg.Id, version, pkgDir);
        }

        /// <summary>
        /// Extracts DLL files from the .nupkg (which is a ZIP archive) for the best matching TFM.
        /// Only the <c>lib/{tfm}/</c> folder is extracted; content/tools/etc. are ignored.
        /// </summary>
        private void ExtractDlls(byte[] nupkgBytes, string targetDir, string pkgId, string version)
        {
            using var zip = new ZipArchive(new MemoryStream(nupkgBytes), ZipArchiveMode.Read);

            // Find all lib/{tfm}/*.dll entries.
            var libEntries = zip.Entries
                .Where(e => e.FullName.StartsWith("lib/", StringComparison.OrdinalIgnoreCase)
                            && e.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (libEntries.Count == 0)
            {
                _logger?.LogWarning(
                    "PackageInstaller: '{Id}' v{Version} contains no lib/*.dll entries.", pkgId, version);
                return;
            }

            // Pick best TFM.
            string? chosenTfm = null;
            foreach (var tfm in TfmPreference)
            {
                if (libEntries.Any(e => e.FullName.StartsWith($"lib/{tfm}/", StringComparison.OrdinalIgnoreCase)))
                {
                    chosenTfm = tfm;
                    break;
                }
            }

            if (chosenTfm == null)
            {
                // Fallback: use whatever TFM exists (pick alphabetically last = presumably newest).
                chosenTfm = libEntries
                    .Select(e => e.FullName.Split('/')[1])
                    .OrderDescending()
                    .First();

                _logger?.LogWarning(
                    "PackageInstaller: '{Id}' v{Version} — no preferred TFM found; falling back to '{Tfm}'.",
                    pkgId, version, chosenTfm);
            }
            else
            {
                _logger?.LogInformation(
                    "PackageInstaller: '{Id}' — selected TFM '{Tfm}'.", pkgId, chosenTfm);
            }

            var prefix = $"lib/{chosenTfm}/";
            foreach (var entry in libEntries.Where(e =>
                e.FullName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
            {
                var destFile = Path.Combine(targetDir, Path.GetFileName(entry.FullName));
                using var src = entry.Open();
                using var dst = File.Create(destFile);
                src.CopyTo(dst);
            }
        }

        private int LoadAssemblies(string pkgDir, string pkgId, string version)
        {
            var dlls = Directory.GetFiles(pkgDir, "*.dll");
            if (dlls.Length == 0)
            {
                _logger?.LogWarning(
                    "PackageInstaller: No DLLs found in cache dir for '{Id}' v{Version}. Skipping load.",
                    pkgId, version);
                return 0;
            }

            var loaded = 0;
            foreach (var dll in dlls)
            {
                var name = Path.GetFileNameWithoutExtension(dll);
                if (IsLoaded(name))
                {
                    _logger?.LogDebug("PackageInstaller: Assembly '{Name}' already in AppDomain.", name);
                    continue;
                }

                try
                {
                    Assembly.LoadFrom(dll);
                    _logger?.LogInformation(
                        "PackageInstaller: Loaded assembly '{Name}' from '{Path}'.", name, dll);
                    loaded++;
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex,
                        "PackageInstaller: Could not load assembly '{Name}' from '{Path}'.", name, dll);
                }
            }

            if (loaded > 0)
            {
                _logger?.LogInformation(
                    "PackageInstaller: '{Id}' v{Version} — {Count} assembly/ies loaded.",
                    pkgId, version, loaded);
            }

            return loaded;
        }

        private static bool IsLoaded(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => string.Equals(a.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Derives the flat-container base URL from a NuGet source URL.
        /// For the default NuGet.org source the flat-container base is well-known.
        /// For other sources we attempt to query the v3 service index to find the resource.
        /// </summary>
        private string GetFlatContainerBase(string? source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return DefaultFlatContainerBase;

            // If the caller already provided a flat-container URL (ends with a package id segment
            // pattern), use it directly.
            if (!source.EndsWith("index.json", StringComparison.OrdinalIgnoreCase))
                return source.TrimEnd('/');

            try
            {
                var client = _httpClientFactory.CreateClient();
                var json = client.GetStringAsync(source).GetAwaiter().GetResult();
                var doc = JsonNode.Parse(json);
                var resources = doc?["resources"]?.AsArray();
                if (resources != null)
                {
                    foreach (var res in resources)
                    {
                        var type = res?["@type"]?.GetValue<string>() ?? string.Empty;
                        if (type.StartsWith("PackageBaseAddress", StringComparison.OrdinalIgnoreCase))
                            return (res?["@id"]?.GetValue<string>() ?? DefaultFlatContainerBase).TrimEnd('/');
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex,
                    "PackageInstaller: Could not query service index at '{Source}'; using default flat-container.",
                    source);
            }

            return DefaultFlatContainerBase;
        }
    }
}
