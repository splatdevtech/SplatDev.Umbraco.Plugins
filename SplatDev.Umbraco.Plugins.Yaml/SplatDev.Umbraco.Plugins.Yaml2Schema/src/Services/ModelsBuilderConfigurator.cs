using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    /// <summary>
    /// Writes Umbraco Models Builder settings from the YAML <c>modelsBuilder:</c> section into
    /// <c>appsettings.json</c> under <c>Umbraco:CMS:ModelsBuilder</c>.
    /// Only fields that are explicitly set in the YAML are written; omitted fields are left unchanged.
    /// </summary>
    public class ModelsBuilderConfigurator
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<ModelsBuilderConfigurator>? _logger;

        public ModelsBuilderConfigurator(
            IHostEnvironment hostEnvironment,
            ILogger<ModelsBuilderConfigurator>? logger = null)
        {
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _logger = logger;
        }

        public void Configure(YamlModelsBuilder config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            var hasOutputPath = !string.IsNullOrWhiteSpace(config.OutputPath);
            var hasMode = !string.IsNullOrWhiteSpace(config.Mode);

            if (!hasOutputPath && !hasMode)
            {
                _logger?.LogDebug("ModelsBuilderConfigurator: No settings to apply.");
                return;
            }

            var appSettingsPath = Path.Combine(_hostEnvironment.ContentRootPath, "appsettings.json");
            if (!File.Exists(appSettingsPath))
            {
                _logger?.LogWarning(
                    "ModelsBuilderConfigurator: appsettings.json not found at '{Path}'. Skipping Models Builder configuration.",
                    appSettingsPath);
                return;
            }

            try
            {
                var json = File.ReadAllText(appSettingsPath);
                var doc = JsonNode.Parse(json, nodeOptions: null,
                    documentOptions: new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip })
                    ?? new JsonObject();

                // Navigate/create: Umbraco → CMS → ModelsBuilder
                var umbracoNode = (doc["Umbraco"] as JsonObject) ?? new JsonObject();
                doc["Umbraco"] = umbracoNode;

                var cmsNode = (umbracoNode["CMS"] as JsonObject) ?? new JsonObject();
                umbracoNode["CMS"] = cmsNode;

                var mbNode = (cmsNode["ModelsBuilder"] as JsonObject) ?? new JsonObject();
                cmsNode["ModelsBuilder"] = mbNode;

                if (hasOutputPath)
                {
                    var resolved = Path.IsPathRooted(config.OutputPath)
                        ? config.OutputPath!
                        : Path.Combine(_hostEnvironment.ContentRootPath, config.OutputPath!);

                    mbNode["ModelsDirectoryAbsolute"] = resolved;
                    _logger?.LogInformation(
                        "ModelsBuilderConfigurator: Set ModelsDirectoryAbsolute = '{Path}'.", resolved);
                }

                if (hasMode)
                {
                    mbNode["ModelsMode"] = config.Mode;
                    _logger?.LogInformation(
                        "ModelsBuilderConfigurator: Set ModelsMode = '{Mode}'.", config.Mode);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(appSettingsPath, doc.ToJsonString(options));

                _logger?.LogInformation(
                    "ModelsBuilderConfigurator: appsettings.json updated successfully.");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,
                    "ModelsBuilderConfigurator: Failed to update appsettings.json.");
                throw;
            }
        }
    }
}
