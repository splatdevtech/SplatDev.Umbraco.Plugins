using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    /// <summary>
    /// Generates typed C# published-model partial classes from YAML-defined document types.
    /// Output is written as <c>.cs</c> files to the configured output path (from the
    /// <c>modelsBuilder.outputPath</c> YAML setting, or <c>Models/GeneratedModels</c> by default).
    ///
    /// Each non-element document type produces one file containing a
    /// <c>[PublishedModel]</c> partial class that inherits <c>PublishedContentModel</c>.
    /// Element types also get models but are generated as element-typed partials.
    /// </summary>
    public class PublishedModelsGenerator
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<PublishedModelsGenerator>? _logger;

        /// <summary>Maps DataType editor alias → C# property type name.</summary>
        private static readonly Dictionary<string, string> _editorTypemap =
            new(StringComparer.OrdinalIgnoreCase)
        {
            { "Umbraco.TextBox",            "string" },
            { "Umbraco.TextArea",           "string" },
            { "Umbraco.EmailAddress",       "string" },
            { "Umbraco.MarkdownEditor",     "string" },
            { "Umbraco.Label",              "string" },
            { "Umbraco.Integer",            "int?" },
            { "Umbraco.Decimal",            "decimal?" },
            { "Umbraco.Slider",             "decimal?" },
            { "Umbraco.TrueFalse",          "bool?" },
            { "Umbraco.DateTime",           "DateTime?" },
            { "Umbraco.DateOnly",           "DateTime?" },
            { "Umbraco.TimeOnly",           "TimeSpan?" },
            { "Umbraco.RichText",           "HtmlEncodedString?" },
            { "Umbraco.MediaPicker3",       "IEnumerable<MediaWithCrops>?" },
            { "Umbraco.ImageCropper",       "ImageCropperValue?" },
            { "Umbraco.ContentPicker",      "IPublishedContent?" },
            { "Umbraco.MultiNodeTreePicker","IEnumerable<IPublishedContent>?" },
            { "Umbraco.BlockList",          "BlockListModel?" },
            { "Umbraco.BlockGrid",          "BlockGridModel?" },
            { "Umbraco.SingleBlock",        "BlockItemData?" },
            { "Umbraco.DropDown.Flexible",  "string" },
            { "Umbraco.CheckBoxList",       "IEnumerable<string>?" },
            { "Umbraco.RadioButtonList",    "string" },
            { "Umbraco.ColorPicker",        "string" },
            { "Umbraco.Tags",               "IEnumerable<string>?" },
            { "Umbraco.MultiUrlPicker",     "IEnumerable<Link>?" },
            { "Umbraco.MultipleTextstring", "IEnumerable<string>?" },
            { "Umbraco.UploadField",        "string" },
        };

        public PublishedModelsGenerator(
            IHostEnvironment hostEnvironment,
            ILogger<PublishedModelsGenerator>? logger = null)
        {
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _logger = logger;
        }

        public void GenerateModels(
            List<YamlDocumentType> documentTypes,
            List<YamlDataType>? dataTypes = null,
            string? outputPath = null)
        {
            if (documentTypes == null) throw new ArgumentNullException(nameof(documentTypes));

            // Resolve output directory
            var outDir = string.IsNullOrWhiteSpace(outputPath)
                ? Path.Combine(_hostEnvironment.ContentRootPath, "Models", "GeneratedModels")
                : Path.IsPathRooted(outputPath)
                    ? outputPath
                    : Path.Combine(_hostEnvironment.ContentRootPath, outputPath);

            Directory.CreateDirectory(outDir);

            // Build alias → editor map from declared data types so we can map property types
            var editorByAlias = BuildEditorByAliasMap(dataTypes);
            var editorByName = BuildEditorByNameMap(dataTypes);

            foreach (var docType in documentTypes)
            {
                if (docType.Remove || string.IsNullOrWhiteSpace(docType.Alias))
                    continue;

                try
                {
                    var source = GenerateClass(docType, editorByAlias, editorByName);
                    var className = ToPascalCase(docType.Alias);
                    var filePath = Path.Combine(outDir, $"{className}.generated.cs");
                    File.WriteAllText(filePath, source, Encoding.UTF8);
                    _logger?.LogInformation(
                        "PublishedModelsGenerator: Generated model '{ClassName}' → '{FilePath}'.",
                        className, filePath);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex,
                        "PublishedModelsGenerator: Failed to generate model for '{Alias}'.", docType.Alias);
                }
            }
        }

        private string GenerateClass(
            YamlDocumentType docType,
            Dictionary<string, string> editorByAlias,
            Dictionary<string, string> editorByName)
        {
            var className = ToPascalCase(docType.Alias);
            var sb = new StringBuilder();

            sb.AppendLine("// <auto-generated />");
            sb.AppendLine("// Generated by SplatDev.SplatDev.Umbraco.Plugins.Yaml2Schema — do not edit manually.");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using Umbraco.Cms.Core.Models.PublishedContent;");
            sb.AppendLine("using Umbraco.Cms.Web.Common.PublishedModels;");
            sb.AppendLine();
            sb.AppendLine("namespace Umbraco.Web.PublishedModels");
            sb.AppendLine("{");

            if (!string.IsNullOrWhiteSpace(docType.Name))
                sb.AppendLine($"    // {docType.Name}");

            sb.AppendLine($"    [PublishedModel(\"{docType.Alias}\")]");
            sb.AppendLine($"    public partial class {className} : PublishedContentModel");
            sb.AppendLine("    {");
            sb.AppendLine($"        public const string ModelTypeAlias = \"{docType.Alias}\";");
            sb.AppendLine();
            sb.AppendLine($"        public {className}(IPublishedContent content, IPublishedValueFallback publishedValueFallback)");
            sb.AppendLine("            : base(content, publishedValueFallback) { }");

            foreach (var tab in docType.Tabs ?? [])
            {
                foreach (var prop in tab.Properties ?? [])
                {
                    if (string.IsNullOrWhiteSpace(prop.Alias)) continue;

                    var csharpType = ResolveCsharpType(prop.DataType, editorByAlias, editorByName);
                    var propName = ToPascalCase(prop.Alias);

                    // Avoid name collision with the class itself or base members
                    if (propName == className) propName = propName + "Value";

                    sb.AppendLine();
                    if (!string.IsNullOrWhiteSpace(prop.Description))
                        sb.AppendLine($"        /// <summary>{EscapeXml(prop.Description)}</summary>");

                    sb.AppendLine($"        [ImplementPropertyType(\"{prop.Alias}\")]");
                    sb.AppendLine($"        public {csharpType} {propName} =>");
                    sb.AppendLine($"            this.Value<{csharpType}>(publishedValueFallback, \"{prop.Alias}\");");
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Resolves the C# type for a property given its <c>dataType</c> YAML field value,
        /// which may be either a DataType alias or a DataType name.
        /// </summary>
        private string ResolveCsharpType(
            string dataTypeRef,
            Dictionary<string, string> editorByAlias,
            Dictionary<string, string> editorByName)
        {
            if (string.IsNullOrWhiteSpace(dataTypeRef))
                return "object";

            // Try alias lookup first, then name lookup
            string? editorAlias = null;
            if (editorByAlias.TryGetValue(dataTypeRef, out var ea)) editorAlias = ea;
            else if (editorByName.TryGetValue(dataTypeRef, out var en)) editorAlias = en;

            if (editorAlias != null && _editorTypemap.TryGetValue(editorAlias, out var mapped))
                return mapped;

            // Fallback: treat as string for unknown types
            return "string";
        }

        private static Dictionary<string, string> BuildEditorByAliasMap(List<YamlDataType>? dataTypes)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (dataTypes == null) return map;
            foreach (var dt in dataTypes)
            {
                if (!string.IsNullOrWhiteSpace(dt.Alias) && !string.IsNullOrWhiteSpace(dt.Editor))
                    map.TryAdd(dt.Alias, dt.Editor);
            }
            return map;
        }

        private static Dictionary<string, string> BuildEditorByNameMap(List<YamlDataType>? dataTypes)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (dataTypes == null) return map;
            foreach (var dt in dataTypes)
            {
                if (!string.IsNullOrWhiteSpace(dt.Name) && !string.IsNullOrWhiteSpace(dt.Editor))
                    map.TryAdd(dt.Name, dt.Editor);
            }
            return map;
        }

        /// <summary>Converts a camelCase or hyphenated alias to PascalCase class name.</summary>
        private static string ToPascalCase(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias)) return "Unknown";
            var parts = alias.Split(new[] { '-', '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                var s = parts[0];
                return char.ToUpperInvariant(s[0]) + s[1..];
            }
            return string.Concat(parts.Select(p => char.ToUpperInvariant(p[0]) + p[1..]));
        }

        private static string EscapeXml(string s) =>
            s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }
}
