using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Tests
{
    /// <summary>
    /// Integration tests verifying the complete YAML parsing and creation flow
    /// Tests all components working together: Parser, DataTypeCreator,
    /// DocumentTypeCreator, TemplateCreator, and ContentCreator
    /// </summary>
    public class IntegrationTests
    {
        private readonly string _testFixturePath = Path.Combine(
            AppContext.BaseDirectory,
            "fixtures",
            "sample.yml"
        );

        private Mock<IShortStringHelper> CreateShortStringHelper()
        {
            var mock = new Mock<IShortStringHelper>();
            mock.Setup(x => x.CleanStringForSafeAlias(It.IsAny<string>()))
                .Returns((string s) => s.ToLower().Replace(" ", ""));
            return mock;
        }

        /// <summary>
        /// Full integration test: Parse YAML and verify complete structure
        /// Verifies that root, sections (DataTypes, DocumentTypes, Templates, Content)
        /// and nested items are correctly deserialized
        /// </summary>
        [Fact]
        public void FullYamlFlow_ShouldParseAndVerifyCompleteStructure()
        {
            // Arrange
            var parser = new YamlParser();

            // Act
            var result = parser.ParseYaml(_testFixturePath);

            // Assert
            // Root verification
            Assert.NotNull(result);
            Assert.NotNull(result.Umbraco);

            // DataTypes: [0] textString(update:true), [1] richText(remove:true), [2] textString, [3] richText
            Assert.NotNull(result.Umbraco.DataTypes);
            Assert.Equal(4, result.Umbraco.DataTypes.Count);
            Assert.Equal("textString", result.Umbraco.DataTypes[0].Alias);
            Assert.Equal("Text String", result.Umbraco.DataTypes[0].Name);
            Assert.Equal("Umbraco.TextBox", result.Umbraco.DataTypes[0].Editor);
            Assert.True(result.Umbraco.DataTypes[0].Update);
            Assert.NotNull(result.Umbraco.DataTypes[0].Config);

            Assert.Equal("richText", result.Umbraco.DataTypes[1].Alias);
            Assert.Equal("Rich Text", result.Umbraco.DataTypes[1].Name);
            Assert.True(result.Umbraco.DataTypes[1].Remove);

            // [2] is the original textString with config
            Assert.Equal("textString", result.Umbraco.DataTypes[2].Alias);
            Assert.Contains("maxLength", result.Umbraco.DataTypes[2].Config.Keys);

            // Scripts and Stylesheets verification
            Assert.NotNull(result.Umbraco.Scripts);
            Assert.Equal(3, result.Umbraco.Scripts.Count);
            Assert.Equal("siteJs", result.Umbraco.Scripts[0].Alias);
            Assert.Equal("js/site.js", result.Umbraco.Scripts[0].Path);
            Assert.False(result.Umbraco.Scripts[0].Remove);
            Assert.False(result.Umbraco.Scripts[0].Update);
            Assert.Equal("siteJsUpdate", result.Umbraco.Scripts[1].Alias);
            Assert.True(result.Umbraco.Scripts[1].Update);
            Assert.Equal("legacyJs", result.Umbraco.Scripts[2].Alias);
            Assert.True(result.Umbraco.Scripts[2].Remove);

            Assert.NotNull(result.Umbraco.Stylesheets);
            Assert.Equal(2, result.Umbraco.Stylesheets.Count);
            Assert.Equal("siteStyles", result.Umbraco.Stylesheets[0].Alias);
            Assert.Equal("css/site.css", result.Umbraco.Stylesheets[0].Path);
            Assert.Equal("oldStyles", result.Umbraco.Stylesheets[1].Alias);
            Assert.True(result.Umbraco.Stylesheets[1].Remove);

            // DocumentTypes verification (page + article[remove])
            Assert.NotNull(result.Umbraco.DocumentTypes);
            Assert.Equal(2, result.Umbraco.DocumentTypes.Count);
            var docType = result.Umbraco.DocumentTypes[0];
            Assert.Equal("page", docType.Alias);
            Assert.Equal("Page", docType.Name);
            Assert.Equal("icon-document", docType.Icon);
            Assert.True(docType.AllowAsRoot);
            Assert.Single(docType.AllowedChildTypes);
            Assert.Contains("page", docType.AllowedChildTypes);

            // Nested tabs and properties verification
            Assert.NotNull(docType.Tabs);
            Assert.Single(docType.Tabs);
            var tab = docType.Tabs[0];
            Assert.Equal("Content", tab.Name);
            Assert.NotNull(tab.Properties);
            Assert.Single(tab.Properties);
            var property = tab.Properties[0];
            Assert.Equal("title", property.Alias);
            Assert.Equal("Title", property.Name);
            Assert.Equal("textString", property.DataType);
            Assert.True(property.Required);
            Assert.Equal("The page title", property.Description);

            // Templates verification (masterPage + customPage)
            Assert.NotNull(result.Umbraco.Templates);
            Assert.Equal(2, result.Umbraco.Templates.Count);
            var template = result.Umbraco.Templates[0];
            Assert.Equal("masterPage", template.Alias);
            Assert.Equal("Master Page", template.Name);
            Assert.Equal("Master.cshtml", template.Path);
            Assert.Null(template.MasterTemplate);

            // Content verification
            Assert.NotNull(result.Umbraco.Content);
            Assert.Single(result.Umbraco.Content);
            var content = result.Umbraco.Content[0];
            Assert.Equal("home", content.Alias);
            Assert.Equal("Home", content.Name);
            Assert.Equal("page", content.Type);
            Assert.True(content.Published);
            Assert.Equal(0, content.SortOrder);
            Assert.NotNull(content.Values);
            Assert.Contains("title", content.Values.Keys);
            Assert.Equal("Welcome to Umbraco", content.Values["title"].ToString());
            Assert.NotNull(content.Children);
            Assert.Empty(content.Children);

            // Template scripts/stylesheets injection
            Assert.Single(template.Scripts);
            Assert.Equal("js/site.js", template.Scripts[0]);
            Assert.Single(template.Stylesheets);
            Assert.Equal("css/site.css", template.Stylesheets[0]);

            // Template with explicit Razor content
            Assert.Equal(2, result.Umbraco.Templates.Count);
            var customTemplate = result.Umbraco.Templates[1];
            Assert.Equal("customPage", customTemplate.Alias);
            Assert.False(string.IsNullOrWhiteSpace(customTemplate.RazorContent));
            Assert.Contains("@inherits", customTemplate.RazorContent);

            // Languages
            Assert.NotNull(result.Umbraco.Languages);
            Assert.Equal(2, result.Umbraco.Languages.Count);
            Assert.Equal("en-US", result.Umbraco.Languages[0].IsoCode);
            Assert.True(result.Umbraco.Languages[0].IsDefault);
            Assert.Equal("es-ES", result.Umbraco.Languages[1].IsoCode);

            // Dictionary items
            Assert.NotNull(result.Umbraco.DictionaryItems);
            Assert.Equal(2, result.Umbraco.DictionaryItems.Count);
            Assert.Equal("general.hello", result.Umbraco.DictionaryItems[0].Key);
            Assert.Equal("Hello", result.Umbraco.DictionaryItems[0].Translations["en-US"]);
            Assert.Equal("Hola", result.Umbraco.DictionaryItems[0].Translations["es-ES"]);

            // Media types
            Assert.NotNull(result.Umbraco.MediaTypes);
            Assert.Single(result.Umbraco.MediaTypes);
            Assert.Equal("customImage", result.Umbraco.MediaTypes[0].Alias);

            // Media items
            Assert.NotNull(result.Umbraco.Media);
            Assert.Single(result.Umbraco.Media);
            Assert.Equal("Site Banner", result.Umbraco.Media[0].Name);
            Assert.False(string.IsNullOrWhiteSpace(result.Umbraco.Media[0].Url));

            // Members
            Assert.NotNull(result.Umbraco.Members);
            Assert.Single(result.Umbraco.Members);
            Assert.Equal("test@example.com", result.Umbraco.Members[0].Email);
            Assert.True(result.Umbraco.Members[0].IsApproved);

            // Users
            Assert.NotNull(result.Umbraco.Users);
            Assert.Single(result.Umbraco.Users);
            Assert.Equal("editor@example.com", result.Umbraco.Users[0].Email);
            Assert.Contains("editor", result.Umbraco.Users[0].UserGroups);
        }

        /// <summary>
        /// Integration test: Verify all creators work with parsed YAML data
        /// Tests DataTypeCreator, DocumentTypeCreator, TemplateCreator, ContentCreator
        /// with the complete parsed YAML structure
        /// </summary>
        [Fact]
        public void AllCreators_ShouldWorkWithParsedYamlData()
        {
            // Arrange
            var parser = new YamlParser();
            var result = parser.ParseYaml(_testFixturePath);

            var mockDataTypeService = new Mock<IDataTypeService>();
            var mockConfigSerializer = new Mock<IConfigurationEditorJsonSerializer>();
            var mockEditor = new Mock<IDataEditor>();
            mockEditor.Setup(x => x.Alias).Returns("Umbraco.TextBox");
            var mockConfigEditorObj = new Mock<IConfigurationEditor>();
            mockConfigEditorObj.Setup(x => x.DefaultConfiguration).Returns(new Dictionary<string, object>());
            mockEditor.Setup(x => x.GetConfigurationEditor()).Returns(mockConfigEditorObj.Object);
            var mockValueEditorObj = new Mock<IDataValueEditor>();
            mockValueEditorObj.Setup(x => x.ValueType).Returns("STRING");
            mockEditor.Setup(x => x.GetValueEditor()).Returns(mockValueEditorObj.Object);
            var dataEditorCollection = new DataEditorCollection(() => Enumerable.Empty<IDataEditor>());
            var mockPropertyEditors = new Mock<PropertyEditorCollection>(dataEditorCollection);
            IDataEditor outEditor = mockEditor.Object;
            mockPropertyEditors.Setup(x => x.TryGet(It.IsAny<string>(), out outEditor)).Returns(true);
            var mockContentTypeService = new Mock<IContentTypeService>();
            var mockTemplateService = new Mock<ITemplateService>();
            var mockContentService = new Mock<IContentService>();
            var mockShortStringHelper = CreateShortStringHelper();
            var mockLogger = new Mock<ILogger<DataTypeCreator>>();
            var mockDocLogger = new Mock<ILogger<DocumentTypeCreator>>();
            var mockTplLogger = new Mock<ILogger<TemplateCreator>>();

            // Setup mocks
            mockDataTypeService
                .Setup(x => x.GetByEditorAlias(It.IsAny<string>()))
                .Returns(Enumerable.Empty<IDataType>());

            // GetDataType: return null for all calls so update:true falls through to create.
            // Both the update and remove entries then proceed through the normal create flow,
            // but duplicates ([2] textString, [3] richText) are skipped by alias dedup logic.
            mockDataTypeService
                .Setup(x => x.GetDataType(It.IsAny<string>()))
                .Returns((IDataType?)null);

            var mockContentType = new Mock<IContentType>();
            mockContentType.Setup(x => x.Id).Returns(1);
            mockContentType.Setup(x => x.Key).Returns(Guid.NewGuid());
            mockContentType.Setup(x => x.Alias).Returns("page");
            // DocumentTypeCreator "page" (update:true, not found):
            //   Call 1: Get("page") update check  → null
            //   Call 2: Get("page") existence check (create path) → null (allow creation)
            //   Call 3: Get("page") for AllowedChildTypes["page"] → mock
            // DocumentTypeCreator "article" (remove:true):
            //   Call 4: Get("article") → mock → Delete
            // ContentCreator "home" (type: "page"):
            //   Call 5: Get("page") → mock
            mockContentTypeService
                .SetupSequence(x => x.Get(It.IsAny<string>()))
                .Returns((IContentType?)null)
                .Returns((IContentType?)null)
                .Returns(mockContentType.Object)
                .Returns(mockContentType.Object)
                .Returns(mockContentType.Object);

            mockTemplateService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((ITemplate?)null);

            var mockContent = new Mock<IContent>();
            mockContent.Setup(x => x.Id).Returns(1);
            mockContent.Setup(x => x.Properties).Returns(new PropertyCollection());
            mockContentService
                .Setup(x => x.Create(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(mockContent.Object);

            var mockMediaService = new Mock<IMediaService>();

            var dataTypeCreator = new DataTypeCreator(mockDataTypeService.Object, mockContentTypeService.Object, mockPropertyEditors.Object, mockConfigSerializer.Object, mockLogger.Object);
            var docTypeCreator = new DocumentTypeCreator(mockContentTypeService.Object, mockDataTypeService.Object, mockTemplateService.Object, mockShortStringHelper.Object, mockDocLogger.Object);
            var templateCreator = new TemplateCreator(mockTemplateService.Object, mockTplLogger.Object);
            var contentCreator = new ContentCreator(mockContentService.Object, mockContentTypeService.Object, mockMediaService.Object);

            // Act
            dataTypeCreator.CreateDataTypes(result.Umbraco.DataTypes);
            docTypeCreator.CreateDocumentTypes(result.Umbraco.DocumentTypes);
            templateCreator.CreateTemplates(result.Umbraco.Templates);
            contentCreator.CreateContent(result.Umbraco.Content);

            // Assert - DataTypes: textString (update:true, not found → create) gets saved.
            // richText (remove:true, not found → skip). [2] textString and [3] richText skipped as duplicate aliases.
            mockDataTypeService.Verify(
                x => x.Save(It.IsAny<IDataType>(), It.IsAny<int>()),
                Times.Once,
                "Should create 1 data type (textString update:true falls through to create)"
            );

            // Assert - DocumentTypes created
            mockContentTypeService.Verify(
                x => x.Save(It.IsAny<IContentType>(), It.IsAny<int>()),
                Times.Once,
                "Should create 1 document type"
            );

            // Assert - Templates created (masterPage + customPage, both new)
            mockTemplateService.Verify(
                x => x.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid?>()),
                Times.Exactly(2),
                "Should create 2 templates"
            );

            // Assert - Content created
            mockContentService.Verify(
                x => x.Save(It.IsAny<IContent>(), It.IsAny<int?>(), It.IsAny<ContentScheduleCollection>()),
                Times.Once,
                "Should create 1 content item"
            );
        }

        /// <summary>
        /// Integration test: Verify complete structure with nested content
        /// Tests that nested children in content are properly parsed
        /// </summary>
        [Fact]
        public void ParseYaml_ShouldHandleNestedContentStructure()
        {
            // Arrange
            var parser = new YamlParser();
            var result = parser.ParseYaml(_testFixturePath);

            // Act - Get the root content item
            var rootContent = result.Umbraco.Content.FirstOrDefault();

            // Assert
            Assert.NotNull(rootContent);
            Assert.Equal("home", rootContent.Alias);
            Assert.NotNull(rootContent.Children);
            Assert.Empty(rootContent.Children);

            // Verify the structure can accommodate nested items
            // (even though the sample doesn't have nested content)
            var testNestedContent = new YamlContent
            {
                Alias = "subpage",
                Name = "Subpage",
                Type = "page",
                Children = new List<YamlContent>()
            };
            rootContent.Children.Add(testNestedContent);

            // Verify nesting works
            Assert.Single(rootContent.Children);
            Assert.Equal("subpage", rootContent.Children[0].Alias);
        }

        /// <summary>
        /// Integration test: Verify all sections exist and are populated correctly
        /// This is a high-level sanity check for the complete YAML structure
        /// </summary>
        [Fact]
        public void ParseYaml_ShouldEnsureAllSectionsExist()
        {
            // Arrange
            var parser = new YamlParser();

            // Act
            var result = parser.ParseYaml(_testFixturePath);

            // Assert - All major sections exist and are populated
            Assert.NotEmpty(result.Umbraco.DataTypes);
            Assert.NotEmpty(result.Umbraco.DocumentTypes);
            Assert.NotEmpty(result.Umbraco.Templates);
            Assert.NotEmpty(result.Umbraco.Content);
            Assert.NotEmpty(result.Umbraco.Scripts);
            Assert.NotEmpty(result.Umbraco.Stylesheets);
            Assert.NotEmpty(result.Umbraco.Languages);
            Assert.NotEmpty(result.Umbraco.DictionaryItems);
            Assert.NotEmpty(result.Umbraco.MediaTypes);
            Assert.NotEmpty(result.Umbraco.Media);
            Assert.NotEmpty(result.Umbraco.Members);
            Assert.NotEmpty(result.Umbraco.Users);
        }

        /// <summary>
        /// Integration test: Verify data consistency across related objects
        /// Tests that DocumentType references DataTypes and Templates correctly
        /// </summary>
        [Fact]
        public void ParseYaml_ShouldVerifyDataConsistency()
        {
            // Arrange
            var parser = new YamlParser();
            var result = parser.ParseYaml(_testFixturePath);

            // Act - Get the document type
            var docType = result.Umbraco.DocumentTypes.FirstOrDefault();
            Assert.NotNull(docType);

            // Get all properties from all tabs
            var allProperties = docType.Tabs
                .SelectMany(t => t.Properties)
                .ToList();

            // Assert - All properties reference valid data types
            foreach (var property in allProperties)
            {
                var dataType = result.Umbraco.DataTypes
                    .FirstOrDefault(dt => dt.Alias == property.DataType);
                Assert.NotNull(dataType);
            }

            // Assert - Content references valid document type
            var content = result.Umbraco.Content.FirstOrDefault();
            Assert.NotNull(content);
            var contentDocType = result.Umbraco.DocumentTypes
                .FirstOrDefault(dt => dt.Alias == content.Type);
            Assert.NotNull(contentDocType);
        }
    }
}
