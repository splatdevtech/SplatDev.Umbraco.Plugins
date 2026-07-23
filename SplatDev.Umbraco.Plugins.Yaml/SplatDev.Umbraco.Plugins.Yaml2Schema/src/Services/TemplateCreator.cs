using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

// ITemplateService was introduced in Umbraco 14 (new backoffice).
// Umbraco 13 (net8.0 target) uses IFileService for template CRUD operations.
#if NET8_0
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
#endif

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    public class TemplateCreator
    {
#if NET8_0
        // Umbraco 13: IFileService handles templates; IShortStringHelper is needed for new Template()
        private readonly IFileService _fileService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly ILogger<TemplateCreator>? _logger;

        public TemplateCreator(
            IFileService fileService,
            IShortStringHelper shortStringHelper,
            ILogger<TemplateCreator>? logger = null)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _shortStringHelper = shortStringHelper ?? throw new ArgumentNullException(nameof(shortStringHelper));
            _logger = logger;
        }
#else
        // Umbraco 14+ / 17: dedicated ITemplateService
        private readonly ITemplateService _templateService;
        private readonly ILogger<TemplateCreator>? _logger;

        public TemplateCreator(ITemplateService templateService, ILogger<TemplateCreator>? logger = null)
        {
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
            _logger = logger;
        }
