using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    public class StaticAssetCreator
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<StaticAssetCreator>? _logger;

        public StaticAssetCreator(IWebHostEnvironment webHostEnvironment, ILogger<StaticAssetCreator>? logger = null)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _logger = logger;
        }

        public void CreateScripts(List<YamlScript> scripts)
        {
            if (scripts == null) throw new ArgumentNullException(nameof(scripts));

            var processedAliases = new HashSet<string>();

            foreach (var script in scripts)
            {
                if (string.IsNullOrWhiteSpace(script.Alias))
                {
                    _logger?.LogWarning("Script is missing an alias. Skipping.");
                    continue;
                }

                if (processedAliases.Contains(script.Alias))
                {
                    _logger?.LogWarning("Script with alias '{Alias}' is a duplicate and will be skipped.", script.Alias);
                    continue;
                }

                if (script.Remove)
                {
                    DeleteAssetFile(script.Path, "Script", script.Alias);
                    processedAliases.Add(script.Alias);
                    continue;
                }

                WriteAssetFile(script.Path, script.Content, "Script", script.Alias, overwrite: script.Update);
                processedAliases.Add(script.Alias);
            }
        }

        public void CreateStylesheets(List<YamlStylesheet> stylesheets)
        {
            if (stylesheets == null) throw new ArgumentNullException(nameof(stylesheets));

            var processedAliases = new HashSet<string>();

            foreach (var stylesheet in stylesheets)
            {
                if (string.IsNullOrWhiteSpace(stylesheet.Alias))
                {
                    _logger?.LogWarning("Stylesheet is missing an alias. Skipping.");
                    continue;
                }

                if (processedAliases.Contains(stylesheet.Alias))
                {
                    _logger?.LogWarning("Stylesheet with alias '{Alias}' is a duplicate and will be skipped.", stylesheet.Alias);
                    continue;
                }

                if (stylesheet.Remove)
                {
                    DeleteAssetFile(stylesheet.Path, "Stylesheet", stylesheet.Alias);
                    processedAliases.Add(stylesheet.Alias);
                    continue;
                }

                WriteAssetFile(stylesheet.Path, stylesheet.Content, "Stylesheet", stylesheet.Alias, overwrite: stylesheet.Update);
                processedAliases.Add(stylesheet.Alias);
            }
        }

        private void DeleteAssetFile(string relativePath, string assetType, string alias)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                _logger?.LogWarning("{AssetType} '{Alias}' has no path configured. Cannot remove.", assetType, alias);
                return;
            }

            var webRootPath = _webHostEnvironment.WebRootPath;
            if (string.IsNullOrEmpty(webRootPath))
            {
                _logger?.LogWarning("WebRootPath is not configured. Cannot remove {AssetType} '{Alias}'.", assetType, alias);
                return;
            }

            var normalizedRelative = relativePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(webRootPath, normalizedRelative);

            if (!File.Exists(fullPath))
            {
                _logger?.LogWarning("{AssetType} '{Alias}' not found at '{Path}'. Skipping removal.", assetType, alias, relativePath);
                return;
            }

            try
            {
                File.Delete(fullPath);
                _logger?.LogInformation("{AssetType} '{Alias}' removed from '{Path}'.", assetType, alias, relativePath);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error removing {AssetType} '{Alias}' from '{Path}'.", assetType, alias, relativePath);
                throw;
            }
        }

        private void WriteAssetFile(string relativePath, string? content, string assetType, string alias, bool overwrite = false)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                _logger?.LogWarning("{AssetType} '{Alias}' has no path configured. Skipping.", assetType, alias);
                return;
            }

            var webRootPath = _webHostEnvironment.WebRootPath;
            if (string.IsNullOrEmpty(webRootPath))
            {
                _logger?.LogWarning("WebRootPath is not configured. Cannot write {AssetType} '{Alias}'.", assetType, alias);
                return;
            }

            var normalizedRelative = relativePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(webRootPath, normalizedRelative);

            if (File.Exists(fullPath) && !overwrite)
            {
                _logger?.LogInformation("{AssetType} '{Alias}' already exists at '{Path}'. Skipping.", assetType, alias, relativePath);
                return;
            }

            try
            {
                var directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllText(fullPath, content ?? string.Empty);

                var action = overwrite ? "updated" : "created";
                _logger?.LogInformation("{AssetType} '{Alias}' {Action} at '{Path}'.", assetType, alias, action, relativePath);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error writing {AssetType} '{Alias}' to '{Path}'.", assetType, alias, relativePath);
                throw;
            }
        }
    }
}
