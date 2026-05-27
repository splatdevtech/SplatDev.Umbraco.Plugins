using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using Microsoft.Extensions.Logging;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Tests
{
    public class DocumentTypeCreatorTests
    {
        private readonly Mock<IContentTypeService> _mockDocTypeService;
        private readonly Mock<IDataTypeService> _mockDataTypeService;
        private readonly Mock<ITemplateService> _mockTemplateService;
        private readonly DocumentTypeCreator _creator;

        public DocumentTypeCreatorTests()
        {
            _mockDocTypeService = new Mock<IContentTypeService>();
            _mockDataTypeService = new Mock<IDataTypeService>();
            _mockTemplateService = new Mock<ITemplateService>();

            var shortStringHelper = new Mock<IShortStringHelper>();
            shortStringHelper
                .Setup(x => x.CleanStringForSafeAlias(It.IsAny<string>()))
                .Returns((string s) => s.ToLower().Replace(" ", ""));

            _creator = new DocumentTypeCreator(
                _mockDocTypeService.Object,
                _mockDataTypeService.Object,
                _mockTemplateService.Object,
                shortStringHelper.Object);
        }

        // ── CREATE ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateDocumentTypes_ShouldCreateFromYaml()
        {
            _mockDocTypeService.Setup(x => x.Get(It.IsAny<string>())).Returns((IContentType?)null);

            var mockDataType = new Mock<IDataType>();
            _mockDataTypeService.Setup(x => x.GetDataType(It.IsAny<string>())).Returns(mockDataType.Object);

            _creator.CreateDocumentTypes(new List<YamlDocumentType>
            {
                new YamlDocumentType
                {
                    Alias = "page", Name = "Page", Icon = "icon-document", AllowAsRoot = true,
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
            });

            _mockDocTypeService.Verify(x => x.Save(It.IsAny<IContentType>()), Times.Once);
        }

        [Fact]
        public void CreateDocumentTypes_ShouldSkipDuplicateAliases()
        {
            _mockDocTypeService.Setup(x => x.Get(It.IsAny<string>())).Returns((IContentType?)null);

            _creator.CreateDocumentTypes(new List<YamlDocumentType>
            {
                new YamlDocumentType { Alias = "page", Name = "Page",   Tabs = new() },
                new YamlDocumentType { Alias = "page", Name = "Page 2", Tabs = new() }
            });

            _mockDocTypeService.Verify(x => x.Save(It.IsAny<IContentType>()), Times.Once);
        }

        [Fact]
        public void CreateDocumentTypes_ShouldThrowOnNullList()
        {
            Assert.Throws<ArgumentNullException>(() => _creator.CreateDocumentTypes(null!));
        }

        // ── REMOVE ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateDocumentTypes_ShouldRemoveExistingDocumentType()
        {
            var existing = new Mock<IContentType>();
            _mockDocTypeService.Setup(x => x.Get("page")).Returns(existing.Object);

            _creator.CreateDocumentTypes(new List<YamlDocumentType>
            {
                new YamlDocumentType { Alias = "page", Name = "Page", Remove = true, Tabs = new() }
            });

            _mockDocTypeService.Verify(x => x.Delete(existing.Object, It.IsAny<int>()), Times.Once);
            _mockDocTypeService.Verify(x => x.Save(It.IsAny<IContentType>()), Times.Never);
        }

        [Fact]
        public void CreateDocumentTypes_ShouldNotThrowWhenRemoveTargetMissing()
        {
            _mockDocTypeService.Setup(x => x.Get(It.IsAny<string>())).Returns((IContentType?)null);

            var ex = Record.Exception(() => _creator.CreateDocumentTypes(new List<YamlDocumentType>
            {
                new YamlDocumentType { Alias = "gone", Name = "Gone", Remove = true, Tabs = new() }
            }));

            Assert.Null(ex);
            _mockDocTypeService.Verify(x => x.Delete(It.IsAny<IContentType>(), It.IsAny<int>()), Times.Never);
        }

        // ── UPDATE ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateDocumentTypes_ShouldUpdateExistingDocumentType()
        {
            var existing = new Mock<IContentType>();
            existing.Setup(x => x.PropertyGroups).Returns(new PropertyGroupCollection());
            existing.Setup(x => x.AllowedContentTypes).Returns(Enumerable.Empty<ContentTypeSort>());

            _mockDocTypeService.Setup(x => x.Get("page")).Returns(existing.Object);

            _creator.CreateDocumentTypes(new List<YamlDocumentType>
            {
                new YamlDocumentType { Alias = "page", Name = "Page Updated", Update = true, Tabs = new() }
            });

            _mockDocTypeService.Verify(x => x.Save(existing.Object), Times.Once);
        }

        [Fact]
        public void CreateDocumentTypes_ShouldCreateWhenUpdateAndNotExists()
        {
            // update:true but not found → fall through to create
            _mockDocTypeService.Setup(x => x.Get(It.IsAny<string>())).Returns((IContentType?)null);

            _creator.CreateDocumentTypes(new List<YamlDocumentType>
            {
                new YamlDocumentType { Alias = "newPage", Name = "New Page", Update = true, Tabs = new() }
            });

            _mockDocTypeService.Verify(x => x.Save(It.IsAny<IContentType>()), Times.Once);
        }
    }
}