#endif

        public void CreateTemplates(List<YamlTemplate> templates)
        {
            if (templates == null)
            {
                throw new ArgumentNullException(nameof(templates));
            }

            var processedAliases = new HashSet<string>();

            foreach (var yamlTemplate in templates)
            {
                try
                {
                    // Skip if alias has already been processed in this batch
                    if (processedAliases.Contains(yamlTemplate.Alias))
                    {
                        _logger?.LogWarning(
                            "Template with alias '{Alias}' is a duplicate and will be skipped.",
                            yamlTemplate.Alias
                        );
                        continue;
                    }

#if NET8_0
                    // Umbraco 13: use IFileService for template operations
                    // [UPDATE]
                    if (yamlTemplate.Update)
                    {
                        var toUpdate = _fileService.GetTemplate(yamlTemplate.Alias);
                        if (toUpdate != null)
                        {
                            toUpdate.Content = !string.IsNullOrWhiteSpace(yamlTemplate.RazorContent)
                                ? yamlTemplate.RazorContent
                                : GenerateDefaultTemplateContent(yamlTemplate.Name, yamlTemplate.Scripts, yamlTemplate.Stylesheets);
                            _fileService.SaveTemplate(toUpdate);
                            _logger?.LogInformation(
                                "Template '{Name}' with alias '{Alias}' updated.",
                                yamlTemplate.Name, yamlTemplate.Alias);
                            processedAliases.Add(yamlTemplate.Alias);
                            continue;
                        }
                        _logger?.LogInformation(
                            "Template '{Alias}' not found during UPDATE; will create it.",
                            yamlTemplate.Alias);
                    }

                    // [REMOVE]
                    if (yamlTemplate.Remove)
                    {
                        var toDelete = _fileService.GetTemplate(yamlTemplate.Alias);
                        if (toDelete != null)
                        {
                            _fileService.DeleteTemplate(yamlTemplate.Alias);
                            _logger?.LogInformation(
                                "Template '{Name}' with alias '{Alias}' removed.",
                                yamlTemplate.Name, yamlTemplate.Alias);
                        }
                        else
                        {
                            _logger?.LogDebug(
                                "Template with alias '{Alias}' not found for removal. Skipping.",
                                yamlTemplate.Alias);
                        }
                        processedAliases.Add(yamlTemplate.Alias);
                        continue;
                    }

                    var existingTemplate = _fileService.GetTemplate(yamlTemplate.Alias);
                    if (existingTemplate != null)
                    {
                        _logger?.LogInformation(
                            "Template with alias '{Alias}' already exists. Skipping.",
                            yamlTemplate.Alias
                        );
                        processedAliases.Add(yamlTemplate.Alias);
                        continue;
                    }

                    var fileContent13 = !string.IsNullOrWhiteSpace(yamlTemplate.RazorContent)
                        ? yamlTemplate.RazorContent
                        : GenerateDefaultTemplateContent(yamlTemplate.Name, yamlTemplate.Scripts, yamlTemplate.Stylesheets);

                    var newTemplate = new Template(_shortStringHelper, yamlTemplate.Name, yamlTemplate.Alias)
                    {
                        Content = fileContent13
                    };
                    _fileService.SaveTemplate(newTemplate);
#else
                    // Umbraco 14+ / 17: use ITemplateService
                    // [UPDATE] — update if exists, create if not found
                    if (yamlTemplate.Update)
                    {
                        var toUpdate = _templateService.GetAsync(yamlTemplate.Alias).GetAwaiter().GetResult();
                        if (toUpdate != null)
                        {
                            toUpdate.Content = !string.IsNullOrWhiteSpace(yamlTemplate.RazorContent)
                                ? yamlTemplate.RazorContent
                                : GenerateDefaultTemplateContent(yamlTemplate.Name, yamlTemplate.Scripts, yamlTemplate.Stylesheets);
                            _templateService.UpdateAsync(toUpdate, Guid.Empty).GetAwaiter().GetResult();
                            _logger?.LogInformation(
                                "Template '{Name}' with alias '{Alias}' updated.",
                                yamlTemplate.Name, yamlTemplate.Alias);
                            processedAliases.Add(yamlTemplate.Alias);
                            continue;
                        }
                        // Not found — fall through to creation below
                        _logger?.LogInformation(
                            "Template '{Alias}' not found during UPDATE; will create it.",
                            yamlTemplate.Alias);
                    }

                    // [REMOVE] — delete the Template if flagged
                    if (yamlTemplate.Remove)
                    {
                        var toDelete = _templateService.GetAsync(yamlTemplate.Alias).GetAwaiter().GetResult();
                        if (toDelete != null)
                        {
                            _templateService.DeleteAsync(toDelete.Key, Guid.Empty).GetAwaiter().GetResult();
                            _logger?.LogInformation(
                                "Template '{Name}' with alias '{Alias}' removed.",
                                yamlTemplate.Name, yamlTemplate.Alias);
                        }
                        else
                        {
                            _logger?.LogDebug(
                                "Template with alias '{Alias}' not found for removal. Skipping.",
                                yamlTemplate.Alias);
                        }
                        processedAliases.Add(yamlTemplate.Alias);
                        continue;
                    }

                    // Check if Template already exists in the system
                    var existingTemplate = _templateService.GetAsync(yamlTemplate.Alias).GetAwaiter().GetResult();
                    if (existingTemplate != null)
                    {
                        _logger?.LogInformation(
                            "Template with alias '{Alias}' already exists. Skipping.",
                            yamlTemplate.Alias
                        );
                        processedAliases.Add(yamlTemplate.Alias);
                        continue;
                    }

                    // Use explicit Razor content if provided, otherwise generate a default scaffold
                    var fileContent = !string.IsNullOrWhiteSpace(yamlTemplate.RazorContent)
                        ? yamlTemplate.RazorContent
                        : GenerateDefaultTemplateContent(yamlTemplate.Name, yamlTemplate.Scripts, yamlTemplate.Stylesheets);

                    // Create new Template via service
                    _templateService.CreateAsync(yamlTemplate.Name, yamlTemplate.Alias, fileContent, Guid.Empty, null)
                        .GetAwaiter().GetResult();
#endif

                    _logger?.LogInformation(
                        "Template '{Name}' with alias '{Alias}' created successfully.",
                        yamlTemplate.Name,
                        yamlTemplate.Alias
                    );

                    processedAliases.Add(yamlTemplate.Alias);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(
                        ex,
                        "Error creating Template '{Name}' with alias '{Alias}'.",
                        yamlTemplate.Name,
                        yamlTemplate.Alias
                    );
                    throw;
                }
            }
        }

        private string GenerateDefaultTemplateContent(string templateName, List<string>? scripts = null, List<string>? stylesheets = null)
        {
            var stylesheetTags = BuildStylesheetTags(stylesheets);
            var scriptTags = BuildScriptTags(scripts);

            return $$"""
@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage
@{
    Layout = null;
}

<!DOCTYPE html>
<html>
<head>
    <title>{{templateName}}</title>
{{stylesheetTags}}</head>
<body>
    <h1>{{templateName}}</h1>
    <main>
        @Html.Raw(Model.Value("bodyText"))
    </main>
{{scriptTags}}</body>
</html>
""";
        }

        private static string BuildStylesheetTags(List<string>? stylesheets)
        {
            if (stylesheets == null || stylesheets.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var path in stylesheets.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                var href = path.StartsWith('/') ? path : $"/{path}";
                sb.AppendLine($"    <link rel=\"stylesheet\" href=\"{href}\" />");
            }
            return sb.ToString();
        }

        private static string BuildScriptTags(List<string>? scripts)
        {
            if (scripts == null || scripts.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var path in scripts.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                var src = path.StartsWith('/') ? path : $"/{path}";
                sb.AppendLine($"    <script src=\"{src}\"></script>");
            }
            return sb.ToString();
        }
    }
}
