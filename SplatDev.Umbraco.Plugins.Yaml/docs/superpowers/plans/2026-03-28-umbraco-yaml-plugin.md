# Umbraco YAML Plugin Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a complete Umbraco 17 plugin that reads YAML configuration and programmatically creates DataTypes, DocumentTypes, Templates, and Content nodes.

**Architecture:** Declarative YAML-driven (Approach A). A startup composer triggers on Umbraco boot, reads a YAML file, and uses the plugin's service classes to create structures via Umbraco's public APIs. Each service (DataTypeCreator, DocumentTypeCreator, etc.) handles one responsibility. Models use YamlDotNet for deserialization.

**Tech Stack:**
- .NET 10 / C# 13
- Umbraco 17
- YamlDotNet NuGet package
- xUnit for testing

---

## Task 1: Create Umbraco 17 Project and Plugin Structure

**Files:**
- Create: `UmbracoYaml.csproj`
- Create: `.gitignore`
- Create: `src/Composers/YamlStartupComposer.cs` (skeleton)
- Create: `src/Services/` directory
- Create: `src/Models/` directory
- Create: `tests/` directory

- [ ] **Step 1: Initialize .NET project with Umbraco 17**

Run from `/mnt/e/Source/Repos/Umbraco-Yaml/`:
```bash
dotnet new classlib -n UmbracoYaml -f net10.0
cd UmbracoYaml
```

- [ ] **Step 2: Add NuGet dependencies**

```bash
dotnet add package Umbraco.Cms --version 17.0.0
dotnet add package YamlDotNet --version 15.1.0
dotnet add package xunit --version 2.7.0
dotnet add package xunit.runner.visualstudio --version 2.5.6
dotnet add package Moq --version 4.20.0
```

- [ ] **Step 3: Create .gitignore**

```
bin/
obj/
*.user
*.suo
.vs/
node_modules/
dist/
```

- [ ] **Step 4: Create YamlStartupComposer skeleton**

Create `src/Composers/YamlStartupComposer.cs`:
```csharp
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Events;
using Microsoft.Extensions.DependencyInjection;

namespace UmbracoYaml.Composers
{
    public class YamlStartupComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Placeholder for dependency registration
        }
    }
}
```

- [ ] **Step 5: Commit**

```bash
git init
git add .
git commit -m "init: scaffold Umbraco 17 plugin project with dependencies"
```

---

## Task 2: Create YAML Model Classes

**Files:**
- Create: `src/Models/YamlModels.cs`
- Create: `tests/YamlModelsTests.cs`

- [ ] **Step 1: Write unit tests for YAML models**

Create `tests/YamlModelsTests.cs`:
```csharp
using Xunit;
using UmbracoYaml.Models;

namespace UmbracoYaml.Tests
{
    public class YamlModelsTests
    {
        [Fact]
        public void YamlRoot_ShouldDeserializeFromValidYaml()
        {
            var yaml = @"
umbraco:
  dataTypes:
    - alias: textString
      name: Text String
      editor: Umbraco.TextBox
      config:
        maxLength: 255
";
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .Build();

            var root = deserializer.Deserialize<YamlRoot>(yaml);

            Assert.NotNull(root);
            Assert.NotNull(root.Umbraco);
            Assert.Single(root.Umbraco.DataTypes);
            Assert.Equal("textString", root.Umbraco.DataTypes[0].Alias);
        }

        [Fact]
        public void DocumentType_ShouldAllowProperties()
        {
            var yaml = @"
umbraco:
  documentTypes:
    - alias: page
      name: Page
      icon: icon-document
      allowAsRoot: true
      tabs:
        - name: Content
          properties:
            - alias: title
              name: Title
              dataType: textString
              required: true
";
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .Build();

            var root = deserializer.Deserialize<YamlRoot>(yaml);

            Assert.Single(root.Umbraco.DocumentTypes);
            var docType = root.Umbraco.DocumentTypes[0];
            Assert.Equal("page", docType.Alias);
            Assert.Single(docType.Tabs);
            Assert.Single(docType.Tabs[0].Properties);
        }
    }
}
```

- [ ] **Step 2: Create YAML model classes**

