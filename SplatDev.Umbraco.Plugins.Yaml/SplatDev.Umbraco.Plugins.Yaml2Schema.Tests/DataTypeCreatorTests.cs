using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Tests
{
    public class DataTypeCreatorTests
    {
        private readonly Mock<IDataTypeService> _mockDataTypeService;
        private readonly Mock<IContentTypeService> _mockContentTypeService;
        private readonly Mock<PropertyEditorCollection> _mockPropertyEditors;
        private readonly Mock<IConfigurationEditorJsonSerializer> _mockConfigSerializer;
        private readonly Mock<ILogger<DataTypeCreator>> _mockLogger;
        private readonly DataTypeCreator _dataTypeCreator;
        private readonly Mock<IDataEditor> _mockEditor;

        public DataTypeCreatorTests()
        {
            _mockDataTypeService = new Mock<IDataTypeService>();
            _mockContentTypeService = new Mock<IContentTypeService>();
            var dataEditorCollection = new DataEditorCollection(() => Enumerable.Empty<IDataEditor>());
            _mockPropertyEditors = new Mock<PropertyEditorCollection>(dataEditorCollection);
            _mockConfigSerializer = new Mock<IConfigurationEditorJsonSerializer>();
            _mockLogger = new Mock<ILogger<DataTypeCreator>>();

            _mockEditor = new Mock<IDataEditor>();
            _mockEditor.Setup(x => x.Alias).Returns("Umbraco.TextBox");
            var mockValueEditor = new Mock<IDataValueEditor>();
            mockValueEditor.Setup(x => x.ValueType).Returns("STRING");
            _mockEditor.Setup(x => x.GetValueEditor()).Returns(mockValueEditor.Object);
            var mockConfigEditor = new Mock<IConfigurationEditor>();
            mockConfigEditor.Setup(x => x.DefaultConfiguration).Returns(new Dictionary<string, object>());
            _mockEditor.Setup(x => x.GetConfigurationEditor()).Returns(mockConfigEditor.Object);

            // Setup TryGet to return the mock editor for any alias
            IDataEditor outEditor = _mockEditor.Object;
            _mockPropertyEditors
                .Setup(x => x.TryGet(It.IsAny<string>(), out outEditor))
                .Returns(true);

            _dataTypeCreator = new DataTypeCreator(
                _mockDataTypeService.Object,
                _mockContentTypeService.Object,
                _mockPropertyEditors.Object,
                _mockConfigSerializer.Object,
                _mockLogger.Object);
        }

        // ── CREATE ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateDataTypes_ShouldCreateDataTypesFromYaml()
        {
            var dataTypes = new List<YamlDataType>
            {
                new YamlDataType { Alias = "customTextString", Name = "Custom Text String", Editor = "Umbraco.TextBox", Config = new() },
                new YamlDataType { Alias = "customRichText",   Name = "Custom Rich Text",   Editor = "Umbraco.RichText", Config = new() }
            };

            _mockDataTypeService
                .Setup(x => x.GetByEditorAlias(It.IsAny<string>()))
                .Returns(Enumerable.Empty<IDataType>());

            _dataTypeCreator.CreateDataTypes(dataTypes);

            _mockDataTypeService.Verify(
                x => x.Save(It.IsAny<IDataType>(), It.IsAny<int>()),
                Times.Exactly(2),
                "Save should be called once for each new DataType");
        }

        [Fact]
        public void CreateDataTypes_ShouldSkipDuplicateAliases()
        {
            var dataTypes = new List<YamlDataType>
            {
                new YamlDataType { Alias = "duplicateText", Name = "Duplicate Text",   Editor = "Umbraco.TextBox", Config = new() },
                new YamlDataType { Alias = "duplicateText", Name = "Duplicate Text 2", Editor = "Umbraco.TextBox", Config = new() }
            };

            _mockDataTypeService
                .Setup(x => x.GetByEditorAlias(It.IsAny<string>()))
                .Returns(Enumerable.Empty<IDataType>());

            _dataTypeCreator.CreateDataTypes(dataTypes);

            _mockDataTypeService.Verify(
                x => x.Save(It.IsAny<IDataType>(), It.IsAny<int>()),
                Times.Once,
                "Second duplicate alias should be skipped");
        }

        [Fact]
        public void CreateDataTypes_ShouldThrowOnNullList()
        {
            Assert.Throws<ArgumentNullException>(() => _dataTypeCreator.CreateDataTypes(null!));
        }

        // ── REMOVE ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateDataTypes_ShouldRemoveExistingDataType()
        {
            var existing = new Mock<IDataType>();
            _mockDataTypeService.Setup(x => x.GetDataType("Old Type")).Returns(existing.Object);

            _dataTypeCreator.CreateDataTypes(new List<YamlDataType>
            {
                new YamlDataType { Alias = "oldType", Name = "Old Type", Editor = "Umbraco.TextBox", Remove = true }
            });

            _mockDataTypeService.Verify(x => x.Delete(existing.Object, It.IsAny<int>()), Times.Once);
            _mockDataTypeService.Verify(x => x.Save(It.IsAny<IDataType>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void CreateDataTypes_ShouldNotThrowWhenRemoveTargetMissing()
        {
            _mockDataTypeService.Setup(x => x.GetDataType(It.IsAny<string>())).Returns((IDataType?)null);

            var ex = Record.Exception(() => _dataTypeCreator.CreateDataTypes(new List<YamlDataType>
            {
                new YamlDataType { Alias = "gone", Name = "Gone", Editor = "Umbraco.TextBox", Remove = true }
            }));

            Assert.Null(ex);
            _mockDataTypeService.Verify(x => x.Delete(It.IsAny<IDataType>(), It.IsAny<int>()), Times.Never);
        }

        // ── UPDATE ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateDataTypes_ShouldSkipSaveWhenUpdateAndExists()
        {
            var existing = new Mock<IDataType>();
            _mockDataTypeService.Setup(x => x.GetDataType("My Type")).Returns(existing.Object);

            _dataTypeCreator.CreateDataTypes(new List<YamlDataType>
            {
                new YamlDataType { Alias = "myType", Name = "My Type", Editor = "Umbraco.TextBox", Update = true }
            });

            // Existing found → skip (no Save)
            _mockDataTypeService.Verify(x => x.Save(It.IsAny<IDataType>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void CreateDataTypes_ShouldCreateWhenUpdateAndNotExists()
        {
            _mockDataTypeService.Setup(x => x.GetDataType(It.IsAny<string>())).Returns((IDataType?)null);
            _mockDataTypeService
                .Setup(x => x.GetByEditorAlias(It.IsAny<string>()))
                .Returns(Enumerable.Empty<IDataType>());

            _dataTypeCreator.CreateDataTypes(new List<YamlDataType>
            {
                new YamlDataType { Alias = "newType", Name = "New Type", Editor = "Umbraco.TextBox", Update = true }
            });

            _mockDataTypeService.Verify(x => x.Save(It.IsAny<IDataType>(), It.IsAny<int>()), Times.Once);
        }

        // ── valueType fallback ────────────────────────────────────────────────

        [Fact]
        public void CreateDataTypes_ShouldFallBackToTextAreaForNtextValueType()
        {
            // When primary editor is not found, set up fallback to succeed for Umbraco.TextArea
            var fallbackEditor = new Mock<IDataEditor>();
            fallbackEditor.Setup(x => x.Alias).Returns("Umbraco.TextArea");
            var fallbackValueEditor = new Mock<IDataValueEditor>();
            fallbackValueEditor.Setup(x => x.ValueType).Returns("NTEXT");
            fallbackEditor.Setup(x => x.GetValueEditor()).Returns(fallbackValueEditor.Object);
            var fallbackConfigEditor = new Mock<IConfigurationEditor>();
            fallbackConfigEditor.Setup(x => x.DefaultConfiguration).Returns(new Dictionary<string, object>());
            fallbackEditor.Setup(x => x.GetConfigurationEditor()).Returns(fallbackConfigEditor.Object);

            // Primary editor NOT found, fallback IS found
            var dataEditorCollection = new DataEditorCollection(() => Enumerable.Empty<IDataEditor>());
            var mockPE = new Mock<PropertyEditorCollection>(dataEditorCollection);

            IDataEditor? primaryOut = null;
            mockPE.Setup(x => x.TryGet("My.CustomEditor", out primaryOut)).Returns(false);

            IDataEditor fallbackOut = fallbackEditor.Object;
            mockPE.Setup(x => x.TryGet("Umbraco.TextArea", out fallbackOut)).Returns(true);

            _mockDataTypeService
                .Setup(x => x.GetByEditorAlias(It.IsAny<string>()))
                .Returns(Enumerable.Empty<IDataType>());

            var creator = new DataTypeCreator(
                _mockDataTypeService.Object,
                _mockContentTypeService.Object,
                mockPE.Object,
                _mockConfigSerializer.Object,
                _mockLogger.Object);

            var ex = Record.Exception(() => creator.CreateDataTypes(new List<YamlDataType>
            {
                new YamlDataType
                {
                    Alias = "customEditor",
                    Name = "Custom Frontend Editor",
                    Editor = "My.CustomEditor",
                    ValueType = "NTEXT"
                }
            }));

            Assert.Null(ex);

            // DataType should be saved (via fallback editor)
            _mockDataTypeService.Verify(x => x.Save(It.IsAny<IDataType>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void CreateDataTypes_ShouldSkipWhenEditorNotFoundAndNoValueType()
        {
            // Neither primary nor fallback found (no valueType provided)
            var dataEditorCollection = new DataEditorCollection(() => Enumerable.Empty<IDataEditor>());
            var mockPE = new Mock<PropertyEditorCollection>(dataEditorCollection);

            IDataEditor? noOut = null;
            mockPE.Setup(x => x.TryGet(It.IsAny<string>(), out noOut)).Returns(false);

            _mockDataTypeService
                .Setup(x => x.GetByEditorAlias(It.IsAny<string>()))
                .Returns(Enumerable.Empty<IDataType>());

            var creator = new DataTypeCreator(
                _mockDataTypeService.Object,
                _mockContentTypeService.Object,
                mockPE.Object,
                _mockConfigSerializer.Object,
                _mockLogger.Object);

            creator.CreateDataTypes(new List<YamlDataType>
            {
                new YamlDataType
                {
                    Alias = "unknown",
                    Name = "Unknown Editor",
                    Editor = "Totally.Unknown.Editor"
                    // No ValueType
                }
            });

            // Should skip — no Save
            _mockDataTypeService.Verify(x => x.Save(It.IsAny<IDataType>(), It.IsAny<int>()), Times.Never);
        }

        // ── LinkBlockListElementTypes ──────────────────────────────────────────

        [Fact]
        public void LinkBlockListElementTypes_ShouldNotThrowOnNullList()
        {
            var ex = Record.Exception(() => _dataTypeCreator.LinkBlockListElementTypes(null!));
            Assert.Null(ex);
        }

        [Fact]
        public void LinkBlockListElementTypes_ShouldSkipNonBlockListDataTypes()
        {
            _dataTypeCreator.LinkBlockListElementTypes(new List<YamlDataType>
            {
                new YamlDataType { Alias = "textString", Name = "Text String", Editor = "Umbraco.TextBox" }
            });

            // IContentTypeService.Get should never be called for non-block-list types
            _mockContentTypeService.Verify(x => x.Get(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void LinkBlockListElementTypes_ShouldSkipBlockListWithNoConfig()
        {
            _dataTypeCreator.LinkBlockListElementTypes(new List<YamlDataType>
            {
                new YamlDataType { Alias = "myList", Name = "My List", Editor = "Umbraco.BlockList", Config = null }
            });

            _mockContentTypeService.Verify(x => x.Get(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void LinkBlockListElementTypes_ShouldSkipBlockListWithNoBlocks()
        {
            _dataTypeCreator.LinkBlockListElementTypes(new List<YamlDataType>
            {
                new YamlDataType
                {
                    Alias = "myList",
                    Name = "My List",
                    Editor = "Umbraco.BlockList",
                    Config = new Dictionary<string, object>
                    {
                        ["blocks"] = new List<object>()
                    }
                }
            });

            _mockContentTypeService.Verify(x => x.Get(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void LinkBlockListElementTypes_ShouldResolveAliasToKey()
        {
            var mockContentType = new Mock<IContentType>();
            mockContentType.Setup(x => x.Key).Returns(Guid.Parse("11111111-1111-1111-1111-111111111111"));

            _mockContentTypeService.Setup(x => x.Get("myElement")).Returns(mockContentType.Object);

            // Simulate what the DataType would look like after YAML parsing
            // YamlDotNet uses Dictionary<object,object> for nested mappings
            var blockDict = new System.Collections.Hashtable
            {
                ["contentElementTypeAlias"] = "myElement"
            };

            var mockDataType = new Mock<DataType>(It.IsAny<IDataEditor>(), It.IsAny<IConfigurationEditorJsonSerializer>());
            _mockDataTypeService.Setup(x => x.GetDataType("My Block List")).Returns((IDataType?)null);

            _dataTypeCreator.LinkBlockListElementTypes(new List<YamlDataType>
            {
                new YamlDataType
                {
                    Alias = "myList",
                    Name = "My Block List",
                    Editor = "Umbraco.BlockList",
                    Config = new Dictionary<string, object>
                    {
                        ["blocks"] = new List<object> { blockDict }
                    }
                }
            });

            // Content type lookup should be called for the alias
            _mockContentTypeService.Verify(x => x.Get("myElement"), Times.Once);
        }

        [Fact]
        public void LinkBlockListElementTypes_ShouldWarnWhenElementTypeNotFound()
        {
            _mockContentTypeService.Setup(x => x.Get(It.IsAny<string>())).Returns((IContentType?)null);

            var blockDict = new Dictionary<string, object>
            {
                ["contentElementTypeAlias"] = "missingElement"
            };

            _dataTypeCreator.LinkBlockListElementTypes(new List<YamlDataType>
            {
                new YamlDataType
                {
                    Alias = "myList",
                    Name = "My Block List",
                    Editor = "Umbraco.BlockList",
                    Config = new Dictionary<string, object>
                    {
                        ["blocks"] = new List<object> { blockDict }
                    }
                }
            });

            // Should log a warning for the missing element type
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("missingElement")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void LinkBlockListElementTypes_ShouldAlsoProcessBlockGridDataTypes()
        {
            var mockContentType = new Mock<IContentType>();
            mockContentType.Setup(x => x.Key).Returns(Guid.NewGuid());
            _mockContentTypeService.Setup(x => x.Get("gridElement")).Returns(mockContentType.Object);
            _mockDataTypeService.Setup(x => x.GetDataType(It.IsAny<string>())).Returns((IDataType?)null);

            var blockDict = new Dictionary<string, object>
            {
                ["contentElementTypeAlias"] = "gridElement"
            };

            _dataTypeCreator.LinkBlockListElementTypes(new List<YamlDataType>
            {
                new YamlDataType
                {
                    Alias = "myGrid",
                    Name = "My Grid",
                    Editor = "Umbraco.BlockGrid",
                    Config = new Dictionary<string, object>
                    {
                        ["blocks"] = new List<object> { blockDict }
                    }
                }
            });

            _mockContentTypeService.Verify(x => x.Get("gridElement"), Times.Once);
        }
    }
}
