using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    /// <summary>
    /// Creates Umbraco 14+ App_Plugins manifests (<c>umbraco-package.json</c>) and optional
    /// JavaScript files for custom (frontend-only) property editors declared in the YAML
    /// <c>propertyEditors:</c> section.
    /// </summary>
    public class PropertyEditorCreator
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<PropertyEditorCreator>? _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public PropertyEditorCreator(
            IWebHostEnvironment webHostEnvironment,
            ILogger<PropertyEditorCreator>? logger = null)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _logger = logger;
        }

        public void CreatePropertyEditors(List<YamlPropertyEditor> editors)
        {
            if (editors == null || editors.Count == 0) return;

            foreach (var editor in editors)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(editor.Alias))
                    {
                        _logger?.LogWarning("A propertyEditor entry is missing an 'alias'. Skipping.");
                        continue;
                    }

                    var folderName = editor.FolderName
                        ?? editor.Alias.Replace(".", "-").ToLowerInvariant();

                    var pluginDir = Path.Combine(
                        _webHostEnvironment.WebRootPath, "App_Plugins", folderName);

                    // [REMOVE]
                    if (editor.Remove)
                    {
                        if (Directory.Exists(pluginDir))
                        {
                            Directory.Delete(pluginDir, recursive: true);
                            _logger?.LogInformation(
                                "PropertyEditor '{Alias}' App_Plugins folder removed.", editor.Alias);
                        }
                        else
                        {
                            _logger?.LogDebug(
                                "PropertyEditor '{Alias}' folder not found for removal. Skipping.", editor.Alias);
                        }
                        continue;
                    }

                    var manifestPath = Path.Combine(pluginDir, "umbraco-package.json");

                    // Skip if already exists and not flagged for update
                    if (File.Exists(manifestPath) && !editor.Update)
                    {
                        _logger?.LogInformation(
                            "PropertyEditor '{Alias}' manifest already exists. Skipping.", editor.Alias);
                        continue;
                    }

                    // Derive aliases and paths
                    var uiAlias  = editor.UiAlias  ?? $"{editor.Alias}.Ui";
                    var jsRelPath = editor.JsPath  ?? $"/App_Plugins/{folderName}/index.js";
                    var jsAbsPath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        jsRelPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    Directory.CreateDirectory(pluginDir);

                    // Build Umbraco 14+ package manifest
                    var manifest = new
                    {
                        name    = editor.Name ?? editor.Alias,
                        version = "1.0.0",
                        allowPublicAccess = true,
                        extensions = new object[]
                        {
                            new
                            {
                                type  = "propertyEditorSchema",
                                alias = editor.Alias,
                                name  = editor.Name ?? editor.Alias,
                                meta  = new
                                {
                                    defaultPropertyEditorUiAlias = uiAlias
                                }
                            },
                            new
                            {
                                type  = "propertyEditorUi",
                                alias = uiAlias,
                                name  = $"{editor.Name ?? editor.Alias} UI",
                                js    = jsRelPath,
                                meta  = new
                                {
                                    label                      = editor.Name ?? editor.Alias,
                                    propertyEditorSchemaAlias  = editor.Alias,
                                    icon                       = editor.Icon  ?? "icon-code",
                                    group                      = editor.Group ?? "common"
                                }
                            }
                        }
                    };

                    File.WriteAllText(manifestPath, JsonSerializer.Serialize(manifest, JsonOptions));
                    _logger?.LogInformation(
                        "PropertyEditor '{Alias}' manifest written to '{Path}'.", editor.Alias, manifestPath);

                    // Write JS file when inline content is provided
                    if (!string.IsNullOrWhiteSpace(editor.JsContent))
                    {
                        if (!File.Exists(jsAbsPath) || editor.Update)
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(jsAbsPath)!);
                            File.WriteAllText(jsAbsPath, editor.JsContent);
                            _logger?.LogInformation(
                                "PropertyEditor '{Alias}' JS written to '{Path}'.", editor.Alias, jsAbsPath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error creating property editor '{Alias}'.", editor.Alias);
                    throw;
                }
            }
        }
    }
}