Create `src/Models/YamlModels.cs`:
```csharp
using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace UmbracoYaml.Models
{
    public class YamlRoot
    {
        [YamlMember(Alias = "umbraco")]
        public UmbracoConfig Umbraco { get; set; }
    }

    public class UmbracoConfig
    {
        [YamlMember(Alias = "dataTypes")]
        public List<YamlDataType> DataTypes { get; set; } = new();

        [YamlMember(Alias = "documentTypes")]
        public List<YamlDocumentType> DocumentTypes { get; set; } = new();

        [YamlMember(Alias = "templates")]
        public List<YamlTemplate> Templates { get; set; } = new();

        [YamlMember(Alias = "content")]
        public List<YamlContent> Content { get; set; } = new();
    }

    public class YamlDataType
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "editor")]
        public string Editor { get; set; }

        [YamlMember(Alias = "config")]
        public Dictionary<string, object> Config { get; set; } = new();
    }

    public class YamlDocumentType
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "icon")]
        public string Icon { get; set; }

        [YamlMember(Alias = "allowAsRoot")]
        public bool AllowAsRoot { get; set; } = true;

        [YamlMember(Alias = "allowedChildTypes")]
        public List<string> AllowedChildTypes { get; set; } = new();

        [YamlMember(Alias = "tabs")]
        public List<YamlTab> Tabs { get; set; } = new();
    }

    public class YamlTab
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "properties")]
        public List<YamlProperty> Properties { get; set; } = new();
    }

    public class YamlProperty
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "dataType")]
        public string DataType { get; set; }

        [YamlMember(Alias = "required")]
        public bool Required { get; set; } = false;

        [YamlMember(Alias = "description")]
        public string Description { get; set; }
    }

    public class YamlTemplate
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "path")]
        public string Path { get; set; }

        [YamlMember(Alias = "masterTemplate")]
        public string MasterTemplate { get; set; }
    }

    public class YamlContent
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "type")]
        public string Type { get; set; }

        [YamlMember(Alias = "published")]
        public bool Published { get; set; } = false;

        [YamlMember(Alias = "sortOrder")]
        public int SortOrder { get; set; } = 0;

        [YamlMember(Alias = "values")]
        public Dictionary<string, object> Values { get; set; } = new();

        [YamlMember(Alias = "children")]
        public List<YamlContent> Children { get; set; } = new();
    }
}
```

- [ ] **Step 3: Run tests to verify they pass**

```bash
dotnet test tests/YamlModelsTests.cs -v
```

Expected: PASS (2 passing tests)

- [ ] **Step 4: Commit**

```bash
git add src/Models/YamlModels.cs tests/YamlModelsTests.cs
git commit -m "feat: define YAML model classes with YamlDotNet attributes"
```

---

## Task 3: Create YamlParser Service

**Files:**
- Create: `src/Services/YamlParser.cs`
- Create: `tests/YamlParserTests.cs`
- Create: `tests/fixtures/sample.yaml`

- [ ] **Step 1: Write failing test for YamlParser**

Create `tests/YamlParserTests.cs`:
```csharp
using Xunit;
using UmbracoYaml.Services;
using UmbracoYaml.Models;
using System.IO;

namespace UmbracoYaml.Tests
{
    public class YamlParserTests
    {
        [Fact]
        public void ParseYaml_ShouldReadAndDeserializeValidFile()
        {
            var yamlPath = "tests/fixtures/sample.yaml";
            var parser = new YamlParser();

            var result = parser.ParseYaml(yamlPath);

            Assert.NotNull(result);
            Assert.NotNull(result.Umbraco);
            Assert.NotEmpty(result.Umbraco.DataTypes);
        }

        [Fact]
        public void ParseYaml_ShouldThrowOnMissingFile()
        {
            var parser = new YamlParser();

            Assert.Throws<FileNotFoundException>(() =>
                parser.ParseYaml("does-not-exist.yaml"));
        }

        [Fact]
        public void ParseYaml_ShouldThrowOnInvalidYaml()
        {
            var yamlPath = "tests/fixtures/invalid.yaml";
            File.WriteAllText(yamlPath, "{ invalid: yaml: syntax: [");

            var parser = new YamlParser();

            Assert.Throws<YamlDotNet.Core.YamlException>(() =>
                parser.ParseYaml(yamlPath));

            File.Delete(yamlPath);
        }
    }
}
```

- [ ] **Step 2: Create sample YAML fixture**

Create `tests/fixtures/sample.yaml`:
```yaml
umbraco:
  dataTypes:
    - alias: textString
      name: Text String
      editor: Umbraco.TextBox
      config:
        maxLength: 255
    - alias: richText
      name: Rich Text Editor
      editor: Umbraco.RichText
      config:
        toolbar: "bold|italic|underline|link"

  documentTypes:
    - alias: page
      name: Page
      icon: icon-document
      allowAsRoot: true
      tabs:
        - name: Content
          properties:
            - alias: title
              name: Title
              dataType: textString
              required: true
            - alias: body
              name: Body
              dataType: richText
              required: false

  templates:
    - alias: page
      name: Page
      path: Views/Page.cshtml

  content:
    - alias: home
      name: Home
      type: page
      published: true
      values:
        title: Welcome
        body: "<p>Welcome to our site</p>"
```

- [ ] **Step 3: Implement YamlParser**

Create `src/Services/YamlParser.cs`:
```csharp
using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using UmbracoYaml.Models;

namespace UmbracoYaml.Services
{
    public class YamlParser
    {
        public YamlRoot ParseYaml(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"YAML file not found: {filePath}");
            }

            var content = File.ReadAllText(filePath);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            try
            {
                var root = deserializer.Deserialize<YamlRoot>(content);
                return root ?? throw new InvalidOperationException("YAML deserializer returned null");
            }
            catch (YamlDotNet.Core.YamlException ex)
            {
                throw new YamlDotNet.Core.YamlException($"Failed to parse YAML: {ex.Message}", ex);
            }
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test tests/YamlParserTests.cs -v
```

Expected: PASS (3 passing tests)

