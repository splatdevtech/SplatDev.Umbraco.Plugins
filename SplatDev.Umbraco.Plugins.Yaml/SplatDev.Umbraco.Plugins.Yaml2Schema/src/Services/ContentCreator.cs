using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    public class ContentCreator
    {
        private readonly IContentService _contentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IMediaService _mediaService;
        private readonly ILogger<ContentCreator>? _logger;

        public ContentCreator(
            IContentService contentService,
            IContentTypeService contentTypeService,
            IMediaService mediaService,
            ILogger<ContentCreator>? logger = null)
        {
            _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
            _contentTypeService = contentTypeService ?? throw new ArgumentNullException(nameof(contentTypeService));
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
            _logger = logger;
        }

        public void CreateContent(List<YamlContent> contentList, int? parentId = null)
        {
            foreach (var yamlContent in contentList)
            {
                try
                {
                    // [UPDATE] — update if exists, skip if not found
                    if (yamlContent.Update)
                    {
                        var candidates = parentId.HasValue
                            ? _contentService.GetPagedChildren(parentId.Value, 0, int.MaxValue, out _).ToList()
                            : _contentService.GetRootContent().ToList();

                        var toUpdate = candidates.FirstOrDefault(c => c.Name == yamlContent.Name);
                        if (toUpdate != null)
                        {
                            // Restore missing template — content created before v1.0.28 may have TemplateId = null
                            if (!toUpdate.TemplateId.HasValue || toUpdate.TemplateId.Value == 0)
                            {
                                var ct = _contentTypeService.Get(toUpdate.ContentType.Alias);
                                if (ct != null && ct.DefaultTemplateId > 0)
                                    toUpdate.TemplateId = ct.DefaultTemplateId;
                            }

                            foreach (var kvp in yamlContent.Values)
                            {
                                var prop = toUpdate.Properties.FirstOrDefault(p => p.Alias == kvp.Key);
                                if (prop != null)
                                    toUpdate.SetValue(kvp.Key, CoerceValue(kvp.Value, prop));
                            }

                            toUpdate.SortOrder = yamlContent.SortOrder;
                            _contentService.Save(toUpdate, null, null);

                            if (yamlContent.Published)
                                #if NET10_0_OR_GREATER
                                _contentService.Publish(toUpdate, Array.Empty<string>(), Constants.Security.SuperUserId);
#else
                                _contentService.SaveAndPublish(toUpdate, userId: Constants.Security.SuperUserId);
#endif

                            _logger?.LogInformation("Content '{Name}' updated.", yamlContent.Name);

                            if (yamlContent.Children?.Any() == true)
                                CreateContent(yamlContent.Children, toUpdate.Id);
                        }
                        else
                        {
                            _logger?.LogInformation("Content '{Name}' not found. Skipping update.", yamlContent.Name);
                        }
                        continue;
                    }

                    // [REMOVE] — delete matching content node if flagged
                    if (yamlContent.Remove)
                    {
                        var candidates = parentId.HasValue
                            ? _contentService.GetPagedChildren(parentId.Value, 0, int.MaxValue, out _).ToList()
                            : _contentService.GetRootContent().ToList();

                        var toDelete = candidates.FirstOrDefault(c => c.Name == yamlContent.Name);
                        if (toDelete != null)
                        {
                            _contentService.Delete(toDelete, Constants.Security.SuperUserId);
                            _logger?.LogInformation("Content '{Name}' removed.", yamlContent.Name);
                        }
                        else
                        {
                            _logger?.LogDebug("Content '{Name}' not found for removal. Skipping.", yamlContent.Name);
                        }
                        // Deletion cascades in Umbraco — no need to recurse into children
                        continue;
                    }

                    var contentType = _contentTypeService.Get(yamlContent.Type);
                    if (contentType == null)
                    {
                        _logger?.LogError("DocumentType not found: {Type}", yamlContent.Type);
                        continue;
                    }

                    var content = _contentService.Create(yamlContent.Name, parentId ?? -1, contentType.Alias, Constants.Security.SuperUserId);

                    // IContentService.Create does not auto-assign the document type's default template.
                    // Explicitly set it so the content node is renderable immediately after creation.
                    if (contentType.DefaultTemplateId > 0)
                        content.TemplateId = contentType.DefaultTemplateId;

                    // Set property values
                    foreach (var kvp in yamlContent.Values)
                    {
                        var prop = content.Properties.FirstOrDefault(p => p.Alias == kvp.Key);
                        if (prop != null)
                            content.SetValue(kvp.Key, CoerceValue(kvp.Value, prop));
                    }

                    content.SortOrder = yamlContent.SortOrder;

                    _contentService.Save(content, null, null);

                    if (yamlContent.Published)
                    {
#if NET10_0_OR_GREATER
                        _contentService.Publish(content, Array.Empty<string>(), Constants.Security.SuperUserId);
#else
                        _contentService.SaveAndPublish(content, userId: Constants.Security.SuperUserId);
#endif
                    }

                    _logger?.LogInformation("Created Content: {Alias}", yamlContent.Alias);

                    // Recursively create children
                    if (yamlContent.Children?.Any() == true)
                    {
                        CreateContent(yamlContent.Children, content.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error creating content {Alias}", yamlContent.Alias);
                    throw;
                }
            }
        }
        /// <summary>
        /// Converts YAML-deserialized values to the type expected by the Umbraco property editor:
        /// <list type="bullet">
        ///   <item>String "true"/"false" → 1/0 for Integer storage (e.g. Umbraco.TrueFalse).</item>
        ///   <item>Numeric strings → int/decimal for Integer/Decimal storage.</item>
        ///   <item>A list whose items are mappings with a <c>$type</c> key → Block List JSON string.</item>
        ///   <item>Any other complex value (list or mapping) stored as Ntext → serialized to JSON string.</item>
        /// </list>
        /// </summary>
        private object CoerceValue(object value, IProperty property)
        {
            // ── MultiUrlPicker: mapping with url + optional title/name ───────────────
            // YAML:
            //   myUrlProp:
            //     url: "/parceiros"
            //     title: "Visit Partners"   # saved as the link name
            //     target: "_blank"          # optional
            if (value is IDictionary urlDict
                && property.PropertyType.PropertyEditorAlias == "Umbraco.MultiUrlPicker")
            {
                var d = NormaliseDictKeys(urlDict);
                if (d != null && d.TryGetValue("url", out var rawUrl) && rawUrl != null)
                {
                    d.TryGetValue("title", out var rawTitle);
                    d.TryGetValue("name", out var rawName);
                    d.TryGetValue("target", out var rawTarget);
                    var link = new Dictionary<string, object?>
                    {
                        ["name"]        = (rawTitle ?? rawName)?.ToString() ?? "",
                        ["target"]      = rawTarget?.ToString() ?? "",
                        ["udi"]         = null,
                        ["url"]         = rawUrl.ToString()!,
                        ["queryString"] = ""
                    };
                    return JsonSerializer.Serialize(new[] { link });
                }
            }

            // ── Flexible Dropdown: plain string → JSON array ─────────────────────────
            // Umbraco.DropDown.Flexible stores its value as a JSON array ["selectedValue"].
            // Seeding a plain string like "Commerce" must be wrapped to avoid a JsonException
            // on read ("'C' is an invalid start of a value").
            if (value is string dropVal
                && property.PropertyType.PropertyEditorAlias == "Umbraco.DropDown.Flexible"
                && !dropVal.TrimStart().StartsWith('['))
            {
                return JsonSerializer.Serialize(new[] { dropVal });
            }

            // ── MediaPicker3: media name string → JSON reference ─────────────────────
            // YAML:
            //   heroImage: "RISIN – Home Hero"   # looks up media by name, creates picker JSON
            if (value is string mediaName
                && property.PropertyType.PropertyEditorAlias == "Umbraco.MediaPicker3")
            {
                var media = _mediaService.GetPagedDescendants(-1, 0, int.MaxValue, out _)
                    .FirstOrDefault(m => string.Equals(m.Name, mediaName, StringComparison.OrdinalIgnoreCase));
                if (media != null)
                {
                    var pickerEntry = new Dictionary<string, object?>
                    {
                        ["key"]        = Guid.NewGuid().ToString(),
                        ["mediaKey"]   = media.Key.ToString(),
                        ["crops"]      = Array.Empty<object>(),
                        ["focalPoint"] = null
                    };
                    return JsonSerializer.Serialize(new[] { pickerEntry });
                }
                _logger?.LogWarning("Media item '{Name}' not found for MediaPicker3 property '{Alias}'. Leaving value as-is.", mediaName, property.Alias);
                return value;
            }

            // ── Scalar string coercion ────────────────────────────────────────────────
            if (value is string strVal)
            {
                // MultiUrlPicker: wrap a plain URL string in the JSON array format Umbraco expects.
                // Seed values like "/my-page" are stored as [{"url":"/my-page",...}] by the editor.
                // Skip if the value already looks like JSON (starts with '[') to avoid double-encoding.
                if (!string.IsNullOrWhiteSpace(strVal)
                    && property.PropertyType.PropertyEditorAlias == "Umbraco.MultiUrlPicker"
                    && !strVal.TrimStart().StartsWith('['))
                {
                    var link = new Dictionary<string, object?>
                    {
                        ["name"]        = "",
                        ["target"]      = "",
                        ["udi"]         = null,
                        ["url"]         = strVal,
                        ["queryString"] = ""
                    };
                    return JsonSerializer.Serialize(new[] { link });
                }

                return property.PropertyType.ValueStorageType switch
                {
                    ValueStorageType.Integer => strVal.ToLowerInvariant() switch
                    {
                        "true"  => (object)1,
                        "false" => (object)0,
                        _       => int.TryParse(strVal, out var i) ? (object)i : value
                    },
                    ValueStorageType.Decimal => decimal.TryParse(
                        strVal,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var d) ? (object)d : value,
                    _ => value
                };
            }

            // ── Single Block: a mapping with $type key ────────────────────────────────
            // YAML:
            //   myBlockProp:
            //     $type: myElementTypeAlias
            //     title: "Block title"
            //     text:  "Block text"
            if (value is IDictionary singleBlockDict
                && property.PropertyType.ValueStorageType == ValueStorageType.Ntext)
            {
                var normSingle = NormaliseDictKeys(singleBlockDict);
                if (normSingle != null && normSingle.ContainsKey("$type"))
                    return BuildSingleBlockJson(normSingle);
            }

            // ── MultipleTextstring: list of strings joined with newline ───────────────
            // Umbraco.MultipleTextstring stores its value as \n-delimited plain text, not JSON.
            // YAML:
            //   myProp:
            //     - "First item"
            //     - "Second item"
            if (value is List<object> textItems
                && property.PropertyType.PropertyEditorAlias == "Umbraco.MultipleTextstring")
            {
                return string.Join("\n", textItems.Select(i => i?.ToString() ?? ""));
            }

            // ── Block List: list of mappings with $type key ───────────────────────────
            // YAML:
            //   myBlockProp:
            //     - $type: myElementTypeAlias
            //       title: "Block 1 title"
            //       text:  "Block 1 text"
            if (value is List<object> blockItems
                && blockItems.Count > 0
                && IsBlockListItems(blockItems)
                && property.PropertyType.ValueStorageType == ValueStorageType.Ntext)
            {
                return BuildBlockListJson(blockItems);
            }

            // ── Generic complex value (list or mapping) stored as Ntext → JSON string ─
            if (property.PropertyType.ValueStorageType == ValueStorageType.Ntext
                && value is not string
                && (value is IList || value is IDictionary))
            {
                return JsonSerializer.Serialize(NormaliseForJson(value));
            }

            return value;
        }

        // ── Block List helpers ────────────────────────────────────────────────────────

        private static bool IsBlockListItems(List<object> list) =>
            list.Any(item =>
            {
                if (item is IDictionary d)
                {
                    foreach (var key in d.Keys)
                        if (key?.ToString() == "$type") return true;
                }
                return false;
            });

        /// <summary>
        /// Builds the Umbraco Block List JSON value from a list of YAML block items.
        /// Each item must be a mapping with a <c>$type</c> key whose value is the element
        /// document-type alias. All other keys become property values on the block.
        /// </summary>
        private string BuildBlockListJson(List<object> items)
        {
            var layoutItems    = new List<Dictionary<string, object?>>();
            var contentData    = new List<Dictionary<string, object?>>();

            foreach (var raw in items)
            {
                var dict = NormaliseDictKeys(raw as IDictionary);
                if (dict == null) continue;

                if (!dict.TryGetValue("$type", out var rawAlias) || rawAlias == null)
                    continue;

                var alias = rawAlias.ToString()!;
                var contentType = _contentTypeService.Get(alias);
                if (contentType == null)
                {
                    _logger?.LogWarning("Block list element type '{Alias}' not found. Block skipped.", alias);
                    continue;
                }

                var udi = $"umb://element/{Guid.NewGuid():N}";

                layoutItems.Add(new Dictionary<string, object?> { ["contentUdi"] = udi });

                var blockEntry = new Dictionary<string, object?>
                {
                    ["contentTypeKey"] = contentType.Key.ToString(),
                    ["udi"]            = udi,
                };

                foreach (var kvp in dict)
                {
                    if (kvp.Key == "$type") continue;
                    blockEntry[kvp.Key] = kvp.Value;
                }

                contentData.Add(blockEntry);
            }

            var blockListValue = new Dictionary<string, object>
            {
                ["layout"]      = new Dictionary<string, object> { ["Umbraco.BlockList"] = layoutItems },
                ["contentData"] = contentData,
                ["settingsData"] = new List<object>(),
            };

            return JsonSerializer.Serialize(blockListValue);
        }

        /// <summary>
        /// Builds the Umbraco Single Block JSON value from a YAML mapping with a <c>$type</c> key.
        /// The layout uses an object (not an array) keyed by <c>Umbraco.SingleBlock</c>.
        /// </summary>
        private string BuildSingleBlockJson(Dictionary<string, object> item)
        {
            if (!item.TryGetValue("$type", out var rawAlias) || rawAlias == null)
                return JsonSerializer.Serialize(NormaliseForJson(item));

            var alias = rawAlias.ToString()!;
            var contentType = _contentTypeService.Get(alias);
            if (contentType == null)
            {
                _logger?.LogWarning("Single block element type '{Alias}' not found. Block skipped.", alias);
                return JsonSerializer.Serialize(NormaliseForJson(item));
            }

            var udi = $"umb://element/{Guid.NewGuid():N}";

            var blockEntry = new Dictionary<string, object?>
            {
                ["contentTypeKey"] = contentType.Key.ToString(),
                ["udi"]            = udi,
            };

            foreach (var kvp in item)
            {
                if (kvp.Key == "$type") continue;
                blockEntry[kvp.Key] = kvp.Value;
            }

            var singleBlockValue = new Dictionary<string, object>
            {
                ["layout"]       = new Dictionary<string, object>
                {
                    ["Umbraco.SingleBlock"] = new Dictionary<string, object?> { ["contentUdi"] = udi }
                },
                ["contentData"]  = new List<Dictionary<string, object?>> { blockEntry },
                ["settingsData"] = new List<object>(),
            };

            return JsonSerializer.Serialize(singleBlockValue);
        }

        // ── Generic JSON helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Recursively converts YamlDotNet's <c>Dictionary&lt;object,object&gt;</c> and nested
        /// collections to JSON-serialisable equivalents with string keys.
        /// </summary>
        private static object NormaliseForJson(object value) =>
            value switch
            {
                IDictionary dict => NormaliseDictKeys(dict)!
                    .ToDictionary(kvp => kvp.Key, kvp => NormaliseForJson(kvp.Value!)),
                IList list when value is not string => list.Cast<object?>()
                    .Select(item => item == null ? (object?)null : NormaliseForJson(item))
                    .ToList(),
                _ => value
            };

        private static Dictionary<string, object>? NormaliseDictKeys(IDictionary? source)
        {
            if (source == null) return null;
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (DictionaryEntry entry in source)
                result[entry.Key?.ToString() ?? string.Empty] = entry.Value!;
            return result;
        }
    }
}
