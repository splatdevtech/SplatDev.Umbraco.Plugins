using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    public class DataTypeCreator
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly IContentTypeService _contentTypeService;
        private readonly PropertyEditorCollection _propertyEditors;
        private readonly IConfigurationEditorJsonSerializer _configSerializer;
        private readonly ILogger<DataTypeCreator>? _logger;

        public DataTypeCreator(
            IDataTypeService dataTypeService,
            IContentTypeService contentTypeService,
            PropertyEditorCollection propertyEditors,
            IConfigurationEditorJsonSerializer configSerializer,
            ILogger<DataTypeCreator>? logger = null)
        {
            _dataTypeService = dataTypeService ?? throw new ArgumentNullException(nameof(dataTypeService));
            _contentTypeService = contentTypeService ?? throw new ArgumentNullException(nameof(contentTypeService));
            _propertyEditors = propertyEditors ?? throw new ArgumentNullException(nameof(propertyEditors));
            _configSerializer = configSerializer ?? throw new ArgumentNullException(nameof(configSerializer));
            _logger = logger;
        }

        /// <summary>
        /// Maps the server-side property editor schema alias (e.g. <c>Umbraco.TextBox</c>) to the
        /// Umbraco 14+ backoffice Web-Component UI alias (e.g. <c>Umb.PropertyEditorUi.TextBox</c>).
        ///
        /// Extracted directly from the Umbraco 17.2.2 backoffice static asset manifests
        /// (<c>defaultPropertyEditorUiAlias</c> field of each <c>propertyEditorSchema</c> manifest).
        /// Fall-back: if the alias is not in the map (e.g. a third-party editor) we reuse the
        /// schema alias so that the DataType is still saved rather than throwing.
        /// </summary>
        private static readonly Dictionary<string, string> _editorUiAliasMap =
            new(StringComparer.OrdinalIgnoreCase)
        {
            { "Umbraco.BlockGrid",              "Umb.PropertyEditorUi.BlockGrid" },
            { "Umbraco.BlockList",              "Umb.PropertyEditorUi.BlockList" },
            { "Umbraco.CheckBoxList",           "Umb.PropertyEditorUi.CheckBoxList" },
            { "Umbraco.ColorPicker",            "Umb.PropertyEditorUi.ColorPicker" },
            { "Umbraco.ColorPicker.EyeDropper", "Umb.PropertyEditorUi.EyeDropper" },
            { "Umbraco.ContentPicker",          "Umb.PropertyEditorUi.DocumentPicker" },
            { "Umbraco.DateOnly",               "Umb.PropertyEditorUi.DateOnlyPicker" },
            { "Umbraco.DateTime",               "Umb.PropertyEditorUi.DatePicker" },
            { "Umbraco.DateTimeUnspecified",    "Umb.PropertyEditorUi.DateTimePicker" },
            { "Umbraco.DateTimeWithTimeZone",   "Umb.PropertyEditorUi.DateTimeWithTimeZonePicker" },
            { "Umbraco.Decimal",                "Umb.PropertyEditorUi.Decimal" },
            { "Umbraco.DropDown.Flexible",      "Umb.PropertyEditorUi.Dropdown" },
            { "Umbraco.EmailAddress",           "Umb.PropertyEditorUi.EmailAddress" },
            { "Umbraco.ImageCropper",           "Umb.PropertyEditorUi.ImageCropper" },
            { "Umbraco.Integer",                "Umb.PropertyEditorUi.Integer" },
            { "Umbraco.Label",                  "Umb.PropertyEditorUi.Label" },
            { "Umbraco.ListView",               "Umb.PropertyEditorUi.Collection" },
            { "Umbraco.MarkdownEditor",         "Umb.PropertyEditorUi.MarkdownEditor" },
            { "Umbraco.MediaPicker3",           "Umb.PropertyEditorUi.MediaPicker" },
            { "Umbraco.MemberGroupPicker",      "Umb.PropertyEditorUi.MemberGroupPicker" },
            { "Umbraco.MemberPicker",           "Umb.PropertyEditorUi.MemberPicker" },
            { "Umbraco.MultiNodeTreePicker",    "Umb.PropertyEditorUi.ContentPicker" },
            { "Umbraco.MultiUrlPicker",         "Umb.PropertyEditorUi.MultiUrlPicker" },
            { "Umbraco.MultipleTextstring",     "Umb.PropertyEditorUi.MultipleTextString" },
            { "Umbraco.RadioButtonList",        "Umb.PropertyEditorUi.RadioButtonList" },
            { "Umbraco.RichText",               "Umb.PropertyEditorUi.Tiptap" },
            { "Umbraco.SingleBlock",            "Umb.PropertyEditorUi.BlockSingle" },
            { "Umbraco.Slider",                 "Umb.PropertyEditorUi.Slider" },
            { "Umbraco.Tags",                   "Umb.PropertyEditorUi.Tags" },
            { "Umbraco.TextArea",               "Umb.PropertyEditorUi.TextArea" },
            { "Umbraco.TextBox",                "Umb.PropertyEditorUi.TextBox" },
            { "Umbraco.TimeOnly",               "Umb.PropertyEditorUi.TimeOnlyPicker" },
            { "Umbraco.TrueFalse",              "Umb.PropertyEditorUi.Toggle" },
            { "Umbraco.UploadField",            "Umb.PropertyEditorUi.UploadField" },
            { "Umbraco.UserPicker",             "Umb.PropertyEditorUi.UserPicker" },
        };

        private static string ResolveEditorUiAlias(string editorAlias) =>
            _editorUiAliasMap.TryGetValue(editorAlias, out var uiAlias) ? uiAlias : editorAlias;

        public void CreateDataTypes(List<YamlDataType> dataTypes)
        {
            if (dataTypes == null)
            {
                throw new ArgumentNullException(nameof(dataTypes));
            }

            var processedAliases = new HashSet<string>();

            foreach (var yamlDataType in dataTypes)
            {
                try
                {
                    // Skip if alias has already been processed in this batch
                    if (processedAliases.Contains(yamlDataType.Alias))
                    {
                        _logger?.LogWarning(
                            "DataType with alias '{Alias}' is a duplicate and will be skipped.",
                            yamlDataType.Alias
                        );
                        continue;
                    }

                    // [UPDATE] — re-apply config + DatabaseType if exists; create if not found
                    if (yamlDataType.Update)
                    {
                        var existingIface = _dataTypeService.GetDataType(yamlDataType.Name);
                        if (existingIface is DataType existing)
                        {
                            // Re-derive storage type and UI alias from the editor so stale entries are corrected
                            var updEditor = ResolveEditor(yamlDataType.Editor, yamlDataType.ValueType);
                            if (updEditor != null)
                            {
                                existing.DatabaseType = string.IsNullOrWhiteSpace(yamlDataType.ValueType)
                                    ? updEditor.GetValueEditor().ValueType switch
                                    {
                                        "TEXT"    => ValueStorageType.Ntext,
                                        "INT"     => ValueStorageType.Integer,
                                        "INTEGER" => ValueStorageType.Integer,
                                        "BIGINT"  => ValueStorageType.Integer,
                                        "DECIMAL" => ValueStorageType.Decimal,
                                        "DATE"    => ValueStorageType.Date,
                                        _         => ValueStorageType.Nvarchar
                                    }
                                    : MapValueType(yamlDataType.ValueType);
#if !NET8_0
                                // EditorUiAlias is a Umbraco 14+ (new backoffice) concept; not present in U13
                                existing.EditorUiAlias = ResolveEditorUiAlias(yamlDataType.Editor);
#endif
                            }

                            // Re-apply config so stale or incorrectly-formatted config is fixed
                            if (yamlDataType.Config != null && yamlDataType.Config.Count > 0)
                            {
                                ApplyConfig(yamlDataType.Config);
                                #if NET10_0_OR_GREATER
                                existing.SetConfigurationData(yamlDataType.Config);
#endif
                            }

                            _dataTypeService.Save(existing, Constants.Security.SuperUserId);
                            _logger?.LogInformation(
                                "DataType '{Name}' updated (config + storage type re-applied).",
                                yamlDataType.Name);
                            processedAliases.Add(yamlDataType.Alias);
                            continue;
                        }
                        if (existingIface != null)
                        {
                            // Unexpected concrete type — skip update, treat as existing
                            _logger?.LogInformation(
                                "DataType '{Name}' exists but is not a concrete DataType; skipping update.",
                                yamlDataType.Name);
                            processedAliases.Add(yamlDataType.Alias);
                            continue;
                        }
                        // Not found — fall through to creation below
                        _logger?.LogInformation(
                            "DataType '{Name}' not found during UPDATE; will create it.",
                            yamlDataType.Name);
                    }

                    // [REMOVE] — delete the DataType if flagged
                    if (yamlDataType.Remove)
                    {
                        var toDelete = _dataTypeService.GetDataType(yamlDataType.Name);
                        if (toDelete != null)
                        {
                            _dataTypeService.Delete(toDelete, Constants.Security.SuperUserId);
                            _logger?.LogInformation(
                                "DataType '{Name}' with alias '{Alias}' removed.",
                                yamlDataType.Name, yamlDataType.Alias);
                        }
                        else
                        {
                            _logger?.LogDebug(
                                "DataType '{Name}' not found for removal. Skipping.",
                                yamlDataType.Name);
                        }
                        processedAliases.Add(yamlDataType.Alias);
                        continue;
                    }

                    // Check if a DataType with the same name already exists
                    var existingByName = _dataTypeService.GetDataType(yamlDataType.Name);
                    if (existingByName != null)
                    {
                        _logger?.LogInformation(
                            "DataType '{Name}' already exists. Skipping.",
                            yamlDataType.Name
                        );
                        processedAliases.Add(yamlDataType.Alias);
                        continue;
                    }

                    // Look up the property editor by alias; fall back to a built-in when
                    // valueType is set (custom frontend-only editors have no server-side IDataEditor)
                    var editor = ResolveEditor(yamlDataType.Editor, yamlDataType.ValueType);
                    if (editor == null)
                    {
                        _logger?.LogWarning(
                            "Property editor '{EditorAlias}' not found. Skipping DataType '{Alias}'. " +
                            "For custom frontend-only editors add 'valueType: NVARCHAR' (or NTEXT/INT/etc.) to enable fallback creation.",
                            yamlDataType.Editor,
                            yamlDataType.Alias
                        );
                        continue;
                    }

                    // Determine storage type: explicit valueType overrides editor's default
                    var dbType = !string.IsNullOrWhiteSpace(yamlDataType.ValueType)
                        ? MapValueType(yamlDataType.ValueType)
                        : editor.GetValueEditor().ValueType switch
                        {
                            "TEXT"    => ValueStorageType.Ntext,
                            "INT"     => ValueStorageType.Integer,
                            "INTEGER" => ValueStorageType.Integer,
                            "BIGINT"  => ValueStorageType.Integer,
                            "DECIMAL" => ValueStorageType.Decimal,
                            "DATE"    => ValueStorageType.Date,
                            _         => ValueStorageType.Nvarchar
                        };

                    var dataType = new DataType(editor, _configSerializer, -1)
                    {
                        Name = yamlDataType.Name,
                        DatabaseType = dbType,
#if !NET8_0
                        // EditorUiAlias is a Umbraco 14+ (new backoffice) concept; not present in U13
                        EditorUiAlias = ResolveEditorUiAlias(yamlDataType.Editor)
#endif
                    };

                    // Apply config from YAML (supports Block List, Image Cropper, etc.)
                    if (yamlDataType.Config != null && yamlDataType.Config.Count > 0)
                    {
                        ApplyConfig(yamlDataType.Config);
#if NET10_0_OR_GREATER
                        dataType.SetConfigurationData(yamlDataType.Config);
#endif
                    }

                    // Save the DataType
                    _dataTypeService.Save(dataType, Constants.Security.SuperUserId);
                    _logger?.LogInformation(
                        "DataType '{Name}' with alias '{Alias}' created successfully.",
                        yamlDataType.Name,
                        yamlDataType.Alias
                    );

                    processedAliases.Add(yamlDataType.Alias);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(
                        ex,
                        "Error creating DataType '{Name}' with alias '{Alias}'.",
                        yamlDataType.Name,
                        yamlDataType.Alias
                    );
                    throw;
                }
            }
        }

        /// <summary>
        /// Tries to resolve a server-side <c>IDataEditor</c> for the given alias.
        /// When the alias is not registered (custom frontend-only editor), falls back to a
        /// built-in editor whose storage type matches <paramref name="valueType"/>.
        /// Returns <c>null</c> if neither the primary alias nor the fallback can be found.
        /// </summary>
        private IDataEditor? ResolveEditor(string editorAlias, string? valueType)
        {
            if (_propertyEditors.TryGet(editorAlias, out var editor) && editor != null)
                return editor;

            if (string.IsNullOrWhiteSpace(valueType))
                return null;

            var fallbackAlias = GetFallbackEditorAlias(valueType);
            _propertyEditors.TryGet(fallbackAlias, out var fallback);
            if (fallback != null)
                _logger?.LogInformation(
                    "Editor '{EditorAlias}' not found server-side; using '{Fallback}' as storage fallback (valueType: {ValueType}).",
                    editorAlias, fallbackAlias, valueType);
            return fallback;
        }

        private static string GetFallbackEditorAlias(string valueType) =>
            valueType.ToUpperInvariant() switch
            {
                "TEXT"    => "Umbraco.TextArea",
                "NTEXT"   => "Umbraco.TextArea",
                "INT"     => "Umbraco.Integer",
                "INTEGER" => "Umbraco.Integer",
                "BIGINT"  => "Umbraco.Integer",
                "DECIMAL" => "Umbraco.Decimal",
                "DATE"    => "Umbraco.DateTime",
                _         => "Umbraco.TextBox"   // NVARCHAR and anything unknown
            };

        private static ValueStorageType MapValueType(string valueType) =>
            valueType.ToUpperInvariant() switch
            {
                "TEXT"    => ValueStorageType.Ntext,
                "NTEXT"   => ValueStorageType.Ntext,
                "INT"     => ValueStorageType.Integer,
                "INTEGER" => ValueStorageType.Integer,
                "BIGINT"  => ValueStorageType.Integer,
                "DECIMAL" => ValueStorageType.Decimal,
                "DATE"    => ValueStorageType.Date,
                _         => ValueStorageType.Nvarchar
            };

        /// <summary>
        /// Resolves <c>contentElementTypeAlias</c> entries inside Block List (and Block Grid) DataType
        /// configs to their actual <c>contentElementTypeKey</c> GUIDs. Must be called after DocumentTypes
        /// have been created so the element types exist and can be looked up by alias.
        /// </summary>
        public void LinkBlockListElementTypes(List<YamlDataType> dataTypes)
        {
            if (dataTypes == null) return;

            foreach (var yamlDataType in dataTypes)
            {
                // ── Single Block ─────────────────────────────────────────────────────────
                // Config is flat: contentElementTypeAlias at the top level (not in a blocks list)
                if (string.Equals(yamlDataType.Editor, "Umbraco.SingleBlock", StringComparison.OrdinalIgnoreCase))
                {
                    if (yamlDataType.Config == null) continue;

                    var cfg = NormaliseDictKeys(yamlDataType.Config as System.Collections.IDictionary)
                              ?? yamlDataType.Config
                                  .ToDictionary(k => k.Key, v => v.Value, StringComparer.OrdinalIgnoreCase);

                    if (!cfg.TryGetValue("contentElementTypeAlias", out var rawAlias) || rawAlias == null)
                        continue;

                    var alias = rawAlias.ToString()!;
                    var contentType = _contentTypeService.Get(alias);
                    if (contentType == null)
                    {
                        _logger?.LogWarning(
                            "Single Block element type '{Alias}' not found for DataType '{Name}'.",
                            alias, yamlDataType.Name);
                        continue;
                    }

                    cfg.Remove("contentElementTypeAlias");
                    cfg["contentElementTypeKey"] = contentType.Key.ToString();
                    yamlDataType.Config = cfg;

                    var sbExisting = _dataTypeService.GetDataType(yamlDataType.Name);
                    if (sbExisting is DataType sbDt)
                    {
#if NET10_0_OR_GREATER
                        sbDt.SetConfigurationData(yamlDataType.Config);
#endif
                        _dataTypeService.Save(sbDt, Constants.Security.SuperUserId);
                    }

                    _logger?.LogInformation(
                        "Resolved contentElementTypeAlias '{Alias}' → '{Key}' for Single Block DataType '{Name}'.",
                        alias, contentType.Key, yamlDataType.Name);
                    continue;
                }

                if (!string.Equals(yamlDataType.Editor, "Umbraco.BlockList", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(yamlDataType.Editor, "Umbraco.BlockGrid", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (yamlDataType.Config == null
                    || !yamlDataType.Config.TryGetValue("blocks", out var rawBlocks)
                    || rawBlocks is not List<object> blocks
                    || blocks.Count == 0)
                    continue;

                bool changed = false;

                for (int i = 0; i < blocks.Count; i++)
                {
                    // YamlDotNet deserialises nested mappings as Dictionary<object,object>
                    // when the target field type is object. Normalise to string keys first.
                    var blockDict = NormaliseDictKeys(blocks[i] as IDictionary);
                    if (blockDict == null) continue;

                    blocks[i] = blockDict; // store normalised version in-place

                    if (!blockDict.TryGetValue("contentElementTypeAlias", out var rawAlias) || rawAlias == null)
                        continue;

                    var alias = rawAlias.ToString()!;
                    var contentType = _contentTypeService.Get(alias);
                    if (contentType == null)
                    {
                        _logger?.LogWarning(
                            "Content element type '{Alias}' not found for BlockList DataType '{Name}'. Block kept without key.",
                            alias, yamlDataType.Name);
                        continue;
                    }

                    blockDict.Remove("contentElementTypeAlias");
                    blockDict["contentElementTypeKey"] = contentType.Key.ToString();
                    changed = true;

                    _logger?.LogInformation(
                        "Resolved contentElementTypeAlias '{Alias}' → '{Key}' for DataType '{Name}'.",
                        alias, contentType.Key, yamlDataType.Name);
                }

                if (!changed) continue;

                var existing = _dataTypeService.GetDataType(yamlDataType.Name);
                if (existing is DataType dt)
                {
#if NET10_0_OR_GREATER
                    dt.SetConfigurationData(yamlDataType.Config);
#endif
                    _dataTypeService.Save(dt, Constants.Security.SuperUserId);
                    _logger?.LogInformation(
                        "DataType '{Name}' saved with resolved Block List element type keys.",
                        yamlDataType.Name);
                }
            }
        }

        /// <summary>Converts an IDictionary with object keys to a string-keyed Dictionary.</summary>
        private static Dictionary<string, object>? NormaliseDictKeys(IDictionary? source)
        {
            if (source == null) return null;
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (DictionaryEntry entry in source)
                result[entry.Key?.ToString() ?? string.Empty] = entry.Value!;
            return result;
        }

        /// <summary>
        /// Normalises the config dict in-place before passing to SetConfigurationData.
        ///
        /// ValueListConfiguration.Items (Umbraco.DropDown.Flexible, CheckBoxList, etc.) is
        /// List&lt;string&gt; in Umbraco 17. YamlDotNet deserialises YAML string sequences as
        /// List&lt;object&gt;. Umbraco's ValueListConfigurationEditor validator performs a strict
        /// <c>is List&lt;string&gt;</c> check — List&lt;object&gt; is rejected even when every element
        /// is a string. We must convert in-place so the correct concrete type is stored.
        ///
        /// For other editors that use an "items" list in {id, value} format the items are
        /// already dicts (not plain strings), so the conversion is skipped and their structure
        /// is left unchanged.
        /// </summary>
        private static void ApplyConfig(Dictionary<string, object> config)
        {
            if (!config.TryGetValue("items", out var raw)) return;
            if (raw is not List<object> items) return;
            if (items.Count == 0 || items[0] is not string) return;

            // Convert List<object> of plain strings → List<string> so Umbraco's
            // ValueListConfigurationEditor validator accepts it.
            config["items"] = items
                .Select(item => item?.ToString() ?? string.Empty)
                .ToList();
        }
    }
}