- [ ] **Step 5: Commit**

```bash
git add src/Services/YamlParser.cs tests/YamlParserTests.cs tests/fixtures/sample.yaml
git commit -m "feat: implement YamlParser service with file reading and deserialization"
```

---

## Task 4: Create DataTypeCreator Service

**Files:**
- Create: `src/Services/DataTypeCreator.cs`
- Create: `tests/DataTypeCreatorTests.cs`

- [ ] **Step 1: Write failing test for DataTypeCreator**

Create `tests/DataTypeCreatorTests.cs`:
```csharp
using Xunit;
using Moq;
using UmbracoYaml.Services;
using UmbracoYaml.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using System.Collections.Generic;

namespace UmbracoYaml.Tests
{
    public class DataTypeCreatorTests
    {
        [Fact]
        public void CreateDataTypes_ShouldCreateDataTypesFromYaml()
        {
            var mockDataTypeService = new Mock<IDataTypeService>();
            var creator = new DataTypeCreator(mockDataTypeService.Object);

            var dataTypes = new List<YamlDataType>
            {
                new YamlDataType
                {
                    Alias = "textString",
                    Name = "Text String",
                    Editor = "Umbraco.TextBox",
                    Config = new() { { "maxLength", 255 } }
                }
            };

            creator.CreateDataTypes(dataTypes);

            mockDataTypeService.Verify(x =>
                x.Save(It.IsAny<IDataType>()), Times.Once);
        }

        [Fact]
        public void CreateDataTypes_ShouldSkipDuplicateAliases()
        {
            var mockDataTypeService = new Mock<IDataTypeService>();
            mockDataTypeService
                .Setup(x => x.GetDataTypeByEditorAlias(It.IsAny<string>()))
                .Returns((IDataType)null);

            var creator = new DataTypeCreator(mockDataTypeService.Object);

            var dataTypes = new List<YamlDataType>
            {
                new YamlDataType { Alias = "textString", Name = "Text String", Editor = "Umbraco.TextBox" },
                new YamlDataType { Alias = "textString", Name = "Text String", Editor = "Umbraco.TextBox" }
            };

            creator.CreateDataTypes(dataTypes);

            mockDataTypeService.Verify(x =>
                x.Save(It.IsAny<IDataType>()), Times.Once);
        }
    }
}
```

- [ ] **Step 2: Implement DataTypeCreator**

Create `src/Services/DataTypeCreator.cs`:
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Microsoft.Extensions.Logging;
using UmbracoYaml.Models;

namespace UmbracoYaml.Services
{
    public class DataTypeCreator
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly ILogger<DataTypeCreator> _logger;

        public DataTypeCreator(IDataTypeService dataTypeService, ILogger<DataTypeCreator> logger = null)
        {
            _dataTypeService = dataTypeService ?? throw new ArgumentNullException(nameof(dataTypeService));
            _logger = logger;
        }

