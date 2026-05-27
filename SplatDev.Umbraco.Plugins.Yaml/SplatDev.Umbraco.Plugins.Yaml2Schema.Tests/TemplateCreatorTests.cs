using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Tests
{
    public class TemplateCreatorTests
    {
        private readonly Mock<ITemplateService> _mockTemplateService;
        private readonly Mock<ILogger<TemplateCreator>> _mockLogger;
        private readonly TemplateCreator _templateCreator;

        public TemplateCreatorTests()
        {
            _mockTemplateService = new Mock<ITemplateService>();
            _mockLogger = new Mock<ILogger<TemplateCreator>>();
            _templateCreator = new TemplateCreator(_mockTemplateService.Object, _mockLogger.Object);
        }

        [Fact]
        public void CreateTemplates_ShouldCreateFromYaml()
        {
            // Arrange
            var templates = new List<YamlTemplate>
            {
                new YamlTemplate
                {
                    Alias = "masterPage",
                    Name = "Master Page",
                    Path = "Master",
                    MasterTemplate = null
                },
                new YamlTemplate
                {
                    Alias = "contentPage",
                    Name = "Content Page",
                    Path = "Content",
                    MasterTemplate = "masterPage"
                }
            };

            // Mock: GetAsync returns null (templates don't exist)
            _mockTemplateService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((ITemplate?)null);

            // Act
            _templateCreator.CreateTemplates(templates);

            // Assert
            // Verify that CreateAsync was called twice (once for each template)
            _mockTemplateService.Verify(
                x => x.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid?>()),
                Times.Exactly(2),
                "CreateAsync should have been called twice for two new templates"
            );
        }

        [Fact]
        public void CreateTemplates_ShouldSkipDuplicateAliases()
        {
            var templates = new List<YamlTemplate>
            {
                new YamlTemplate { Alias = "duplicatePage", Name = "Duplicate Page", Path = "Duplicate" },
                new YamlTemplate { Alias = "duplicatePage", Name = "Duplicate Page Again", Path = "Duplicate2" }
            };

            _mockTemplateService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((ITemplate?)null);

            _templateCreator.CreateTemplates(templates);

            _mockTemplateService.Verify(
                x => x.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid?>()),
                Times.Once,
                "CreateAsync should have been called only once - second duplicate should be skipped"
            );
        }

        // ── REMOVE ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateTemplates_ShouldRemoveExistingTemplate()
        {
            var existingTemplate = new Mock<ITemplate>();
            existingTemplate.Setup(x => x.Key).Returns(Guid.NewGuid());

            _mockTemplateService
                .Setup(x => x.GetAsync("oldPage"))
                .ReturnsAsync(existingTemplate.Object);

            _templateCreator.CreateTemplates(new List<YamlTemplate>
            {
                new YamlTemplate { Alias = "oldPage", Name = "Old Page", Remove = true }
            });

            _mockTemplateService.Verify(
                x => x.DeleteAsync(existingTemplate.Object.Key, It.IsAny<Guid>()),
                Times.Once);
        }

        [Fact]
        public void CreateTemplates_ShouldNotThrowWhenRemoveTargetMissing()
        {
            _mockTemplateService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((ITemplate?)null);

            var ex = Record.Exception(() => _templateCreator.CreateTemplates(new List<YamlTemplate>
            {
                new YamlTemplate { Alias = "gone", Name = "Gone", Remove = true }
            }));

            Assert.Null(ex);
            _mockTemplateService.Verify(
                x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never);
        }

        // ── UPDATE ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateTemplates_ShouldUpdateExistingTemplate()
        {
            var existingTemplate = new Mock<ITemplate>();
            existingTemplate.SetupProperty(x => x.Content);

            _mockTemplateService
                .Setup(x => x.GetAsync("masterPage"))
                .ReturnsAsync(existingTemplate.Object);

            _templateCreator.CreateTemplates(new List<YamlTemplate>
            {
                new YamlTemplate { Alias = "masterPage", Name = "Master Page", Update = true }
            });

            _mockTemplateService.Verify(
                x => x.UpdateAsync(existingTemplate.Object, It.IsAny<Guid>()),
                Times.Once);

            _mockTemplateService.Verify(
                x => x.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid?>()),
                Times.Never,
                "Should not create when update target was found");
        }

        [Fact]
        public void CreateTemplates_ShouldCreateWhenUpdateTargetMissing()
        {
            _mockTemplateService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((ITemplate?)null);

            _templateCreator.CreateTemplates(new List<YamlTemplate>
            {
                new YamlTemplate { Alias = "newPage", Name = "New Page", Update = true }
            });

            // update:true but not found → fall through to create
            _mockTemplateService.Verify(
                x => x.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid?>()),
                Times.Once);
        }

        // ── HTML injection ────────────────────────────────────────────────────

        [Fact]
        public void CreateTemplates_ShouldInjectStylesheetTagsIntoContent()
        {
            string? capturedContent = null;

            _mockTemplateService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((ITemplate?)null);

            _mockTemplateService
                .Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .Callback<string, string, string, Guid, Guid?>((_, _, content, _, _) => capturedContent = content);

            _templateCreator.CreateTemplates(new List<YamlTemplate>
            {
                new YamlTemplate
                {
                    Alias = "site",
                    Name = "Site",
                    Stylesheets = new List<string> { "css/site.css" },
                    Scripts = new List<string> { "js/app.js" }
                }
            });

            Assert.NotNull(capturedContent);
            Assert.Contains("<link rel=\"stylesheet\" href=\"/css/site.css\"", capturedContent);
            Assert.Contains("<script src=\"/js/app.js\">", capturedContent);
        }
    }
}