        public void CreateDataTypes(List<YamlDataType> dataTypes)
        {
            var processedAliases = new HashSet<string>();

            foreach (var yamlDataType in dataTypes)
            {
                if (processedAliases.Contains(yamlDataType.Alias))
                {
                    _logger?.LogWarning($"Skipping duplicate DataType alias: {yamlDataType.Alias}");
                    continue;
                }

                try
                {
                    var existing = _dataTypeService.GetDataTypeByEditorAlias(yamlDataType.Editor);
                    if (existing != null && existing.Alias == yamlDataType.Alias)
                    {
                        _logger?.LogInformation($"DataType already exists: {yamlDataType.Alias}");
                        processedAliases.Add(yamlDataType.Alias);
                        continue;
                    }

                    var dataType = new DataType(yamlDataType.Editor)
                    {
                        Name = yamlDataType.Name,
                        Alias = yamlDataType.Alias
                    };

                    _dataTypeService.Save(dataType);
                    processedAliases.Add(yamlDataType.Alias);
                    _logger?.LogInformation($"Created DataType: {yamlDataType.Alias}");
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"Error creating DataType {yamlDataType.Alias}: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
```

- [ ] **Step 3: Run tests to verify they pass**

```bash
dotnet test tests/DataTypeCreatorTests.cs -v
```

Expected: PASS (2 passing tests)

- [ ] **Step 4: Commit**

```bash
git add src/Services/DataTypeCreator.cs tests/DataTypeCreatorTests.cs
git commit -m "feat: implement DataTypeCreator service for programmatic DataType creation"
```

---

## Task 5: Create DocumentTypeCreator Service

**Files:**
- Create: `src/Services/DocumentTypeCreator.cs`
- Create: `tests/DocumentTypeCreatorTests.cs`

- [ ] **Step 1: Write failing test for DocumentTypeCreator**

Create `tests/DocumentTypeCreatorTests.cs`:
```csharp
using Xunit;
using Moq;
using UmbracoYaml.Services;
using UmbracoYaml.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using System.Collections.Generic;

namespace UmbracoYaml.Tests
{
    public class DocumentTypeCreatorTests
    {
        [Fact]
        public void CreateDocumentTypes_ShouldCreateFromYaml()
        {
            var mockDocTypeService = new Mock<IContentTypeService>();
            var mockDataTypeService = new Mock<IDataTypeService>();
            var creator = new DocumentTypeCreator(mockDocTypeService.Object, mockDataTypeService.Object);

            var documentTypes = new List<YamlDocumentType>
            {
                new YamlDocumentType
                {
                    Alias = "page",
                    Name = "Page",
                    Icon = "icon-document",
                    AllowAsRoot = true,
                    Tabs = new List<YamlTab>
                    {
                        new YamlTab
                        {
                            Name = "Content",
                            Properties = new List<YamlProperty>
                            {
                                new YamlProperty { Alias = "title", Name = "Title", DataType = "textString", Required = true }
                            }
                        }
                    }
                }
            };

            creator.CreateDocumentTypes(documentTypes);

            mockDocTypeService.Verify(x =>
                x.Save(It.IsAny<IContentType>()), Times.Once);
        }
    }
}
```

- [ ] **Step 2: Implement DocumentTypeCreator**

Create `src/Services/DocumentTypeCreator.cs`:
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using Microsoft.Extensions.Logging;
using UmbracoYaml.Models;

namespace UmbracoYaml.Services
{
    public class DocumentTypeCreator
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeService _dataTypeService;
        private readonly ILogger<DocumentTypeCreator> _logger;

        public DocumentTypeCreator(
            IContentTypeService contentTypeService,
            IDataTypeService dataTypeService,
            ILogger<DocumentTypeCreator> logger = null)
        {
            _contentTypeService = contentTypeService ?? throw new ArgumentNullException(nameof(contentTypeService));
            _dataTypeService = dataTypeService ?? throw new ArgumentNullException(nameof(dataTypeService));
            _logger = logger;
        }

        public void CreateDocumentTypes(List<YamlDocumentType> documentTypes)
        {
            var processedAliases = new HashSet<string>();

            foreach (var yamlDocType in documentTypes)
            {
                if (processedAliases.Contains(yamlDocType.Alias))
                {
                    _logger?.LogWarning($"Skipping duplicate DocumentType alias: {yamlDocType.Alias}");
                    continue;
                }

                try
                {
                    var existing = _contentTypeService.Get(yamlDocType.Alias);
                    if (existing != null)
                    {
                        _logger?.LogInformation($"DocumentType already exists: {yamlDocType.Alias}");
                        processedAliases.Add(yamlDocType.Alias);
                        continue;
                    }

                    var contentType = new ContentType(null)
                    {
                        Name = yamlDocType.Name,
                        Alias = yamlDocType.Alias,
                        Icon = yamlDocType.Icon ?? "icon-document",
                        AllowAsRoot = yamlDocType.AllowAsRoot
                    };

                    // Add tabs and properties
                    foreach (var tab in yamlDocType.Tabs)
                    {
                        var contentTab = new ContentPropertyGroup { Name = tab.Name };

                        foreach (var property in tab.Properties)
                        {
                            var dataType = _dataTypeService.Get(property.DataType);
                            if (dataType == null)
                            {
                                _logger?.LogWarning($"DataType not found: {property.DataType}, skipping property {property.Alias}");
                                continue;
                            }

                            var contentProp = new ContentPropertyType(dataType)
                            {
                                Alias = property.Alias,
                                Name = property.Name,
                                Mandatory = property.Required,
                                Description = property.Description
                            };

                            contentTab.PropertyTypes.Add(contentProp);
                        }

                        contentType.PropertyGroups.Add(contentTab);
                    }

                    // Set allowed child types
                    if (yamlDocType.AllowedChildTypes.Any())
                    {
                        var childTypes = yamlDocType.AllowedChildTypes
                            .Select(alias => _contentTypeService.Get(alias))
                            .Where(ct => ct != null)
                            .ToList();

                        contentType.AllowedContentTypes = childTypes.Select(ct => new ContentTypeSort(ct.Id, 0)).ToList();
                    }

                    _contentTypeService.Save(contentType);
                    processedAliases.Add(yamlDocType.Alias);
                    _logger?.LogInformation($"Created DocumentType: {yamlDocType.Alias}");
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"Error creating DocumentType {yamlDocType.Alias}: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
```

- [ ] **Step 3: Run tests to verify they pass**

```bash
dotnet test tests/DocumentTypeCreatorTests.cs -v
```

Expected: PASS (1 passing test)

- [ ] **Step 4: Commit**

```bash
git add src/Services/DocumentTypeCreator.cs tests/DocumentTypeCreatorTests.cs
git commit -m "feat: implement DocumentTypeCreator service for programmatic DocumentType creation"
```

---

## Task 6: Create TemplateCreator Service

**Files:**
- Create: `src/Services/TemplateCreator.cs`
- Create: `tests/TemplateCreatorTests.cs`

- [ ] **Step 1: Write failing test for TemplateCreator**

Create `tests/TemplateCreatorTests.cs`:
```csharp
using Xunit;
using Moq;
using UmbracoYaml.Services;
using UmbracoYaml.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using System.Collections.Generic;
using System.IO;

namespace UmbracoYaml.Tests
{
    public class TemplateCreatorTests
    {
        [Fact]
        public void CreateTemplates_ShouldCreateFromYaml()
        {
            var mockTemplateService = new Mock<ITemplateService>();
            var creator = new TemplateCreator(mockTemplateService.Object);

            var templates = new List<YamlTemplate>
            {
                new YamlTemplate
                {
                    Alias = "page",
                    Name = "Page",
                    Path = "Views/Page.cshtml"
                }
            };

            creator.CreateTemplates(templates);

            mockTemplateService.Verify(x =>
                x.Save(It.IsAny<ITemplate>()), Times.Once);
        }
    }
}
```

- [ ] **Step 2: Implement TemplateCreator**

Create `src/Services/TemplateCreator.cs`:
```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using Microsoft.Extensions.Logging;
using UmbracoYaml.Models;

namespace UmbracoYaml.Services
{
    public class TemplateCreator
    {
        private readonly ITemplateService _templateService;
        private readonly ILogger<TemplateCreator> _logger;
        private readonly string _viewsPath = Path.Combine(Directory.GetCurrentDirectory(), "Views");

        public TemplateCreator(ITemplateService templateService, ILogger<TemplateCreator> logger = null)
        {
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
            _logger = logger;
        }

        public void CreateTemplates(List<YamlTemplate> templates)
        {
            var processedAliases = new HashSet<string>();

            foreach (var yamlTemplate in templates)
            {
                if (processedAliases.Contains(yamlTemplate.Alias))
                {
                    _logger?.LogWarning($"Skipping duplicate Template alias: {yamlTemplate.Alias}");
                    continue;
                }

                try
                {
                    var existing = _templateService.GetByAlias(yamlTemplate.Alias);
                    if (existing != null)
                    {
                        _logger?.LogInformation($"Template already exists: {yamlTemplate.Alias}");
                        processedAliases.Add(yamlTemplate.Alias);
                        continue;
                    }

                    ITemplate masterTemplate = null;
                    if (!string.IsNullOrEmpty(yamlTemplate.MasterTemplate))
                    {
                        masterTemplate = _templateService.GetByAlias(yamlTemplate.MasterTemplate);
                        if (masterTemplate == null)
                        {
                            _logger?.LogWarning($"Master template not found: {yamlTemplate.MasterTemplate}");
                        }
                    }

                    var template = new Template(null)
                    {
                        Name = yamlTemplate.Name,
                        Alias = yamlTemplate.Alias,
                        Path = yamlTemplate.Path,
                        MasterTemplate = masterTemplate
                    };

                    // Create template file if it doesn't exist
                    var fullPath = Path.Combine(_viewsPath, yamlTemplate.Path);
                    if (!File.Exists(fullPath))
                    {
                        var directory = Path.GetDirectoryName(fullPath);
                        Directory.CreateDirectory(directory);
                        File.WriteAllText(fullPath, $"<!-- Template: {yamlTemplate.Name} -->\r\n@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage\r\n");
                    }

                    _templateService.Save(template);
                    processedAliases.Add(yamlTemplate.Alias);
                    _logger?.LogInformation($"Created Template: {yamlTemplate.Alias}");
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"Error creating Template {yamlTemplate.Alias}: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
```

- [ ] **Step 3: Run tests to verify they pass**

```bash
dotnet test tests/TemplateCreatorTests.cs -v
```

Expected: PASS (1 passing test)

- [ ] **Step 4: Commit**

```bash
git add src/Services/TemplateCreator.cs tests/TemplateCreatorTests.cs
git commit -m "feat: implement TemplateCreator service for programmatic Template creation"
```

---

## Task 7: Create ContentCreator Service

**Files:**
- Create: `src/Services/ContentCreator.cs`
- Create: `tests/ContentCreatorTests.cs`

- [ ] **Step 1: Write failing test for ContentCreator**

Create `tests/ContentCreatorTests.cs`:
```csharp
using Xunit;
using Moq;
using UmbracoYaml.Services;
using UmbracoYaml.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using System.Collections.Generic;

namespace UmbracoYaml.Tests
{
    public class ContentCreatorTests
    {
        [Fact]
        public void CreateContent_ShouldCreateFromYaml()
        {
            var mockContentService = new Mock<IContentService>();
            var mockContentTypeService = new Mock<IContentTypeService>();

            var contentType = new Mock<IContentType>();
            contentType.Setup(x => x.Id).Returns(1);
            mockContentTypeService.Setup(x => x.Get(It.IsAny<string>())).Returns(contentType.Object);

            var creator = new ContentCreator(mockContentService.Object, mockContentTypeService.Object);

            var content = new List<YamlContent>
            {
                new YamlContent
                {
                    Alias = "home",
                    Name = "Home",
                    Type = "page",
                    Published = true,
                    Values = new() { { "title", "Welcome" } }
                }
            };

            creator.CreateContent(content);

            mockContentService.Verify(x =>
                x.Save(It.IsAny<IContent>()), Times.Once);
        }
    }
}
```

- [ ] **Step 2: Implement ContentCreator**

Create `src/Services/ContentCreator.cs`:
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using Microsoft.Extensions.Logging;
using UmbracoYaml.Models;

namespace UmbracoYaml.Services
{
    public class ContentCreator
    {
        private readonly IContentService _contentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly ILogger<ContentCreator> _logger;

        public ContentCreator(
            IContentService contentService,
            IContentTypeService contentTypeService,
            ILogger<ContentCreator> logger = null)
        {
            _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
            _contentTypeService = contentTypeService ?? throw new ArgumentNullException(nameof(contentTypeService));
            _logger = logger;
        }

        public void CreateContent(List<YamlContent> contentList, int? parentId = null)
        {
            foreach (var yamlContent in contentList)
            {
                try
                {
                    var contentType = _contentTypeService.Get(yamlContent.Type);
                    if (contentType == null)
                    {
                        _logger?.LogError($"DocumentType not found: {yamlContent.Type}");
                        continue;
                    }

                    var existing = _contentService.GetById(yamlContent.Alias);
                    if (existing != null)
                    {
                        _logger?.LogInformation($"Content already exists: {yamlContent.Alias}");
                        continue;
                    }

                    var content = _contentService.Create(yamlContent.Name, parentId ?? -1, contentType.Alias);

                    // Set property values
                    foreach (var kvp in yamlContent.Values)
                    {
                        if (content.Properties.Any(p => p.Alias == kvp.Key))
                        {
                            content.SetValue(kvp.Key, kvp.Value);
                        }
                    }

                    content.SortOrder = yamlContent.SortOrder;

                    _contentService.Save(content);

                    if (yamlContent.Published)
                    {
                        _contentService.Publish(content);
                    }

                    _logger?.LogInformation($"Created Content: {yamlContent.Alias}");

                    // Recursively create children
                    if (yamlContent.Children.Any())
                    {
                        CreateContent(yamlContent.Children, content.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"Error creating content {yamlContent.Alias}: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
```

- [ ] **Step 3: Run tests to verify they pass**

```bash
dotnet test tests/ContentCreatorTests.cs -v
```

Expected: PASS (1 passing test)

- [ ] **Step 4: Commit**

```bash
git add src/Services/ContentCreator.cs tests/ContentCreatorTests.cs
git commit -m "feat: implement ContentCreator service for programmatic Content creation"
```

---

## Task 8: Implement YamlStartupComposer

**Files:**
- Modify: `src/Composers/YamlStartupComposer.cs`
- Create: `tests/YamlStartupComposerTests.cs`

- [ ] **Step 1: Write failing test for YamlStartupComposer**

Create `tests/YamlStartupComposerTests.cs`:
```csharp
using Xunit;
using Moq;
using UmbracoYaml.Composers;
using Umbraco.Cms.Core.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace UmbracoYaml.Tests
{
    public class YamlStartupComposerTests
    {
        [Fact]
        public void YamlStartupComposer_ShouldRegisterServices()
        {
            var mockBuilder = new Mock<Umbraco.Cms.Core.Composing.IUmbracoBuilder>();
            var composer = new YamlStartupComposer();

            composer.Compose(mockBuilder.Object);

            // Verify that services were added to DI
            mockBuilder.Verify(x =>
                x.Services.AddScoped(It.IsAny<Type>(), It.IsAny<Type>()),
                Times.AtLeastOnce);
        }
    }
}
```

- [ ] **Step 2: Implement YamlStartupComposer**

Modify `src/Composers/YamlStartupComposer.cs`:
```csharp
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using UmbracoYaml.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace UmbracoYaml.Composers
{
    public class YamlStartupComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Register plugin services
            builder.Services.AddScoped<YamlParser>();
            builder.Services.AddScoped<DataTypeCreator>();
            builder.Services.AddScoped<DocumentTypeCreator>();
            builder.Services.AddScoped<TemplateCreator>();
            builder.Services.AddScoped<ContentCreator>();

            // Register the startup notification handler
            builder.AddNotificationHandler<UmbracoApplicationStartedNotification, YamlInitializationHandler>();
        }
    }

    public class YamlInitializationHandler : INotificationHandler<UmbracoApplicationStartedNotification>
    {
        private readonly YamlParser _yamlParser;
        private readonly DataTypeCreator _dataTypeCreator;
        private readonly DocumentTypeCreator _documentTypeCreator;
        private readonly TemplateCreator _templateCreator;
        private readonly ContentCreator _contentCreator;
        private readonly ILogger<YamlInitializationHandler> _logger;
        private readonly IConfiguration _configuration;

        public YamlInitializationHandler(
            YamlParser yamlParser,
            DataTypeCreator dataTypeCreator,
            DocumentTypeCreator documentTypeCreator,
            TemplateCreator templateCreator,
            ContentCreator contentCreator,
            ILogger<YamlInitializationHandler> logger,
            IConfiguration configuration)
        {
            _yamlParser = yamlParser;
            _dataTypeCreator = dataTypeCreator;
            _documentTypeCreator = documentTypeCreator;
            _templateCreator = templateCreator;
            _contentCreator = contentCreator;
            _logger = logger;
            _configuration = configuration;
        }

        public void Handle(UmbracoApplicationStartedNotification notification)
        {
            _logger.LogInformation("YamlStartupComposer: Starting YAML initialization");

            try
            {
                var yamlPath = _configuration.GetValue<string>("UmbracoYaml:ConfigPath")
                    ?? "config/umbraco.yaml";

                var root = _yamlParser.ParseYaml(yamlPath);

                _logger.LogInformation($"Parsed YAML configuration from {yamlPath}");

                // Create structures in order
                if (root?.Umbraco?.DataTypes?.Count > 0)
                {
                    _logger.LogInformation($"Creating {root.Umbraco.DataTypes.Count} DataTypes");
                    _dataTypeCreator.CreateDataTypes(root.Umbraco.DataTypes);
                }

                if (root?.Umbraco?.DocumentTypes?.Count > 0)
                {
                    _logger.LogInformation($"Creating {root.Umbraco.DocumentTypes.Count} DocumentTypes");
                    _documentTypeCreator.CreateDocumentTypes(root.Umbraco.DocumentTypes);
                }

                if (root?.Umbraco?.Templates?.Count > 0)
                {
                    _logger.LogInformation($"Creating {root.Umbraco.Templates.Count} Templates");
                    _templateCreator.CreateTemplates(root.Umbraco.Templates);
                }

                if (root?.Umbraco?.Content?.Count > 0)
                {
                    _logger.LogInformation($"Creating {root.Umbraco.Content.Count} Content nodes");
                    _contentCreator.CreateContent(root.Umbraco.Content);
                }

                _logger.LogInformation("YamlStartupComposer: Completed successfully");
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning($"YAML configuration not found: {ex.Message}. Skipping initialization.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "YamlStartupComposer: Error during initialization");
                throw;
            }
        }
    }
}
```

- [ ] **Step 3: Run all tests to verify they pass**

```bash
dotnet test tests/ -v
```

Expected: PASS (all tests passing)

- [ ] **Step 4: Commit**

```bash
git add src/Composers/YamlStartupComposer.cs tests/YamlStartupComposerTests.cs
git commit -m "feat: implement YamlStartupComposer with startup handler for orchestration"
```

---

## Task 9: Create Example Configuration File

**Files:**
- Create: `config/umbraco.yaml`

- [ ] **Step 1: Create comprehensive example YAML**

Create `config/umbraco.yaml`:
```yaml
umbraco:
  dataTypes:
    - alias: textString
      name: Text String
      editor: Umbraco.TextBox
      config:
        maxLength: 255

    - alias: richText
      name: Rich Text Editor
      editor: Umbraco.RichText
      config:
        toolbar: "bold|italic|link"

    - alias: markdown
      name: Markdown Editor
      editor: Umbraco.MarkdownEditor
      config:
        preview: true

  documentTypes:
    - alias: page
      name: Page
      icon: icon-document
      allowAsRoot: true
      allowedChildTypes:
        - article
      tabs:
        - name: Content
          properties:
            - alias: title
              name: Page Title
              dataType: textString
              required: true
              description: "The page title shown in browser tabs"
            - alias: body
              name: Body Content
              dataType: richText
              required: false
        - name: SEO
          properties:
            - alias: metaDescription
              name: Meta Description
              dataType: textString
              required: false
            - alias: keywords
              name: Keywords
              dataType: textString
              required: false

    - alias: article
      name: Article
      icon: icon-document-alt
      allowAsRoot: false
      tabs:
        - name: Content
          properties:
            - alias: title
              name: Title
              dataType: textString
              required: true
            - alias: summary
              name: Summary
              dataType: textString
              required: true
            - alias: content
              name: Content
              dataType: richText
              required: true

  templates:
    - alias: page
      name: Page Template
      path: Views/Page.cshtml

    - alias: article
      name: Article Template
      path: Views/Article.cshtml
      masterTemplate: page

  content:
    - alias: home
      name: Home
      type: page
      published: true
      sortOrder: 0
      values:
        title: Welcome to Our Site
        body: "<p>This is the home page content.</p>"
        metaDescription: "Welcome to our site"
      children:
        - alias: about
          name: About Us
          type: article
          published: true
          sortOrder: 0
          values:
            title: About Our Company
            summary: "Learn more about us"
            content: "<p>We are a company focused on excellence.</p>"

        - alias: contact
          name: Contact
          type: article
          published: true
          sortOrder: 1
          values:
            title: Contact Us
            summary: "Get in touch"
            content: "<p>Email us at contact@example.com</p>"
```

- [ ] **Step 2: Commit**

```bash
git add config/umbraco.yaml
git commit -m "docs: add example YAML configuration file with sample structures and content"
```

---

## Task 10: Add appsettings Configuration

**Files:**
- Create: `appsettings.json`

- [ ] **Step 1: Create appsettings.json with plugin configuration**

Create `appsettings.json`:
```json
{
  "UmbracoYaml": {
    "ConfigPath": "config/umbraco.yaml"
  },
  "Umbraco": {
    "CMS": {
      "Global": {
        "Id": "00000000-0000-0000-0000-000000000000",
        "ServerRole": "Single"
      }
    }
  }
}
```

- [ ] **Step 2: Commit**

```bash
git add appsettings.json
git commit -m "chore: add appsettings.json with plugin configuration"
```

---

## Task 11: Final Integration Test

**Files:**
- Create: `tests/IntegrationTests.cs`

- [ ] **Step 1: Write integration test**

Create `tests/IntegrationTests.cs`:
```csharp
using Xunit;
using UmbracoYaml.Services;
using System.IO;

namespace UmbracoYaml.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public void FullYamlFlow_ShouldParseAndProcessCompleteYamlFile()
        {
            var parser = new YamlParser();
            var yamlPath = "tests/fixtures/sample.yaml";

            var result = parser.ParseYaml(yamlPath);

            Assert.NotNull(result);
            Assert.NotNull(result.Umbraco);
            Assert.NotEmpty(result.Umbraco.DataTypes);
            Assert.NotEmpty(result.Umbraco.DocumentTypes);
            Assert.NotEmpty(result.Umbraco.Templates);
            Assert.NotEmpty(result.Umbraco.Content);

            // Verify structure
            Assert.Equal("textString", result.Umbraco.DataTypes[0].Alias);
            Assert.Equal("page", result.Umbraco.DocumentTypes[0].Alias);
            Assert.Equal("page", result.Umbraco.Templates[0].Alias);
            Assert.Equal("home", result.Umbraco.Content[0].Alias);

            // Verify relationships
            Assert.Single(result.Umbraco.DocumentTypes[0].Tabs);
            Assert.Single(result.Umbraco.DocumentTypes[0].Tabs[0].Properties);
        }
    }
}
```

- [ ] **Step 2: Run all tests one final time**

```bash
dotnet test tests/ -v
```

Expected: PASS (all tests, integration included)

- [ ] **Step 3: Commit**

```bash
git add tests/IntegrationTests.cs
git commit -m "test: add integration test verifying complete YAML parsing flow"
```

---

## Task 12: Document and Build

**Files:**
- Create: `README.md`
- Create: `.github/workflows/build.yml` (optional)

- [ ] **Step 1: Create README.md**

Create `README.md`:
```markdown
# Umbraco YAML Plugin

A declarative, Infrastructure-as-Code style plugin for Umbraco 17 that enables programmatic creation of DataTypes, DocumentTypes, Templates, and Content from a YAML configuration file.

## Installation

1. Add this plugin to your Umbraco 17 project
2. Create a `config/umbraco.yaml` file in your project root
3. Restart your Umbraco application
4. The plugin automatically creates all structures defined in the YAML file on startup

## Configuration

Place your YAML configuration file at `config/umbraco.yaml` (configurable in `appsettings.json`):

```json
{
  "UmbracoYaml": {
    "ConfigPath": "config/umbraco.yaml"
  }
}
```

## YAML Schema

See `config/umbraco.yaml` for a complete example.

### DataTypes
```yaml
dataTypes:
  - alias: textString
    name: Text String
    editor: Umbraco.TextBox
    config:
      maxLength: 255
```

### DocumentTypes
```yaml
documentTypes:
  - alias: page
    name: Page
    icon: icon-document
    allowAsRoot: true
    tabs:
      - name: Content
        properties:
          - alias: title
            name: Title
            dataType: textString
            required: true
```

### Templates
```yaml
templates:
  - alias: page
    name: Page
    path: Views/Page.cshtml
    masterTemplate: null
```

### Content
```yaml
content:
  - alias: home
    name: Home
    type: page
    published: true
    values:
      title: Welcome
    children: []
```

## Testing

Run all tests:
```bash
dotnet test
```

## Architecture

- **YamlParser:** Deserializes YAML file
- **DataTypeCreator:** Creates DataTypes
- **DocumentTypeCreator:** Creates DocumentTypes with properties
- **TemplateCreator:** Creates Templates
- **ContentCreator:** Creates and publishes Content
- **YamlStartupComposer:** Orchestrates initialization on Umbraco startup

## License

MIT
```

- [ ] **Step 2: Verify project builds**

```bash
dotnet build
```

Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add README.md
git commit -m "docs: add comprehensive README with installation and usage instructions"
```

---

## Summary

This plan implements a complete, TDD-driven Umbraco YAML plugin with:
- ✅ 5 service classes (Parser, 4 Creators)
- ✅ Complete YAML model deserialization
- ✅ Startup orchestration via INotificationHandler
- ✅ Unit and integration tests
- ✅ Example configuration file
- ✅ Dependency injection setup
- ✅ Comprehensive logging
- ✅ Error handling

**Total: 12 tasks, ~50 commits for a production-ready plugin.**

---

Plan complete and saved to `docs/superpowers/plans/2026-03-28-umbraco-yaml-plugin.md`.

**Two execution options:**

**1. Subagent-Driven (recommended)** — I dispatch a fresh subagent per task, review between tasks, fast iteration

**2. Inline Execution** — Execute tasks in this session using executing-plans, batch execution with checkpoints

**Which approach?**